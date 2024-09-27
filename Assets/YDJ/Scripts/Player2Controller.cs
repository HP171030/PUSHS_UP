using System.Collections;
//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player2Controller : MonoBehaviour
{
    public Vector3 moveDir;

    public bool moveOn;
    public bool inputKey;
    [Header("Player")]
    [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;

    [Header("Configs")]
    [SerializeField] public Animator animator;
    [SerializeField] Rigidbody rb;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask obstacleUpperLayer;
    [SerializeField] LayerMask wall;
    [SerializeField] LayerMask ground;
    [SerializeField] public bool onGround;
    [SerializeField] public bool onClimb;
    [SerializeField] public bool ontheBox;
    [SerializeField] public bool downingBox;
    [SerializeField] public bool onIce = false;
    [SerializeField] LayerMask isNotplayerLayer;
    [SerializeField] RaycastHit debug;

    Vector3 debugVec;
    Vector3 debugVec2;

    [SerializeField] ParticleSystem boomEffect;

    [SerializeField] public CameraSwitch cameraSwitch;

    [Header("Sound")]
    [SerializeField] AudioClip WalkSound;
    [SerializeField] AudioClip cubeJumpSound;
    [SerializeField] AudioClip jumpDownSound;
    [SerializeField] AudioClip boomSound;




    RaycastHit BoomableHit;


    private void FixedUpdate()
    {
        if ( onGround && !downingBox )
            MoveFunc();

    }
    private void OnTriggerStay( Collider other )
    {
        if ( obstacleUpperLayer.Contain(other.gameObject.layer) )
        {

            ontheBox = true;
        }
    }
    private void OnTriggerExit( Collider other )
    {
        if ( obstacleUpperLayer.Contain(other.gameObject.layer) )
        {

            ontheBox = false;
        }
    }
    public IEnumerator DownAnim()
    {
        downingBox = true;
        animator.SetFloat("MoveSpeed", 0f);
        animator.Play("Jumping Down");
        yield return new WaitForSeconds(0.8f);
        downingBox = false;
    }
    private void Start()
    {


        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        inputKey = true;

    }
    private void OnMove( InputValue value )
    {
        if ( !cameraSwitch.IsPlayer1Active && !onIce && inputKey )
        {
            Vector2 input = value.Get<Vector2>();
            moveDir = new Vector3(input.x, 0, input.y);
        }

    }

    public void OnBoom( InputValue value )
    {
        if ( value.isPressed && !cameraSwitch.IsPlayer1Active && Manager.game.boomAction > 0 && !onClimb )
        {

            moveOn = true;
            // animator.SetTrigger("Boom");

            if ( Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, out BoomableHit, 2f) )                          // 잘 안맞는 버그 잇음 오버랩으로 바꿀것
            {
                if ( obstacleLayer.Contain(BoomableHit.collider.gameObject.layer) )
                {
                    Instantiate(boomEffect, BoomableHit.collider.gameObject.transform.position, Quaternion.identity);
                    Destroy(BoomableHit.collider.gameObject);
                    StartCoroutine(MoveReturn());
                    Manager.game.boomAction--;
                    Manager.sound.PlaySFX(boomSound);
                }
            }
            else
            {
                Debug.Log("없다");
            }
        }
    }
    IEnumerator MoveReturn()
    {
        yield return new WaitForSeconds(2f);
        moveOn = false;
    }
    public void MoveFunc()
    {
        if ( !moveOn && !onIce )
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
    }
    public void MoveExit()
    {
        moveOn = false;
        animator.SetFloat("MoveSpeed", 0f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(debugVec, 1f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(debugVec2, 1f);
    }
    private IEnumerator MoveRoutine( Vector3 moveDirValue )
    {
        LayerMask layer = obstacleLayer | wall;
        Vector3 targetPos = transform.position + moveDirValue * moveDistance;
        Vector3 startPos = transform.position;

        Collider [] tiles = Physics.OverlapSphere(targetPos, 0.3f, ground);
        if ( tiles.Length > 0 )
        {
            foreach ( Collider tile in tiles )
            {
                Tile tileIns = tile.GetComponent<Tile>();
                if ( tileIns != null )
                {
                    Collider [] isBlank = Physics.OverlapSphere(tileIns.middlePoint.transform.position + new Vector3(0, 1f, 0), 0.1f, wall);
                    debugVec2 = tileIns.middlePoint.transform.position + new Vector3(0, 1, 0);
                    if ( isBlank.Length == 0 )
                    {
                        targetPos = tileIns.middlePoint.position;


                    }
                    else
                    {
                        foreach ( Collider col in isBlank )
                        {
                            transform.position = transform.position;
                            moveOn = false;
                            yield break;
                        }

                    }
                }

            }
        }
        RaycastHit hit;
        Vector3 PreMoveDir = moveDir;

        if ( Physics.BoxCast(transform.position + new Vector3(0, 1f, 0), new Vector3(0.3f, 0.3f, 0.3f), moveDirValue, out hit, Quaternion.identity, 1f, layer) )
        {
            if ( !obstacleLayer.Contain(hit.collider.gameObject.layer) )
            {
                Debug.Log("장애물이 아님");
                Debug.Log(hit.collider.gameObject.name);
                moveOn = false;
                yield break;
            }
            else
            {
                RaycastHit [] raycastHits = Physics.RaycastAll(hit.collider.transform.position, hit.collider.transform.up, 3f, obstacleLayer);
                if(raycastHits.Length > 1 )
                {
                    Debug.Log($"위에 장애물이 있습니다");
                    moveOn = false;
                    yield break;
                }

               /* foreach ( RaycastHit other in raycastHits )
                {
                    Debug.Log(other.collider.name);
                    if ( other.collider.gameObject != hit.collider.gameObject && !obstacleUpperLayer.Contain(other.collider.gameObject.layer) )
                    {
                       
                    }

                }*/

                float time = 0;

                Vector3 ClimbPos = hit.collider.transform.position + new Vector3(0, 2, 0);
                hit.rigidbody.isKinematic = true;
                onClimb = true;
                animator.SetTrigger("ClimbStart");
                Manager.sound.PlaySFX(cubeJumpSound);
                while ( time < 1 )
                {
                    time += Time.deltaTime;
                    rb.MovePosition(Vector3.Lerp(startPos, ClimbPos, time));
                    yield return null;
                }
                Manager.game.StepAction++;
                if ( hit.collider != null )
                    //  hit.rigidbody.isKinematic = false;
                    animator.SetFloat("MoveSpeed", 0f);
                yield return new WaitForSeconds(0.5f);

                moveOn = false;
                onClimb = false;
                hit.rigidbody.isKinematic = false;
            }

        }
        else if ( ontheBox )            //박스에 올라가 있는 경우
        {
            LayerMask checkLayer = ground | obstacleLayer | wall;
            if ( Physics.Raycast(transform.position + new Vector3(0, 1f, 0), -transform.up, out RaycastHit onBox, 2f, obstacleLayer) )    // 박스를 검출하기 위한 레이캐스트
            {
                Debug.Log($"올라가있던 박스 : {onBox.collider.gameObject.name}");
                debug = onBox;
                Vector3 onBoxPos = onBox.collider.transform.position + new Vector3(0, 3f, 0);
                if ( Physics.Raycast(transform.position + new Vector3(0, 1f, 0), transform.forward, out RaycastHit isWall, 2f, wall) )         // 벽을 검출하기 위한 레이캐스트
                {
                    if ( wall.Contain(isWall.collider.gameObject.layer) )
                    {
                        Debug.Log("앞에 벽");
                        moveOn = false;
                        yield break;
                    }

                }
                else
                {

                    Collider [] colliders = Physics.OverlapSphere(onBoxPos + moveDirValue * 2f, 1f, checkLayer);         // 박스에 올라가 있는 상태에서 앞에 무엇이 있는지 확인하기 위한 오버랩
                    debugVec = onBoxPos + moveDirValue * 2f;
                    Debug.Log("벽은 없다");
                    Debug.Log(colliders.Length);
                    if ( colliders.Length > 0 )
                    {
                        foreach ( Collider collider in colliders )
                        {
                            Debug.Log($"서있는 박스 앞에 {collider.gameObject.name}");

                            if ( collider.gameObject != onBox.collider.gameObject )
                            {
                                Debug.Log("isDone");
                                if ( wall.Contain(collider.gameObject.layer) )
                                {
                                    Debug.Log("벽 검출");
                                    Collider [] isDoors = Physics.OverlapSphere(collider.transform.position + new Vector3(0, 1.5f, 0), 1f, ground);

                                    foreach ( Collider col in isDoors )
                                    {
                                        Debug.Log(col.name);
                                        if ( col.gameObject.CompareTag("Door") )
                                        {
                                            float time = 0;
                                            Manager.sound.PlaySFX(WalkSound);
                                            while ( time < 1 )
                                            {
                                                time += Time.deltaTime * moveSpeed;
                                                rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                                                yield return null;
                                            }
                                            Manager.game.StepAction++;
                                            moveOn = false;
                                            yield break;
                                        }
                                        else
                                        {
                                            Debug.Log($"아래에 벽이 있다 {collider.gameObject.name}");

                                            moveOn = false;
                                            yield break;
                                        }

                                    }
                                }

                                else
                                {
                                    RaycastHit [] raycastHits = Physics.RaycastAll(collider.transform.position, collider.transform.up, 3f, obstacleLayer);
                                    if ( raycastHits.Length > 1 )
                                    {
                                        Debug.Log($"위에 장애물이 있습니다");
                                        moveOn = false;
                                        yield break;
                                    }
                                    Debug.Log(collider.gameObject.name);
                                    float time = 0;
                                    Manager.sound.PlaySFX(WalkSound);
                                    while ( time < 1 )
                                    {
                                        time += Time.deltaTime * moveSpeed;
                                        rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                                        yield return null;
                                    }
                                    Manager.game.StepAction++;
                                    moveOn = false;
                                }

                            }

                        }
                        Debug.Log("isWhat");
                        moveOn = false;
                    }
                    else                                        // 박스에 올라가있는 상태에서 앞에 아무것도 없는 경우
                    {
                        Debug.Log("없나?");
                        LayerMask GOW = ground | obstacleLayer | wall;
                        
                        if ( Physics.Raycast(transform.position + moveDirValue * 2f, Vector3.down, out RaycastHit isTile, 2.5f, GOW) )        // 박스에 올라가있는 상태에서 움직임 방향에 무엇이 있는지 확인
                        {
                            Debug.Log($"{isTile.collider.gameObject.name}");
                            Tile Ank = isTile.collider.gameObject.GetComponent<Tile>();
                            if ( Ank != null )
                            {
                                Collider [] isBlank = Physics.OverlapSphere(Ank.transform.position + new Vector3(0, 1f, 0), 1f, layer);

                                if ( isBlank.Length == 0 )
                                {
                                    targetPos = Ank.middlePoint.position;
                                    Debug.Log(Ank.gameObject.name);
                                }
                            }
                            else
                            {

                                if ( wall.Contain(isTile.collider.gameObject.layer) )
                                {
                                    Debug.Log("못가는 블럭임");
                                    MoveExit();
                                    yield break;
                                }
                                targetPos = isTile.collider.gameObject.transform.position + new Vector3(0, 2, 0);
                            }
                            float time = 0;
                            StartCoroutine(DownAnim());
                            while ( time < 1 )
                            {
                                time += Time.deltaTime * moveSpeed;
                                rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                                yield return null;
                            }
                            Manager.sound.PlaySFX(jumpDownSound);
                            Manager.game.StepAction++;
                            MoveExit();
                            yield return null;
                        }
                        else
                        {
                            Debug.Log("너무 높다");
                            MoveExit();
                            yield break;
                        }
                    }


                }


            }
            else
            {
                Debug.Log("없을수는 없어");
                ontheBox = false;
                MoveExit();
                yield break;
            }

        }
        else // 앞에 아무것도 없음 && 평지임
        {
            float time = 0;
            Collider [] cols;
            while ( time < 1 )
            {
                cols = Physics.OverlapSphere(transform.position + new Vector3(0, 1, 0) + moveDirValue *0.2f,0.3f, layer);
                Debug.DrawLine(transform.position + new Vector3(0, 1, 0) + moveDirValue * 0.2f, transform.position + new Vector3(0, 1, 0) + moveDirValue * 1f, Color.red);
                if ( cols.Length > 0)
                {
                    foreach( Collider col in cols )
                    {
                        if ( obstacleLayer.Contain(col.gameObject.layer) )
                        {
                            RaycastHit [] raycastHits = Physics.RaycastAll(col.transform.position, col.transform.up, 3f, obstacleLayer);
                            if ( raycastHits.Length > 1 )
                            {
                                Debug.Log($"위에 장애물이 있습니다");
                                moveOn = false;
                                yield break;
                            }
                            else
                            {

                                Vector3 onPos = transform.position;
                                Vector3 ClimbPos = col.transform.position + new Vector3(0, 2, 0);
                                Rigidbody obsRb = col.gameObject.GetComponent<Rigidbody>();
                                obsRb.isKinematic = true;
                                onClimb = true;
                                animator.SetTrigger("ClimbStart");
                                Manager.sound.PlaySFX(cubeJumpSound);
                                while ( time < 1 )
                                {
                                    time += Time.deltaTime;
                                    rb.MovePosition(Vector3.Lerp(onPos, ClimbPos, time));
                                    yield return null;
                                }
                                Manager.game.StepAction++;
                                if ( hit.collider != null )
                                    //  hit.rigidbody.isKinematic = false;
                                    animator.SetFloat("MoveSpeed", 0f);
                                yield return new WaitForSeconds(0.5f);

                                moveOn = false;
                                onClimb = false;
                                obsRb.isKinematic = false;
                                yield break;
                            }
                        }
                        if ( wall.Contain(col.gameObject.layer) )
                        {
                            Debug.Log(col.gameObject.name);
                            moveOn = false;
                            transform.position = transform.position;
                            yield break;
                        }
                    }


                }
                time += Time.deltaTime * moveSpeed;
                rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                yield return null;
            }
            Manager.sound.PlaySFX(WalkSound);
            Manager.game.StepAction++;
            moveOn = false;
        }

    }
}
