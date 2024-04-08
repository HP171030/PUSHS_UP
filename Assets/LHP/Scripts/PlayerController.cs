using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections.Generic;
using Unity.VisualScripting;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{
    public Vector3 moveDir;

   public bool moveOn;
    bool pullOn;
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
    LayerMask layerMask;

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
         layerMask = ~( 1 << LayerMask.NameToLayer("Ground") );

    }
    private void OnMove( InputValue value )
    {
        if ( cameraSwitch.IsPlayer1Active &&!onIce )
        {
        Vector2 input = value.Get<Vector2>();
        moveDir = new Vector3(input.x, 0, input.y);

        }

    }
    
    public void OnPull( InputValue value)
    {
        if ( value.isPressed &&cameraSwitch.IsPlayer1Active)
        {
            grabOn = true;
            Debug.Log("잡기");
            animator.SetTrigger("PullStart");

            if ( Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, out grabHit, 1.5f) )
            {
                if (obstacleLayer.Contain(grabHit.collider.gameObject.layer))
                {
                    animator.SetBool("Pull", true);
                    Debug.Log("잡음");

                }

            }
        }
        else
        {

            grabOn = false;
            if(!pullOn)
            moveOn = false;
            Debug.Log("놓기");


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
        if ( !moveOn && !grabOn &&!onIce )
        {
           
            animator.SetFloat("MoveSpeed", moveDir.magnitude);
            if ( Mathf.Abs(moveDir.x) != 0 && Mathf.Abs(moveDir.z) != 0 )
            {

                return;
            }
            else if ( !moveOn && moveDir.magnitude > 0 )
            {
                moveOn = true;

                StartCoroutine(MoveRoutine(moveDir));

                transform.forward = moveDir;

            }

        }
        else if ( !moveOn && grabOn && moveDir.magnitude > 0 )
        {
            pullOn = true;
            if ( grabHit.collider != null )
            {
                Debug.Log($"잡음 {grabHit.collider.gameObject.name}");
                Vector3 grabDir = ( grabHit.collider.gameObject.transform.position - transform.position ).normalized;
                if(Physics.Raycast(transform.position + new Vector3(0,1,0),-transform.forward,out RaycastHit hit, 1.5f) )
                {
                    if ( hit.collider != null )
                    {
                        Debug.Log($"뒤에 벽잇음 {hit.collider.gameObject.name} ");
                        moveOn = false;
                        return;
                    }
                }
                
                if ( grabDir.x > 0.9f && moveDir.x < 0f )
                {

                    bool X = true;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}떙겨x");
                }
                else if ( grabDir.x < -0.9f && moveDir.x > 0f )
                {
                    bool X = true;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}떙겨x");
                }
                else if ( grabDir.z > 0.9f && moveDir.z < 0f )
                {
                    bool X = false;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}떙겨z");
                }
                else if ( grabDir.z < -0.9f && moveDir.z > 0f )
                {
                    bool X = false;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}떙겨z");
                }
                else
                {
                    Debug.Log("상정외 ");
                    moveOn = false;
                }

            }


        }


    }

    private IEnumerator PullRoutine( Vector3 pullDir, bool X )
    {
        moveOn = true;
        LayerMask backObsWall = wall | obstacleLayer;
        if ( Physics.Raycast(transform.position, -transform.forward,out RaycastHit somethingBack, 1f,backObsWall) )
        {
            Debug.Log($"뒤에 잇음 {somethingBack.collider.gameObject.name}");
            yield break;
        }
        Vector3 startPos = transform.position;
        Vector3 targetPos = pullDir;
        Vector3 grabStartPos = grabHit.collider.transform.position;
        Vector3 grabTargetPos = pullDir;
        if ( X )
        {
            targetPos = transform.position + new Vector3(-pullDir.x, 0, 0) * moveDistance;
            grabTargetPos = grabHit.collider.transform.position + new Vector3(-pullDir.x, 0, 0) * moveDistance;
        }
        else if ( !X )
        {
            targetPos = transform.position + new Vector3(0, 0, -pullDir.z) * moveDistance;
            grabTargetPos = grabHit.collider.transform.position + new Vector3(0, 0, -pullDir.z) * moveDistance;
        }


        List<RaycastHit> hitArray = new List<RaycastHit>(Physics.RaycastAll(grabHit.collider.transform.position, grabHit.collider.transform.up, 10f,obstacleLayer));

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

    }
    private void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if ( Physics.Raycast(ray, out hitInfo, 1f) )
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }

        Gizmos.DrawRay(ray);
        if ( debugVec != null)
        Gizmos.DrawWireSphere(debugVec, 0.5f);
        Gizmos.DrawWireCube(transform.position+new Vector3(0,1f,0) + moveDir, new Vector3(0.2f, 0.2f, 0.2f));
    }
    private IEnumerator MoveRoutine( Vector3 moveDirValue )
    {
        LayerMask WandObslayer = obstacleLayer | wall;
        Vector3 targetPos = transform.position + moveDirValue * moveDistance;
       Collider[] tiles =  Physics.OverlapSphere(targetPos, 0.5f, ground);
        if ( tiles.Length > 0 )
        {
            foreach ( Collider tile in tiles )
            {
                Tile tileIns = tile.GetComponent<Tile>();
                if ( tileIns != null )
                {
                    Collider [] isBlank = Physics.OverlapSphere(tileIns.transform.position + new Vector3(0, 1f, 0), 0.5f, WandObslayer);
                    
                    if ( isBlank.Length == 0 )
                    {
                        targetPos = tileIns.middlePoint.position;
                        debugVec = targetPos;
                        Debug.Log(tileIns.gameObject.name);
                    }
                }

            }
        }
        Vector3 startPos = transform.position;
        RaycastHit hit;
        Vector3 PreMoveDir = moveDir;
        if ( Physics.BoxCast(transform.position + new Vector3(0,1f,0),new Vector3(0.2f,0.2f,0.2f),moveDirValue*2f,out hit,Quaternion.identity,1f))
        {

            if ( obstacleLayer.Contain(hit.collider.gameObject.layer) )
            {
                Debug.Log("asff");
                obstacle = hit.transform;
                Vector3 obsStartPos = obstacle.position;
                Vector3 obsTargetPos = obstacle.position + moveDirValue * moveDistance;
                Collider[] colliders = Physics.OverlapSphere(obsTargetPos, 0.5f, ground);
                if(colliders.Length > 0 )
                {
                    foreach(Collider collider in colliders)
                    {
                        Tile obsTile = collider.GetComponent<Tile>();
                        obsTargetPos = obsTile.middlePoint.position;
                        
                    }
                }

                float time = 0;

                LayerMask layer = obstacleLayer | wall | crystal;
                if ( Physics.BoxCast(obstacle.position,new Vector3 (0.5f, 0.5f, 0.5f), moveDirValue, out RaycastHit hitInfo, Quaternion.identity, 0.7f,layer))
                {
                    Debug.Log($"뒤에 {hitInfo.collider.gameObject.name}가 있다");
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
                    List<RaycastHit> pushHitArray = new List<RaycastHit>(Physics.RaycastAll(hit.collider.transform.position, hit.collider.transform.up, 10f,obstacleLayer));
                    foreach ( RaycastHit hits in pushHitArray )
                    {
                        hits.collider.gameObject.transform.SetParent(hit.collider.transform, true);
                        hits.rigidbody.isKinematic = true;
                        Debug.Log(hits.collider.gameObject.name);
                    }
                    while ( time < 2 )
                    {
                        if ( Physics.Raycast(obstacle.position + new Vector3(0, 0.5f, 0), moveDirValue, out RaycastHit notThis, 1f, layer) && notThis.collider.gameObject != obstacle.gameObject )
                        {
                            
                            Debug.Log("뒤에");
                            animator.SetBool("Push", false);
                            moveOn = false;
                            yield break;
                        }
                       
                        animator.SetBool("Push", true);
                        time += Time.deltaTime * moveSpeed;
                        rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / 2));
                        hit.rigidbody.MovePosition(Vector3.Lerp(obsStartPos, obsTargetPos, time / 2));
                        Debug.Log(hit.collider.gameObject.name);

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

                    Debug.Log("장애물도 같이 무브");
                }
            }
            else if (wall.Contain(hit.collider.gameObject.layer)) 
            {
                Debug.Log($"wall name is {hit.collider.gameObject.name}");
                    moveOn = false;
                    yield return null;
                
            }
            else
            {
                float time = 0;
                while ( time < 1 )
                {
                    if(Physics.Raycast(transform.position,transform.forward,out RaycastHit isWall, 0.2f, wall) )
                    {
                        Debug.Log($"wall {isWall}");
                        transform.position = transform.position;
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
        else
        {
            float time = 0;
            while ( time < 1 )
            {

                time += Time.deltaTime * moveSpeed;
                rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                if(Physics.Raycast(transform.position+new Vector3 (0,0.5f,0),transform.forward,out RaycastHit hitinfo, 1f) )
                {
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




    }


}
