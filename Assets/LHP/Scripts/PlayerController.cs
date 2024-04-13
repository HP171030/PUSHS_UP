using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections.Generic;
using Unity.VisualScripting;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.InputSystem.HID;

public class PlayerController : MonoBehaviour
{
    public Vector3 moveDir = Vector3.zero;

    public bool moveOn;
    public bool inputKey = true;
    bool onSpace = false;
    bool pullOn = false;
    bool grabOn;
    [Header("Player")]
    [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;
    [SerializeField] float pullSpeed;

    [Header("Configs")]
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask wall;
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask crystal;
    [SerializeField] public bool onIce = false;
    [SerializeField] LayerMask NonePlayer;

    [SerializeField] CameraSwitch cameraSwitch;

    Transform obstacle;
    RaycastHit otherObs;

    Vector3 debugVec;

    RaycastHit grabHit;
    private void FixedUpdate()
    {
        OffPull();
        MoveFunc();

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
       
        inputKey = true;

    }
    private void OnMove( InputValue value )
    {
        if ( cameraSwitch.IsPlayer1Active && !onIce && inputKey )
        {
            Vector2 input = value.Get<Vector2>();
            moveDir = new Vector3(input.x, 0, input.y);

        }

    }

    public void OnPull( InputValue value )
    {
        if ( value.isPressed && cameraSwitch.IsPlayer1Active )
        {
            grabOn = true;
           
            animator.SetTrigger("PullStart");

            if ( Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, out grabHit, 1.5f) )
            {
                if ( obstacleLayer.Contain(grabHit.collider.gameObject.layer) )
                {
                    animator.SetBool("Pull", true);
        

                }

            }
        }
        else
        {

            Debug.Log("offPull");

            grabOn = false;

        }

    }
    public void OffPull()
    {
        if ( !grabOn && !moveOn )
        {
            animator.SetBool("Pull", false);
        }

    }
    public void MoveFunc()
    {
        if ( !moveOn && !grabOn && !onIce )
        {

            animator.SetFloat("MoveSpeed", moveDir.magnitude);
            if ( Mathf.Abs(moveDir.x) != 0 && Mathf.Abs(moveDir.z) != 0 )
            {

                return;
            }
            else if ( !moveOn && moveDir.magnitude > 0 )
            {
                moveOn = true;
                transform.forward = moveDir;
                StartCoroutine(MoveRoutine(moveDir));

            }

        }
        else if ( !moveOn && grabOn && moveDir.magnitude > 0 )
        {
           
            if ( grabHit.collider != null )
            {
               
                Vector3 grabDir = ( grabHit.collider.gameObject.transform.position - transform.position ).normalized;
                if ( Physics.Raycast(transform.position + new Vector3(0, 1, 0), -transform.forward, out RaycastHit hit, 1.5f) )
                {
                    if ( hit.collider != null )
                    {
                       
                        moveOn = false;
                        return;
                    }
                }

                if ( grabDir.x > 0.9f && moveDir.x < 0f )
                {

                    bool X = true;
                    StartCoroutine(PullRoutine(grabDir, X));
                  
                }
                else if ( grabDir.x < -0.9f && moveDir.x > 0f )
                {
                    bool X = true;
                    StartCoroutine(PullRoutine(grabDir, X));
                  
                }
                else if ( grabDir.z > 0.9f && moveDir.z < 0f )
                {
                    bool X = false;
                    StartCoroutine(PullRoutine(grabDir, X));
                   
                }
                else if ( grabDir.z < -0.9f && moveDir.z > 0f )
                {
                    bool X = false;
                    StartCoroutine(PullRoutine(grabDir, X));
                  
                }
                else
                {
                    
                    moveOn = false;
                    pullOn = false;
                    Debug.Log("elseGrab");
                }

            }


        }
        else if (!moveOn&& !grabOn && !pullOn && moveDir.magnitude <= 0.1f )
        {
            
            animator.SetBool("Pull", false);
        }


    }

    private IEnumerator PullRoutine( Vector3 pullDir, bool X )
    {
        pullOn = true;
        moveOn = true;
        LayerMask backObsWall = wall | obstacleLayer;
        if ( Physics.Raycast(transform.position, -transform.forward, out RaycastHit somethingBack, 1f, backObsWall) )
        {
            Debug.Log($"{somethingBack.collider.gameObject.name}");
            yield break;
        }
        Vector3 startPos = transform.position;
        Vector3 targetPos = pullDir;
        Vector3 grabStartPos = grabHit.collider.transform.position;
        Vector3 grabTargetPos = pullDir;


        if ( X )
        {
            Collider [] pullTarget = Physics.OverlapSphere(transform.position - transform.forward * 2, 0.5f, ground | wall);
            Tile tile;

            if ( pullTarget.Length > 0 )
            {
                foreach ( Collider col in pullTarget )
                {
                    if ( wall.Contain(col.gameObject.layer) )
                    {
                      
                        Debug.Log($"{col.name}");
                        moveOn = false;
                        pullOn = false;
                        yield break;
                    }
                    tile = col.gameObject.GetComponent<Tile>();
                    if ( tile != null )
                    {
                        Debug.Log($" {tile.name}");
                        targetPos = tile.middlePoint.position;
                        debugVec = transform.position - transform.forward * 2;
                    }

                }
            }
        }
        else if ( !X )
        {
            Collider [] pullTarget = Physics.OverlapSphere(transform.position - transform.forward * 2, 0.5f, ground | wall);
            Tile tile;
            if ( pullTarget.Length > 0 )
            {
                foreach ( Collider col in pullTarget )
                {
                    if ( wall.Contain(col.gameObject.layer) )
                    {
                       
                        yield break;
                    }
                    tile = col.gameObject.GetComponent<Tile>();
                   
                    targetPos = tile.middlePoint.position;
                    debugVec = transform.position - transform.forward * 2;
                }
            }
        }
        Collider [] grabPullTarget = Physics.OverlapSphere(transform.position, 0.5f, ground);
        Tile grabTile;
        if ( grabPullTarget.Length > 0 )
        {
            foreach ( Collider col in grabPullTarget )
            {
                grabTile = col.gameObject.GetComponent<Tile>();
                if ( grabTile != null )
                    grabTargetPos = grabTile.middlePoint.position;
               

            }
        }


        List<RaycastHit> hitArray = new List<RaycastHit>(Physics.RaycastAll(grabHit.collider.transform.position, grabHit.collider.transform.up, 10f, obstacleLayer));

        hitArray.Add(grabHit);

        foreach ( RaycastHit hit in hitArray )
        {
            hit.collider.gameObject.transform.SetParent(grabHit.collider.transform, true);
            hit.rigidbody.isKinematic = true;
        }


        float time = 0;

        while ( time < pullSpeed )
        {

            time += Time.deltaTime;
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / pullSpeed));
            grabHit.rigidbody.MovePosition(Vector3.Lerp(grabStartPos, grabTargetPos, time / pullSpeed));
            yield return null;


            if ( time >= pullSpeed )
            {
                foreach ( RaycastHit hit in hitArray )
                {
                    hit.collider.gameObject.transform.SetParent(null, true);
                    hit.rigidbody.isKinematic = false;
                }
                Manager.game.StepAction++;

            }




        }
        moveOn = false;
        pullOn = false;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if ( debugVec != null )
            Gizmos.DrawWireSphere(debugVec, 0.5f);

    }
    private IEnumerator MoveRoutine(Vector3 moveDirValue)
    {
        Vector3 startPos = transform.position;
        Vector3 PreMoveDir = moveDir;

        LayerMask WandObslayer = obstacleLayer | wall;
        Vector3 targetPos = transform.position + moveDirValue *2 ;
        debugVec = targetPos;
        Collider [] tiles = Physics.OverlapSphere(targetPos, 0.5f, ground);               

        if ( tiles.Length > 0 )
        {

            foreach ( Collider tile in tiles )
            {

                Tile tileIns = tile.GetComponent<Tile>();                                             //내 앞쪽 타일 : tileIns

                if (tileIns != null)
                {

                    Collider [] isBlank = Physics.OverlapSphere(tileIns.middlePoint.position +new Vector3(0,1,0), 0.5f, WandObslayer);
                   
                    if ( isBlank.Length == 0 )
                    {
                        
                        targetPos = tileIns.middlePoint.position;
                     //   debugVec = targetPos;
                       
                        float time = 0;
                        while ( time < 1 )
                        {

                            time += Time.deltaTime * moveSpeed;
                            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                            if ( Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.forward, out RaycastHit hitinfo, 1f, NonePlayer) )
                            {
                                Debug.Log($"Someting {hitinfo.collider.name}");
                                transform.position = startPos;

                                moveOn = false;
                                yield break;
                            }
                            yield return null;
                        }
                        yield return null;
                        Manager.game.StepAction++;
                        moveOn = false;
                        if ( moveDir.magnitude < 1 || PreMoveDir != moveDir )
                        {
                            animator.SetBool("Push", false);
                        }
                    }
                    else
                    {                                               // 장애물이나 벽이 있는 경우
                        foreach ( Collider isCollider in isBlank )
                        {
                            Debug.Log($"{isCollider.name}이 앞에 있다");
                            if ( obstacleLayer.Contain(isCollider.gameObject.layer) )
                            {
                                Debug.Log(isCollider.name);
                                
                                Vector3 obsStartPos = isCollider.gameObject.transform.position;
                                Vector3 obsTargetPos = isCollider.gameObject.transform.position + moveDirValue * 2f;
                                Collider [] colliders = Physics.OverlapSphere(obsTargetPos, 0.5f, ground);
                                if ( colliders.Length > 0 )
                                {
                                    foreach ( Collider collider in colliders )
                                    {

                                        Tile obsTile = collider.GetComponent<Tile>();
                                        if ( obsTile != null )
                                        {
                                            Debug.Log($"obstacle foward is {obsTile.name}");
                                            obsTargetPos = obsTile.middlePoint.position;
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.Log("obstacle foward is blocked");
                                    moveOn = false;
                                    break;
                                }

                                float time = 0;

                                LayerMask layer = obstacleLayer | wall | crystal;
                                if ( Physics.BoxCast(isCollider.gameObject.transform.position, new Vector3(0.5f, 0.5f, 0.5f), moveDirValue, out RaycastHit hitInfo, Quaternion.identity, 0.7f, layer) )
                                {
                                    Debug.Log($" {hitInfo.collider.gameObject.name}");
                                    animator.SetBool("Push", false);
                                    moveOn = false;

                                    //    hit.rigidbody.isKinematic = false;
                                    if ( moveDir.magnitude < 1 || PreMoveDir != moveDir )
                                    {
                                        animator.SetBool("Push", false);
                                    }
                                }
                                else
                                {
                                    List<RaycastHit> pushHitArray = new List<RaycastHit>(Physics.RaycastAll(isCollider.transform.position, isCollider.transform.up, 10f, obstacleLayer));
                                    foreach ( RaycastHit hits in pushHitArray )
                                    {
                                        hits.collider.gameObject.transform.SetParent(isCollider.transform, true);
                                        hits.rigidbody.isKinematic = true;
                                        Debug.Log(hits.collider.gameObject.name);
                                    }
                                    while ( time < 2 )
                                    {
                                        if ( Physics.Raycast(isCollider.gameObject.transform.position + new Vector3(0, 0.5f, 0), moveDirValue, out RaycastHit notThis, 1f, layer) && notThis.collider.gameObject != isCollider.gameObject )
                                        {


                                            animator.SetBool("Push", false);
                                            moveOn = false;
                                            yield break;
                                        }

                                        animator.SetBool("Push", true);
                                        time += Time.deltaTime * moveSpeed;
                                        rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / 2));
                                        isCollider.GetComponent<Rigidbody>().MovePosition(Vector3.Lerp(obsStartPos, obsTargetPos, time / 2));


                                        yield return null;
                                    }
                                    Manager.game.StepAction++;
                                    moveOn = false;

                                    if ( moveDir.magnitude < 1 || PreMoveDir != moveDir )
                                    {
                                        animator.SetBool("Push", false);

                                    }
                                    foreach ( RaycastHit hits in pushHitArray )
                                    {
                                        hits.collider.gameObject.transform.SetParent(null, true);
                                        //  hits.rigidbody.isKinematic = false;
                                    }


                                }
                            }
                            else if ( wall.Contain(isCollider.gameObject.layer) )
                            {
                                Debug.Log($"wall name is {isCollider.gameObject.name}");
                                moveOn = false;
                                yield return null;

                            }
                            else
                            {
                                float time = 0;
                                while ( time < 1 )
                                {
                                    if ( Physics.Raycast(transform.position, transform.forward, out RaycastHit isWall, 0.2f, wall) )
                                    {
                                        Debug.Log($"wall {isWall}");
                                        transform.position = startPos;
                                        moveOn = false;
                                        yield break;
                                    }
                                    time += Time.deltaTime * moveSpeed;
                                    rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));


                                    yield return null;
                                }
                                yield return null;
                                Manager.game.StepAction++;
                                moveOn = false;
                                if ( moveDir.magnitude < 1 || PreMoveDir != moveDir )
                                {
                                    animator.SetBool("Push", false);
                                }
                            }

                        }
                    }
                }
                else
                {
                    Debug.Log("isNoneTile");
                    yield return null;
                    
                }

            }
        }

        }


    }

