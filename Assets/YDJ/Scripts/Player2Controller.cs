using System.Collections;
using System.Linq;

//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;

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
    IEnumerator ClimbRoutine( Transform target )
    {
        float time = 0;
        Vector3 startPos = transform.position;
        Vector3 ClimbPos = target.position + new Vector3(0, 2, 0);
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        targetRb.isKinematic = true;
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
        animator.SetFloat("MoveSpeed", 0f);
        yield return new WaitForSeconds(0.5f);

        moveOn = false;
        onClimb = false;
  //     targetRb.isKinematic = false;
    }
    Vector3? TileCheck( Vector3 targetPos )
    {
        Vector3 tilePosition;
        Collider [] tiles = Physics.OverlapSphere(targetPos, 0.5f, ground);
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
                        return tilePosition = tileIns.middlePoint.position;
                    }
                }
            }
        }
        return null;
    }
    LayerMask FrontCheck( Vector3 targetPos )
    {
        Collider [] cols = Physics.OverlapSphere(targetPos, 0.3f);
        LayerMask layer = 0;
        foreach ( Collider col in cols )
        {
            layer |= 1 << col.gameObject.layer;
        }
        return layer;
    }
    LayerMask FrontCheck( Vector3 targetPosition, out Collider [] frontColliders )
    {
        frontColliders = Physics.OverlapSphere(targetPosition, 0.5f);
        LayerMask layer = 0;
        foreach ( Collider col in frontColliders )
        {
            layer |= 1 << col.gameObject.layer;
        }
        return (int)layer;
    }
    LayerMask FrontCheck( Vector3 targetPosition, out Collider [] frontColliders,int layerCondition )
    {
        frontColliders = Physics.OverlapSphere(targetPosition, 0.5f, layerCondition);
        LayerMask layer = 0;
        foreach ( Collider col in frontColliders )
        {
            layer |= 1 << col.gameObject.layer;
        }
        return layer;
    }
    bool UpperCheck( Transform target )
    {
        RaycastHit [] raycastHits = Physics.RaycastAll(target.position, target.up, 3f, obstacleLayer);
        return raycastHits.Length > 1;
    }


    private IEnumerator MoveRoutine( Vector3 moveDirValue )
    {
        Vector3 targetPos = transform.position + moveDirValue * moveDistance;
        Vector3 startPos = transform.position;

        Vector3? result = TileCheck(targetPos);
        moveOn = true;
        targetPos = result ?? targetPos;


        if ( !ontheBox )              //평지인 경우
        {
            if ( FrontCheck(targetPos, out Collider [] cols,obstacleLayer | wall).Contain(obstacleLayer|wall) )        //앞에 뭐 있는지 체크
            {
                if ( cols.Any(col => obstacleLayer.Contain(col.gameObject.layer)) )        //만약 장애물이라면
                {
                    Collider col = cols.FirstOrDefault();
                    if ( UpperCheck(col.transform) )           //장애물인데 올라갈 수 없게 뭔가 있다면
                    {
                        moveOn = false;
                        yield break;
                    }
                    StartCoroutine(ClimbRoutine(col.transform));
                }
                else                            //벽인 경우
                {
                    if ( cols.Any(collider => wall.Contain(collider.gameObject.layer)) )
                    {
                        Debug.Log("벽이 있습니다.");
                        moveOn = false;
                        yield break;
                    }
                    else
                    {
                        foreach(Collider collider in cols )
                        {
                            Debug.Log(collider.gameObject.name);
                        }
                        moveOn = false;
                        yield break;
                    }
                }
            }
            else
            {

                float time = 0;
                Collider [] colliders;
                if(result == null )
                {
                    MoveExit();
                    yield break;
                }
                while ( time < 1 )
                {
                    colliders = Physics.OverlapSphere(transform.position + new Vector3(0, 1, 0) + moveDirValue * 0.2f, 0.3f, obstacleLayer|wall);
                    if ( colliders.Length > 0 )
                    {
                        foreach ( Collider col in colliders )
                        {
                            Debug.Log(col);
                            if ( obstacleLayer.Contain(col.gameObject.layer) )
                            {
                                if ( UpperCheck(col.transform) )
                                {
                                    Debug.Log("UpperCheck");
                                    MoveExit();
                                    yield break;
                                }
                                else
                                {
                                    StartCoroutine(ClimbRoutine(col.transform));
                                    yield break;
                                }
                            }
                            if ( wall.Contain(col.gameObject.layer) )
                            {
                                Debug.Log("Wall Check");
                                MoveExit();
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
        else            //박스에 올라가 있는 경우
        {
            LayerMask checkLayer = ground | obstacleLayer | wall;
            if ( Physics.Raycast(transform.position + new Vector3(0, 1f, 0), -transform.up, out RaycastHit onBox, 2f, obstacleLayer) )    // 박스를 검출하기 위한 레이캐스트
            {
                Debug.Log($"올라가있던 박스 : {onBox.collider.gameObject.name}");
                debug = onBox;
                Vector3 onBoxPos = onBox.collider.transform.position + new Vector3(0, 3f, 0);
                debugVec = targetPos + Vector3.up;
                LayerMask targetLayer = FrontCheck(targetPos + Vector3.up, out Collider [] colliders, obstacleLayer | wall);
                Debug.Log("Target Layer: " + ( int )targetLayer);
                Debug.Log("Obstacle Layer Mask: " + LayerMask.GetMask("Obstacle"));
                Debug.Log("Wall Layer Mask: " + LayerMask.GetMask("Wall"));
                if ( targetLayer.Contain(LayerMask.GetMask("Obstacle")) ||targetLayer.Contain(LayerMask.GetMask("Wall")))        //앞에 뭐 있는지 체크
                {
                    Debug.Log("앞에 뭔가 있긴 있음");
                    if ( colliders.Any(col => obstacleLayer.Contain(col.gameObject.layer)) )        //만약 장애물이라면
                    {
                        Debug.Log("앞에 뭔가가 장애물이었음");
                        Collider col = colliders.FirstOrDefault();
                        if ( UpperCheck(col.transform) )           //장애물인데 올라갈 수 없게 뭔가 있다면
                        {
                            moveOn = false;
                            yield break;
                        }
                        StartCoroutine(ClimbRoutine(col.transform));
                    }
                    else                            //벽인 경우
                    {
                        Debug.Log("앞에 뭔가가 있긴있는데 장애물이 아니었음");
                        if ( colliders.Any(collider => wall.Contain(collider.gameObject.layer)) )
                        {
                            Debug.Log("벽이 있습니다.");
                            moveOn = false;
                            yield break;
                        }
                        else
                        {
                            Debug.Log("벽이 아님 충격");
                            foreach ( Collider collider in colliders )
                            {
                                Debug.Log(collider.gameObject.name);
                            }
                            moveOn = false;
                            yield break;
                        }
                    }
                }
                else
                {
                        Debug.Log("What");

                }
               /* if ( FrontCheck(targetPos).Contain(LayerMask.GetMask("Wall")) )         // 벽을 검출하기 위한 레이캐스트
                {
                    Debug.Log("앞에 벽");
                    MoveExit();
                    yield break;
                }*/
               /* else
                {

                    Collider [] colliders = Physics.OverlapSphere(onBoxPos + moveDirValue * 2f, 0.3f, checkLayer);         // 박스에 올라가 있는 상태에서 앞에 무엇이 있는지 확인하기 위한 오버랩
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
                                            MoveExit();
                                            yield break;
                                        }
                                        else
                                        {
                                            Debug.Log($"아래에 벽이 있다 {collider.gameObject.name}");
                                            MoveExit();
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
                                        MoveExit();
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
                        Collider [] targetPositionObstacle = Physics.OverlapSphere(onBox.collider.transform.position + moveDirValue, 0.3f, obstacleLayer);

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
                            if(targetPositionObstacle.Length > 0)
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

                */
            }
        }

    }
}
