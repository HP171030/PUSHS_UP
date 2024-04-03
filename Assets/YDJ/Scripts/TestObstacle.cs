using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class TestObstacle : MonoBehaviour
{
    private Vector3 moveDir;

    bool moveOn;
    bool grabOn;
    [Header("Player")]
    [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;

    [Header("Configs")]
    [SerializeField] Rigidbody rb;

    Transform obstacle;
    RaycastHit otherObs;

    RaycastHit grabHit;

    [SerializeField] float distanceFromPlayer;

    private void FixedUpdate()
    {
        MoveFunc();

    }


    private SphereCollider sphereCollider; // 필드 선언


    private void Start()
    {
        rb = GetComponent<Rigidbody>();


    }
    private void OnMove(InputValue value)
    {


        Vector2 input = value.Get<Vector2>();
        moveDir = new Vector3(input.x, 0, input.y);



    }

    public void OnPull(InputValue value)
    {
        if (value.isPressed)
        {
            grabOn = true;
            Debug.Log("잡기");

            if (Physics.Raycast(transform.position, transform.forward, out grabHit, 1f))
            {
                if (grabHit.collider.gameObject.CompareTag("Obstacle"))
                {

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

    public void MoveFunc()
    {
        if (!moveOn && !grabOn)
        {

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
            //Debug.Log(hit.collider.gameObject.name);
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

                        }
                        else
                        {
                            while (time < 2)
                            {
                                time += Time.deltaTime * moveSpeed;
                                rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / 2));
                                if (obsRb != null) // 파괴되지 않았는지 다시 확인합니다.
                                {
                                    obsRb.MovePosition(Vector3.Lerp(obsStartPos, obsTargetPos, time / 2));
                                }
                                yield return null;
                            }
                            moveOn = false;

                            Debug.Log("장애물도 같이 무브");
                        }
                    }
                    else
                    {
                        while (time < 2)
                        {
                            time += Time.deltaTime * moveSpeed;
                            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / 2));
                            if (obsRb != null) // 파괴되지 않았는지 다시 확인합니다.
                            {
                                obsRb.MovePosition(Vector3.Lerp(obsStartPos, obsTargetPos, time / 2));
                            }
                            yield return null;
                        }
                        moveOn = false;


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

            Debug.Log("무브");
        }
    }
}

