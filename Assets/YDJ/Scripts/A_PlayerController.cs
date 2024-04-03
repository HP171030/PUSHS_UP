using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class A_PlayerController : MonoBehaviour
{
    private Vector3 moveDir;

    bool moveOn;
    bool grabOn;
    [Header("Player")]
    [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;

    [Header("Configs")]
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rb;
    [SerializeField] CameraSwitch cameraSwitch;
    [SerializeField] Holder holder;

    [Header("Property")]
    [SerializeField] GameObject holderPoint;

    private bool wallMirrorChecker;
    public bool WallMirrorChecker { get { return wallMirrorChecker; } }

    Transform obstacle;
    RaycastHit otherObs;

    RaycastHit grabHit;

    bool isVerticalWall = true;

    [SerializeField] float distanceFromPlayer;

    private void FixedUpdate()
    {
        OffPull();
        MoveFunc();

    }


    private SphereCollider sphereCollider; // 필드 선언


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        sphereCollider = holder.GetComponent<SphereCollider>(); // Start 메서드에서 초기화


    }
    private void OnMove(InputValue value)
    {

        if (cameraSwitch.IsPlayer1Active)
        {
            Vector2 input = value.Get<Vector2>();
            moveDir = new Vector3(input.x, 0, input.y);

            //거울 설치 방향 조정
            if(input.x == 1)
            {
                isVerticalWall = false;
            }
            else if(input.y == 1)
            {
                isVerticalWall = true;
            }

        }

    }

    public void OnPull(InputValue value)
    {
        if (value.isPressed)
        {
            grabOn = true;
            Debug.Log("잡기");
            animator.SetTrigger("PullStart");

            if (Physics.Raycast(transform.position, transform.forward, out grabHit, 1f))
            {
                if (grabHit.collider.gameObject.CompareTag("Obstacle"))
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
        if (!moveOn && !grabOn)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        if (obstacle != null && otherObs.point != null)
            Gizmos.DrawLine(obstacle.position, otherObs.point);
    }
    private IEnumerator PullRoutine(Vector3 pullDir, bool X)
    {
        Vector3 targetPos = pullDir;
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
        Vector3 startPos = transform.position;
        Vector3 grabStartPos = grabHit.collider.transform.position;
        float time = 0;
        float targetTime = 2;
        while (time < 2)
        {

            time += Time.deltaTime;
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / targetTime));
            grabHit.rigidbody.MovePosition(Vector3.Lerp(grabStartPos, grabTargetPos, time / targetTime));
            if (time >= 2)
                moveOn = false;


            yield return null;
        }



        Debug.Log("무브온펄스");
    }
    
    private IEnumerator MoveRoutine(Vector3 moveDirValue)
    {
        Vector3 targetPos = transform.position + moveDirValue * moveDistance;
        Vector3 startPos = transform.position;
        RaycastHit hit;
        Vector3 PreMoveDir = moveDir;
        if (Physics.Raycast(transform.position, moveDirValue, out hit, 1f))
        {
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.CompareTag("Obstacle"))
            {
                obstacle = hit.transform;
                Vector3 obsStartPos = obstacle.position;
                Vector3 obsTargetPos = obstacle.position + moveDirValue * moveDistance;
                Rigidbody obsRb = obstacle.GetComponent<Rigidbody>();
                float time = 0;

                if (obsRb != null) // Rigidbody가 null인지 확인합니다.
                {
                    if (Physics.Raycast(obstacle.position, moveDirValue, out otherObs, 1f))
                    {
                        if (otherObs.collider.CompareTag("Obstacle"))
                        {
                            moveOn = false;
                            if (moveDir.magnitude < 1 || PreMoveDir != moveDir)
                            {
                                animator.SetBool("Push", false);
                            }
                        }
                        else
                        {
                            while (time < 2)
                            {
                                animator.SetBool("Push", true);
                                time += Time.deltaTime * moveSpeed;
                                rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / 2));
                                if (obsRb != null) // 파괴되지 않았는지 다시 확인합니다.
                                {
                                    obsRb.MovePosition(Vector3.Lerp(obsStartPos, obsTargetPos, time / 2));
                                }
                                yield return null;
                            }
                            moveOn = false;
                            if (moveDir.magnitude < 1 || PreMoveDir != moveDir)
                            {
                                animator.SetBool("Push", false);
                            }
                            Debug.Log("장애물도 같이 무브");
                        }
                    }
                    else
                    {
                        while (time < 2)
                        {
                            animator.SetBool("Push", true);
                            time += Time.deltaTime * moveSpeed;
                            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / 2));
                            if (obsRb != null) // 파괴되지 않았는지 다시 확인합니다.
                            {
                                obsRb.MovePosition(Vector3.Lerp(obsStartPos, obsTargetPos, time / 2));
                            }
                            yield return null;
                        }
                        moveOn = false;
                        if (moveDir.magnitude < 1 || PreMoveDir != moveDir)
                        {
                            animator.SetBool("Push", false);
                        }
                        Debug.Log("장애물도 같이 무브");
                    }
                }
                else
                {
                    Debug.Log("Rigidbody가 파괴되었습니다.");
                }
            }
        }
        else
        {
            float time = 0;
            while (time < 1)
            {
                time += Time.deltaTime * moveSpeed;
                rb.MovePosition(Vector3.Lerp(startPos, targetPos, time));
                yield return null;
            }
            yield return null;
            moveOn = false;
            if (moveDir.magnitude < 1 || PreMoveDir != moveDir)
            {
                animator.SetBool("Push", false);
            }
            Debug.Log("무브");
        }
    }


    //private void OnHold(InputValue value)
    //{

    //    if (sphereCollider != null)
    //    {
    //        sphereCollider.enabled = true;

    //        if (holder.AbleHold)
    //        {
    //            Hold();
    //            sphereCollider.enabled = false;
    //        }





    //    }

    //}

    //private void Hold()
    //{
    //    Debug.Log("잡음");
    //    sphereCollider.enabled = false;
    //}


    public bool mirrorHolding = false; // 거울이 있는지 확인하는 플래그

    private void OnHold(InputValue value)
    {
        if (!mirrorHolding) // 거울이 확인되지 않았을 때
        {
            Hold();
        }
        //거울 놓기
        else if(mirrorHolding)
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
            wallMirrorChecker = false;
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
        mirrorHolding = false;

        // 거울을 홀더 포인트에서 떼어냅니다.
        GameObject mirrorObject = holder.GetMirror();


        if (mirrorObject != null )
        {
            mirrorObject.transform.parent = null; // 부모 설정을 해제하여 자식에서 빼냅니다.
            if (holder.WallLader()) //벽에 거울을 놓는다면
            {
                wallMirrorChecker = true;


                if (isVerticalWall) // 위 아래 벽에 거울을 놓는다면
                {
                    // 위 아래 벽에 맞게 회전 각도를 설정합니다.
                    mirrorObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    Vector3 mirrorPosition = holderPoint.transform.position;
                    // 거울의 위치를 홀더 포인트의 위치로 설정합니다.
                    mirrorObject.transform.position = mirrorPosition;
                }
                else // 왼쪽 오른쪽 벽에 거울을 놓는다면
                {
                    // 왼쪽 오른쪽 벽에 맞게 회전 각도를 설정합니다.
                    mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    Vector3 mirrorPosition = holderPoint.transform.position;
                    // 거울의 위치를 홀더 포인트의 위치로 설정합니다.
                }


                ////왼쪽 오른쪽 벽
                //mirrorObject.transform.localRotation = Quaternion.Euler( 0, 0, 90);
                //Vector3 mirrorPosition = holderPoint.transform.position;

                ////앞뒤 벽
                //if ()
                //{
                //    mirrorObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
                //    Vector3 mirrorPosition = holderPoint.transform.position;
                //}
                //Vector3 mirrorPosition = transform.position + transform.forward * distanceFromPlayer; // 플레이어 앞에 일정한 거리만큼 떨어진 위치
                //mirrorObject.transform.position = mirrorPosition;
            }


            else // 바닥에 거울을 놓는다면
            {
                mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Vector3 mirrorPosition = transform.position + transform.forward * distanceFromPlayer; // 플레이어 앞에 일정한 거리만큼 떨어진 위치
                mirrorObject.transform.position = mirrorPosition;
            }
        }
        else
        {
            Debug.LogWarning("잡은 오브젝트가 유효하지 않습니다.");
        }
    }
}