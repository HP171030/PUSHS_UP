using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections.Generic;
using Unity.VisualScripting;
//using static UnityEditor.Experimental.GraphView.GraphView;
//using UnityEditor.PackageManager;
using UnityEngine.InputSystem.HID;
public class YHP_PlayerController : MonoBehaviour
{
    public Vector3 moveDir = Vector3.zero;
    public bool moveOn;
    public bool inputKey = true;
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
    [SerializeField] LayerMask mirror;
    [SerializeField] LayerMask crystal;
    [SerializeField] public bool onIce = false;
    [SerializeField] LayerMask NonePlayer;
    [SerializeField] public CameraSwitch cameraSwitch;
    [SerializeField] Holder holder;
    [SerializeField] Mirror1 mirror1;
    [SerializeField] GameObject mirror1Image;
    [SerializeField] Mirror2 mirror2;

    [Header("Sound")]
    [SerializeField] AudioClip WalkSound;
    [SerializeField] AudioClip cubePullSound;
    [SerializeField] AudioClip cubePushSound;
    [SerializeField] AudioClip mirrorGrab;
    [SerializeField] AudioClip mirrorDown;




    Transform obstacle;
    RaycastHit otherObs;
    Vector3 debugVec;
    RaycastHit grabHit;

    [Header("Property")]
    [SerializeField] GameObject holderPoint;
    [SerializeField] float distanceFromPlayer;
    [SerializeField] float wallMirrorOffset;
    [SerializeField] float unHoldwallMirrorOffset;


    public bool mirrorHolding = false; // 거울이 있는지 확인하는 플래그
    public bool MirrorHolding { get { return mirrorHolding; } }

    public bool wallMirrorBumpChecker;
    public bool WallMirrorBumpChecker { get { return wallMirrorBumpChecker; } }

    //public bool alreadyMap2Obstacle;
    //public bool AlreadyMap2Obstacle
    //{
    //    get { return alreadyMap2Obstacle; }
    //    set { alreadyMap2Obstacle = value; }
    //}

    public int mirror1WallAttachedDir;
    public int Mirror1WallAttachedDir { get { return mirror1WallAttachedDir; } }

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
    private void OnMove(InputValue value)
    {
        if (cameraSwitch.IsPlayer1Active && !onIce && inputKey)
        {
            Vector2 input = value.Get<Vector2>();
            moveDir = new Vector3(input.x, 0, input.y);


            if (input.x > 0) // 오른쪽으로 들어옴
            {
                mirror1WallAttachedDir = 1;
            }
            else if (input.x < 0) // 왼쪽으로 들어옴
            {
                mirror1WallAttachedDir = 2;
            }
            else if (input.y > 0) // 위로 들어옴
            {
                mirror1WallAttachedDir = 3;
            }
            else if (input.y < 0) // 아래로 들어옴
            {
                mirror1WallAttachedDir = 4;
            }
        }
    }
    public void OnPull(InputValue value)
    {
        if (value.isPressed && cameraSwitch.IsPlayer1Active)
        {
            grabOn = true;
            Manager.ui.ChangePushPull(grabOn);
            animator.SetTrigger("PullStart");
            if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, out grabHit, 1.5f))
            {
                if (obstacleLayer.Contain(grabHit.collider.gameObject.layer))
                {
                    animator.SetBool("Pull", true);

                }
            }
        }
        else
        {
            grabOn = false;
            Manager.ui.ChangePushPull(grabOn);
            if (!pullOn)
                moveOn = false;

        }
    }
    public void OffPull()
    {
        if (!grabOn && !moveOn)
        {
            animator.SetBool("Pull", false);
        }
    }
    public void MoveFunc()
    {
        if (!moveOn && !grabOn && !onIce)
        {
            animator.SetFloat("MoveSpeed", moveDir.magnitude);
            if (Mathf.Abs(moveDir.x) != 0 && Mathf.Abs(moveDir.z) != 0)
            {
                return;
            }
            else if (!moveOn && moveDir.magnitude > 0)
            {
                moveOn = true;
                transform.forward = moveDir;
                StartCoroutine(MoveRoutine(moveDir));
            }
        }
        else if (!moveOn && grabOn && moveDir.magnitude > 0)
        {
            pullOn = true;
            if (grabHit.collider != null)
            {

                Vector3 grabDir = (grabHit.collider.gameObject.transform.position - transform.position).normalized;
                if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), -transform.forward, out RaycastHit hit, 1.5f))
                {
                    if (hit.collider != null)
                    {

                        moveOn = false;
                        return;
                    }
                }
                if (grabDir.x > 0.9f && moveDir.x < 0f)
                {
                    bool X = true;
                    StartCoroutine(PullRoutine(grabDir, X));

                }
                else if (grabDir.x < -0.9f && moveDir.x > 0f)
                {
                    bool X = true;
                    StartCoroutine(PullRoutine(grabDir, X));

                }
                else if (grabDir.z > 0.9f && moveDir.z < 0f)
                {
                    bool X = false;
                    StartCoroutine(PullRoutine(grabDir, X));

                }
                else if (grabDir.z < -0.9f && moveDir.z > 0f)
                {
                    bool X = false;
                    StartCoroutine(PullRoutine(grabDir, X));

                }
                else
                {

                    moveOn = false;
                    pullOn = false;
                }
            }
        }
    }
    private IEnumerator PullRoutine(Vector3 pullDir, bool X)
    {



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
        if (X)
        {
            Collider [] pullTarget = Physics.OverlapSphere(transform.position - transform.forward * 2, 0.5f, ground | wall|mirror);
            Tile tile;
            if (pullTarget.Length > 0)
            {
                foreach ( Collider col in pullTarget )
                {
                    if ( wall.Contain(col.gameObject.layer) | mirror.Contain(col.gameObject.layer) )
                    {

                        Debug.Log($"{col.name}");
                        moveOn = false;
                        pullOn = false;
                        yield break;
                    }
                    tile = col.gameObject.GetComponent<Tile>();
                    if (tile != null)
                    {
                        Debug.Log($" {tile.name}");
                        targetPos = tile.middlePoint.position;
                        debugVec = transform.position - transform.forward * 2;
                    }
                }
            }
        }
        else if (!X)
        {
            Collider [] pullTarget = Physics.OverlapSphere(transform.position - transform.forward * 2, 0.5f, ground | wall| mirror);
            Tile tile;
            if ( pullTarget.Length > 0 )
            {
                foreach ( Collider col in pullTarget )
                {
                    if ( wall.Contain(col.gameObject.layer) | mirror.Contain(col.gameObject.layer) )
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
                if (grabTile != null)
                    grabTargetPos = grabTile.middlePoint.position;

            }
        }
        List<RaycastHit> hitArray = new List<RaycastHit>(Physics.RaycastAll(grabHit.collider.transform.position, grabHit.collider.transform.up, 10f, obstacleLayer));
        hitArray.Add(grabHit);
        foreach (RaycastHit hit in hitArray)
        {
            hit.collider.gameObject.transform.SetParent(grabHit.collider.transform, true);
            hit.rigidbody.isKinematic = true;
        }
        float time = 0;
        Manager.sound.PlaySFX(cubePullSound);
        while (time < pullSpeed)
        {
            time += Time.deltaTime;
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / pullSpeed));
            grabHit.rigidbody.MovePosition(Vector3.Lerp(grabStartPos, grabTargetPos, time / pullSpeed));
            yield return null;
            if (time >= pullSpeed)
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
        if (debugVec != null)
            Gizmos.DrawWireSphere(debugVec, 0.5f);
        else
        {
            Debug.Log("isnoneDebugVec");
        }
    }


    private IEnumerator BumpTimer()
    {
        Debug.Log("1초 지나면 범프폴스");
        yield return new WaitForSeconds(2f);
        wallMirrorBumpChecker = false;
    }        //if (mirror1.wallChecker)
             //{
             //    mirror &= ~(1 << 31);
             //}
             //else
             //{
             //    mirror &= ~(30 << 1);
             //}




    private IEnumerator MoveRoutine(Vector3 moveDirValue)
    {
        Vector3 startPos = transform.position;
        Vector3 PreMoveDir = moveDir;
        LayerMask WandObslayer = obstacleLayer | wall | mirror;
        Vector3 targetPos = transform.position + moveDirValue * 2;
        debugVec = targetPos;

        // 1. 앞에 땅 충돌체가 없으면 이동 불가능
        Collider[] tiles = Physics.OverlapSphere(targetPos, 0.5f, ground);

        Tile isTile = null;
        if (tiles.Length == 0 )
        {
            Debug.Log("isNotGround");
            moveOn = false;
            yield break;
        }
        // 2. 앞에 땅 충돌체에 타일 컴포넌트가 없으면 이동 불가능
        foreach(Collider tile in tiles)
        {
           // Debug.Log($"the name is {tile}");
            isTile = tile.GetComponent<Tile>();
            if ( isTile == null )
            {
                Debug.Log("isnotTile");
                moveOn = false;
                yield return null;
            }
            else
            {
                // 3. 앞에 장애물을 확인
                Collider [] isBlank = Physics.OverlapSphere(transform.position + transform.forward + new Vector3(0, 1.5f, 0), 0.5f, WandObslayer);

                // 3-1. 앞이 빈공간이라면
                if ( isBlank.Length == 0 )
                {

                    targetPos = isTile.middlePoint.position;
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
                    // 3-2. 장애물이나 벽이 있는 경우
                    foreach ( Collider isCollider in isBlank )
                    {
                        //if ((mirror1.WallChecker && mirror2.obstacleChecker && mirror1.WallChecker))
                        //{
                        //    Debug.Log("장애물 못밀어");
                        //    yield return null;
                        //}

            Debug.Log($"{isCollider.name}이 앞에 있다");
            if (obstacleLayer.Contain(isCollider.gameObject.layer) && !mirrorHolding)
            {
                    moveSpeed = 2;

                    Debug.Log("장애물 밀기");
                Debug.Log(isCollider.name);

                Vector3 obsStartPos = isCollider.gameObject.transform.position;
                Vector3 obsTargetPos = isCollider.gameObject.transform.position + moveDirValue * 2f;
                Collider[] colliders = Physics.OverlapSphere(obsTargetPos, 0.5f, ground);
                if (colliders.Length > 0)
                {
                    foreach (Collider collider in colliders)
                    {
                        Tile obsTile = collider.GetComponent<Tile>();
                        if (obsTile != null)
                        {
                            Debug.Log(obsTile.name);
                            obsTargetPos = obsTile.middlePoint.position;
                        }
                    }
                }
                float time = 0;
                LayerMask layer = obstacleLayer | wall | crystal;
                if (Physics.BoxCast(isCollider.gameObject.transform.position, new Vector3(0.5f, 0.5f, 0.5f), moveDirValue, out RaycastHit hitInfo, Quaternion.identity, 0.7f, layer) && !MirrorHolding)
                {
                    Debug.Log($" {hitInfo.collider.gameObject.name}");
                    animator.SetBool("Push", false);
                    moveOn = false;
                        //    hit.rigidbody.isKinematic = false;

                    if (moveDir.magnitude < 1 || PreMoveDir != moveDir)
                    {
                        animator.SetBool("Push", false);
                    }
                      wallMirrorBumpChecker = true;
                      StartCoroutine(BumpTimer());
                }
                else
                {
                    List<RaycastHit> pushHitArray = new List<RaycastHit>(Physics.RaycastAll(isCollider.transform.position, isCollider.transform.up, 10f, obstacleLayer));
                    foreach (RaycastHit hits in pushHitArray)
                    {
                        hits.collider.gameObject.transform.SetParent(isCollider.transform, true);
                        hits.rigidbody.isKinematic = true;
                        Debug.Log(hits.collider.gameObject.name);
                    }
                                Manager.sound.PlaySFX(cubePushSound);
                                while (time < 2)
                    {
                        if (Physics.Raycast(isCollider.gameObject.transform.position + new Vector3(0, 0.5f, 0), moveDirValue, out RaycastHit notThis, 1f, layer) && notThis.collider.gameObject != isCollider.gameObject)
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
                    if (moveDir.magnitude < 1 || PreMoveDir != moveDir)
                    {
                        animator.SetBool("Push", false);
                    }
                    foreach (RaycastHit hits in pushHitArray)
                    {
                        hits.collider.gameObject.transform.SetParent(null, true);
                        

                        hits.rigidbody.isKinematic = false;
                            moveSpeed = 4;
                    }
                }
                    moveSpeed = 4;
                }
            else if (wall.Contain(isCollider.gameObject.layer))
            {
                Debug.Log($"wall name is {isCollider.gameObject.name}");
                moveOn = false;
                yield return null;
            }
            else if (holder.FrontMirrorLader() && !mirrorHolding && !mirror1.WallChecker)
            {
                Debug.Log("거울을 밟고 갈 수 없습니다.FrontMirrorLader");
                moveOn = false;
                yield return null;
            }
            else if (holder.FrontObstacleLader() && mirrorHolding)
            {
                Debug.Log("거울을 들고 장애물을 밀 수 없습니다. FrontObstacleLader");
                moveOn = false;
                yield return null;
            }
            else if (wall.Contain(isCollider.gameObject.layer))
            {
                Debug.Log($"wall name is {isCollider.gameObject.name}");
                moveOn = false;
                yield return null;
            }

                        else if ( mirror.Contain(isCollider.gameObject.layer) && !mirrorHolding && !mirror1.WallChecker )
                        {
                            Debug.Log(isCollider.name);
                            Debug.Log($"거울이 앞에 있어서 이동할 수 없습니다.");
                            moveOn = false;
                            yield return null;
                        }
                        else if ( holder.FrontMirrorLader() && !mirrorHolding && !mirror1.WallChecker )
                        {
                            Debug.Log($"거울이 앞에 있어서 이동할 수 없습니다.");
                            moveOn = false;
                            yield return null;
                        }
                        else if ( holder.WallLader() && mirrorHolding )
                        {
                            Debug.Log($"거울을 들고 벽 통과 못함");
                            moveOn = false;
                            yield return null;
                        }
                        else if ( holder.MoveDisableCheckerLader() && mirrorHolding )
                        {
                            Debug.Log($"거울을 들고 움직일 수 없는 장애물 통과 못함");
                            moveOn = false;
                            yield return null;
                        }
                        else if ( holder.FrontMirrorLader() && holder.FrontObstacleLader() && mirror2.ObstacleChecker )
                        {
                            Debug.Log($"이미 맵2에 장애물이 있어서 못밀어넣음");
                            moveOn = false;
                            yield return null;
                        }
                        else if ( holder.FrontMirrorLader() && holder.FrontObstacleLader() && mirror2.ObstacleChecker )
                        {
                            Debug.Log($"이미 맵2에 장애물이 있어서 못밀어넣음");
                            moveOn = false;
                            yield return null;
                        }
                        //else if (holder.MoveDisableCheckerLader())
                        //{
                        //    Debug.Log($"움직일 수 없는 장애물이 있어 못지나감");
                        //    moveOn = false;
                        //    yield return null;
                        //}

                        else
                        {
                            float time = 0;
                            bool hitWall = Physics.OverlapSphere(transform.position + transform.forward + new Vector3(0, 0.5f, 0), 0.5f, wall|mirror).Length > 0;
                            Manager.sound.PlaySFX(WalkSound);
                            while ( time < 1 )
                            {

                                if ( hitWall &&!mirrorHolding)
                                {
                                    Debug.Log("Hit wall");
                                    transform.position = startPos;
                                    moveOn = false;
                                    yield break;
                                }

                                time += Time.deltaTime * moveSpeed;
                                rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));

                                yield return null;
                            }

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

        
    }
           
        }



    private void OnHold(InputValue value)
    {
        if( cameraSwitch.IsPlayer1Active )
        {
        if (!mirrorHolding) // 거울이 확인되지 않았을 때
        {
            Hold();
        }
        //거울 놓기
        else if (mirrorHolding && !holder.FrontObstacleLader() && !moveOn)
        {
            UnHold();
        }

        }
    }

    private void Hold()
    {
        Debug.Log("잡기 시도");
        GameObject mirrorObject = holder.GrabMirror();

        if (mirrorObject != null && !holder.FrontObstacleLader())
        {
            Debug.Log("잡았음");
            Manager.sound.PlaySFX(mirrorGrab);
            // 거울 오브젝트를 홀더 포인트의 자식으로 설정합니다.
            mirrorObject.transform.parent = holderPoint.transform;
            // 거울의 로컬 포지션을 (0, 0, 0)으로 설정하여 정확한 위치에 배치합니다.
            mirrorObject.transform.localPosition = Vector3.zero;
            mirrorObject.transform.localRotation = Quaternion.Euler(90, 0, 0);

            mirrorHolding = true;

            Debug.Log("거울 가까이 둬기");
            mirror1Image.transform.localPosition = Vector3.zero;
            Vector3 newPosition = mirror1Image.transform.localPosition;
            newPosition.y += wallMirrorOffset;
            mirror1Image.transform.localPosition = newPosition;
        }
        else
        {
            Debug.Log("못잡았음");
            mirrorHolding = false;
        }
    }

    private void UnHold()
    {

        GameObject mirrorObject = holder.GetMirror();


        if (mirrorObject != null)
        {


            if (holder.WallLader() && !mirror1.MoveDisableChecker) //벽에 거울을 놓는다면
            {
                Debug.Log("벽에 거울 놓기");
                //holdChecker = false;
                mirrorObject.transform.parent = null; // 부모 설정을 해제하여 자식에서 빼냅니다.


                switch (mirror1WallAttachedDir)
                {
                    case 1: // 오른쪽으로 들어옴
                        //mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                        mirrorObject.transform.forward = Vector3.right;

                        //newmirror1ImegePosition.y -= wallMirrorOffset;
                        Debug.Log("오른쪽으로 둠");
                        break;
                    case 2: // 왼쪽으로 들어옴
                        //mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, -90);
                        mirrorObject.transform.forward = Vector3.left;
                        //newmirror1ImegePosition.y += wallMirrorOffset;
                        Debug.Log("왼쪽으로 둠");

                        break;
                    case 3: // 위로 들어옴
                        //mirrorObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
                        mirrorObject.transform.forward = Vector3.up;
                        //newmirror1ImegePosition.x -= wallMirrorOffset;
                        Debug.Log("위로 둠");

                        break;
                    case 4: // 아래로 들어옴
                        //mirrorObject.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        mirrorObject.transform.forward = Vector3.down;
                        //newmirror1ImegePosition.x += wallMirrorOffset;
                        Debug.Log("아래로 둠");

                        break;
                    default: // 그 외의 경우
                        Debug.LogError("거울 설치방향 예외!");
                        break;
                }
                Manager.sound.PlaySFX(mirrorDown);
                mirrorHolding = false;
                Vector3 forwardDirection = mirrorObject.transform.forward;
                mirrorObject.transform.forward = forwardDirection;
                mirror1.SetForwardDirection(forwardDirection); // forwardDirection 값을 Mirror1 스크립트에 전달합니다.


                Debug.Log("벽에 가까이 둬기");
                // 거울을 벽에 놓을 때 mirror1Image를 플립합니다.
                if (mirror1WallAttachedDir == 1 || mirror1WallAttachedDir == 2) // 오른쪽 , 왼쪽
                {
                    mirror1Image.transform.localPosition = Vector3.zero;
                    Vector3 newPosition = mirror1Image.transform.localPosition;
                    newPosition.y -= unHoldwallMirrorOffset;
                    mirror1Image.transform.localPosition = newPosition;


                    //Vector3 newPosition1 = mirror1Image.transform.localPosition;
                    //newPosition1.y -= unHoldwallMirrorOffset;
                    //mirror1.transform.localPosition = newPosition1;
                }
                //else if (mirror1WallAttachedDir == 2) // 왼쪽
                //{
                //    Vector3 newPosition = mirror1Image.transform.localPosition;
                //    newPosition.y -= unHoldwallMirrorOffset;
                //    mirror1Image.transform.localPosition = newPosition;

                //    Vector3 newPosition1 = mirror1Image.transform.localPosition;
                //    newPosition1.y -= unHoldwallMirrorOffset;
                //    mirror1.transform.localPosition = newPosition1;
                //}

                // 결과 출력
                //Debug.Log($"mirrorObject.transform.forward: {forwardDirection}");
                //Debug.Log("거울 벽에 놓기");
                //Debug.Log($"mirrorObject.transform.forward{mirrorObject.transform.forward}");
            }


            else if(!holder.MoveDisableCheckerLader()) // 바닥에 거울을 놓는다면
            {
                // 거울을 땅에 놓을 때 앞에 장애물이 있는지 확인하고, 있으면 거울을 놓지 않습니다.
                Debug.Log("바닥에 거울 놓기");
                if (holder.FrontObstacleLader())
                {
                    Debug.Log("장애물이 앞에 있어 거울을 놓을 수 없습니다.");
                    return;
                }
                //if (holder.MoveDisableCheckerLader())
                //{
                //    mirror1.wallChecker = false;
                //    Debug.Log("움직일 수 없는 장애물이 앞에 있어 거울을 놓을 수 없습니다.");
                //    return;
                //}



                // 거울을 바닥에 놓습니다.
                Manager.sound.PlaySFX(mirrorDown);
                mirrorObject.transform.parent = null; // 부모 설정을 해제하여 자식에서 빼냅니다.
                mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Vector3 mirrorPosition = transform.position + transform.forward * distanceFromPlayer;
                mirrorObject.transform.position = mirrorPosition;
                mirrorHolding = false;
                mirror1Image.transform.localPosition = Vector3.zero;
                mirror1.wallChecker = false;



                Debug.Log("거울 바닥에 놓기");
            }

        }
        else
        {
            Debug.LogWarning("잡은 오브젝트가 유효하지 않습니다.");
        }
    }
}



