using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine.WSA;
using Unity.VisualScripting;

public class YHP_PlayerController : MonoBehaviour
{
    public Vector3 moveDir;

    bool moveOn;
    bool grabOn;
    [Header("Player")]
    [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;

    [Header("Configs")]
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask wall;
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask mirror;
    [SerializeField] public bool onIce = false;
    [SerializeField] Holder holder;
    [SerializeField] Mirror1 mirror1;



    LayerMask layerMask;

    [Header("Property")]
    [SerializeField] GameObject holderPoint;
    [SerializeField] float distanceFromPlayer;

    [SerializeField] CameraSwitch cameraSwitch;

    public bool mirrorHolding = false; // 거울이 있는지 확인하는 플래그
    public bool MirrorHolding { get { return mirrorHolding; } }

    public bool wallMirrorBumpChecker;
    public bool WallMirrorBumpChecker { get { return wallMirrorBumpChecker; } }



    public float mirror1WallAttachedDir;






    Transform obstacle;
    RaycastHit otherObs;

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
        layerMask = ~(1 << LayerMask.NameToLayer("Ground"));

    }
    private void OnMove(InputValue value)
    {
        if (cameraSwitch.IsPlayer1Active && !onIce)
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
            Debug.Log("잡기");
            animator.SetTrigger("PullStart");

            if (Physics.Raycast(transform.position, transform.forward, out grabHit, 1f))
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
            Debug.Log("놓기");


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

                StartCoroutine(MoveRoutine(moveDir));

                transform.forward = moveDir;

            }

        }
        else if (!moveOn && grabOn && moveDir.magnitude > 0)
        {
            if (grabHit.collider != null)
            {
                Vector3 grabDir = (grabHit.collider.gameObject.transform.position - transform.position).normalized;
                if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, 1.5f))
                {
                    if (hit.collider != null)
                    {
                        Debug.Log("뒤에 벽잇음 ");
                        moveOn = false;
                        return;
                    }
                }
                moveOn = true;
                if (grabDir.x > 0.9f && moveDir.x < 0f)
                {

                    bool X = true;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}떙겨x");
                }
                else if (grabDir.x < -0.9f && moveDir.x > 0f)
                {
                    bool X = true;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}떙겨x");
                }
                else if (grabDir.z > 0.9f && moveDir.z < 0f)
                {
                    bool X = false;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}떙겨z");
                }
                else if (grabDir.z < -0.9f && moveDir.z > 0f)
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

    /*  private void OnDrawGizmos()
      {
          Gizmos.color = Color.red;
          Gizmos.DrawLine(obstacle.position,obstacle.position + moveDir);
      }*/
    private IEnumerator PullRoutine(Vector3 pullDir, bool X)
    {

        if (Physics.Raycast(transform.position, -transform.forward, 1f))
        {
            yield break;
        }
        Vector3 startPos = transform.position;
        Vector3 targetPos = pullDir;
        Vector3 grabStartPos = grabHit.collider.transform.position;
        Vector3 grabTargetPos = pullDir;
        if (X)
        {
            targetPos = transform.position + new Vector3(-pullDir.x, 0, 0) * moveDistance;
            grabTargetPos = grabHit.collider.transform.position + new Vector3(-pullDir.x, 0, 0) * moveDistance;
        }
        else if (!X)
        {
            targetPos = transform.position + new Vector3(0, 0, -pullDir.z) * moveDistance;
            grabTargetPos = grabHit.collider.transform.position + new Vector3(0, 0, -pullDir.z) * moveDistance;
        }


        List<RaycastHit> hitArray = new List<RaycastHit>(Physics.RaycastAll(grabHit.collider.transform.position, grabHit.collider.transform.up, 10f));

        hitArray.Add(grabHit);

        foreach (RaycastHit hit in hitArray)
        {
            hit.collider.gameObject.transform.SetParent(grabHit.collider.transform, true);
            hit.rigidbody.isKinematic = true;
        }


        float time = 0;
        float targetTime = 2;
        while (time < 2)
        {

            time += Time.deltaTime;
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / targetTime));
            grabHit.rigidbody.MovePosition(Vector3.Lerp(grabStartPos, grabTargetPos, time / targetTime));
            yield return null;


            if (time >= 2)
            {
                foreach (RaycastHit hit in hitArray)
                {
                    hit.collider.gameObject.transform.SetParent(null, true);
                    hit.rigidbody.isKinematic = false;
                }
                Manager.game.StepAction++;
                moveOn = false;
            }




        }

    }
    private IEnumerator MoveRoutine(Vector3 moveDirValue)
    {

        Vector3 targetPos = transform.position + moveDirValue * moveDistance;
        Vector3 startPos = transform.position;
        RaycastHit hit;
        Vector3 PreMoveDir = moveDir;
        LayerMask layer = obstacleLayer | wall;


        //비트 마스크로 미러홀딩이면 미러 할당 노할당 정하기
        if (mirrorHolding)
        {
            mirror = 0;
        }
        else
        {
            mirror = 1 << 11;
        }

        //Debug.DrawLine(transform.position + new Vector3(0, 0.3f, 0), transform.position + new Vector3(0, 0.3f, 0) + moveDirValue * 1.2f, Color.red, 1f);
        if ( Physics.Raycast(transform.position + new Vector3(0, 0.3f, 0), moveDirValue, out hit, 1.2f, layer | mirror))
        {


            if (obstacleLayer.Contain(hit.collider.gameObject.layer))
            {
                Debug.Log("asff");
                obstacle = hit.transform;
                Vector3 obsStartPos = obstacle.position;
                Vector3 obsTargetPos = obstacle.position + moveDirValue * moveDistance;

                float time = 0;


                if (Physics.BoxCast(obstacle.position, new Vector3(0.5f, 0.5f, 0.5f), moveDirValue, out RaycastHit hitInfo, Quaternion.identity, 1f, layer))
                {
                    //if (!mirror1.mirrorObstacleAttachedChecker)
                    {
                        Debug.Log(hitInfo.collider.gameObject.name);
                        moveOn = false;
                        if (moveDir.magnitude < 1 || PreMoveDir != moveDir)
                        {
                            animator.SetBool("Push", false);
                        }
                        wallMirrorBumpChecker = true;

                    }

                }
                else
                {
                    List<RaycastHit> pushHitArray = new List<RaycastHit>(Physics.RaycastAll(hit.collider.transform.position, hit.collider.transform.up, 10f, layer));
                    foreach (RaycastHit hits in pushHitArray)
                    {
                        hits.collider.gameObject.transform.SetParent(hit.collider.transform, true);
                        hits.rigidbody.isKinematic = true;
                        Debug.Log(hits.collider.gameObject.name);
                    }
                    while (time < 2)
                    {
                        if (Physics.Raycast(obstacle.position + new Vector3(0, 0.5f, 0), moveDirValue, out RaycastHit notThis, 1f, layer) && notThis.collider.gameObject != obstacle.gameObject)
                        {

                            Debug.Log("뒤에");
                            moveOn = false;
                            yield break;
                        }
                        if (mirrorHolding)
                        {
                            Debug.Log("거울 들고있어서 장애물 못밈");
                            moveOn = false;
                            yield break;
                        }
                        //Debug.Log("m");
                        animator.SetBool("Push", true);
                        time += Time.deltaTime * moveSpeed;
                        rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / 2));
                        hit.rigidbody.MovePosition(Vector3.Lerp(obsStartPos, obsTargetPos, time / 2));
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
                    }

                    Debug.Log("장애물도 같이 무브");
                }

            }
            else if (wall.Contain(hit.collider.gameObject.layer))
            {
                Debug.Log("앞에 벽");
                moveOn = false;
                yield return null;

            }

            else if (mirror.Contain(hit.collider.gameObject.layer))
            {
                {
                    if (mirror1.wallChecker)
                    {
                        float time = 0;
                        while (time < 1)
                        {

                            time += Time.deltaTime * moveSpeed;
                            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));

                            yield return null;
                        }
                        yield return null;
                        Manager.game.StepAction++;
                        moveOn = false;
                        if (moveDir.magnitude < 1 || PreMoveDir != moveDir)
                        {
                            animator.SetBool("Push", false);
                        }
                        Debug.Log("벽거울이라 지나갈 수 있음");
                    }
                    else
                    {
                        Debug.Log("앞에 거울");
                        moveOn = false;
                        yield return null;
                    }
                }

            }
        }
        else // 무브
        {
            float time = 0;
            while (time < 1)
            {

                time += Time.deltaTime * moveSpeed;
                rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
               
                yield return null;
            }
            yield return null;
            Manager.game.StepAction++;
            moveOn = false;
            if (moveDir.magnitude < 1 || PreMoveDir != moveDir)
            {
                animator.SetBool("Push", false);
            }
        }

    }




    private void OnHold(InputValue value)
    {
        if (!mirrorHolding) // 거울이 확인되지 않았을 때
        {
            Hold();
        }
        //거울 놓기
        else if (mirrorHolding && !holder.FrontObstacleLader())
        {
            UnHold();
        }
    }

    private void Hold()
    {
        Debug.Log("잡기 시도");
        GameObject mirrorObject = holder.GrabMirror();

        if (mirrorObject != null)
        {
            Debug.Log("잡았음");
            // 거울 오브젝트를 홀더 포인트의 자식으로 설정합니다.
            mirrorObject.transform.parent = holderPoint.transform;
            // 거울의 로컬 포지션을 (0, 0, 0)으로 설정하여 정확한 위치에 배치합니다.
            mirrorObject.transform.localPosition = Vector3.zero;
            mirrorObject.transform.localRotation = Quaternion.Euler(90, 0, 0);

            mirrorHolding = true;
        }
        else
        {
            Debug.Log("못잡았음");
            mirrorHolding = false;
        }
    }

    private void UnHold()
    {
        Debug.Log("거울 놓기");
        GameObject mirrorObject = holder.GetMirror();


        if (mirrorObject != null)
        {
            mirrorHolding = false;

            if (holder.WallLader()) //벽에 거울을 놓는다면
            {
                //holdChecker = false;
                mirrorObject.transform.parent = null; // 부모 설정을 해제하여 자식에서 빼냅니다.

                switch (mirror1WallAttachedDir)
                {
                    case 1: // 오른쪽으로 들어옴
                        //mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                        mirrorObject.transform.forward = Vector3.right;
                        break;
                    case 2: // 왼쪽으로 들어옴
                        //mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, -90);
                        mirrorObject.transform.forward = Vector3.left;

                        break;
                    case 3: // 위로 들어옴
                        //mirrorObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
                        mirrorObject.transform.forward = Vector3.up;

                        break;
                    case 4: // 아래로 들어옴
                        //mirrorObject.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        mirrorObject.transform.forward = Vector3.down;

                        break;
                    default: // 그 외의 경우
                        Debug.LogError("거울 설치방향 예외!");
                        break;
                }


                Vector3 forwardDirection = mirrorObject.transform.forward;
                mirrorObject.transform.forward = forwardDirection;
                mirror1.SetForwardDirection(forwardDirection); // forwardDirection 값을 Mirror1 스크립트에 전달합니다.

                // 결과 출력
                Debug.Log($"mirrorObject.transform.forward: {forwardDirection}");

                //Debug.Log($"mirrorObject.transform.forward{mirrorObject.transform.forward}");
            }


            else // 바닥에 거울을 놓는다면
            {
                // 거울을 땅에 놓을 때 앞에 장애물이 있는지 확인하고, 있으면 거울을 놓지 않습니다.

                if (mirror1.ObstacleChecker)
                {
                    Debug.Log("장애물이 앞에 있어 거울을 놓을 수 없습니다.");
                    return;
                }

                // 거울을 바닥에 놓습니다.
                mirrorObject.transform.parent = null; // 부모 설정을 해제하여 자식에서 빼냅니다.
                mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Vector3 mirrorPosition = transform.position + transform.forward * distanceFromPlayer;
                mirrorObject.transform.position = mirrorPosition;
                mirror1.wallChecker = false;
            }

        }
        else
        {
            Debug.LogWarning("잡은 오브젝트가 유효하지 않습니다.");
        }
    }
}



