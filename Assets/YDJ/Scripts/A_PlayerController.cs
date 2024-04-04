using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
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
    [SerializeField] Mirror1 mirror1;
    //[SerializeField] Obstacle obstacleScript;

    [Header("Property")]
    [SerializeField] GameObject holderPoint;

    //public bool holdChecker;
    public bool MirrorHolding { get { return mirrorHolding; } }
    public bool mirrorHolding = false; // �ſ��� �ִ��� Ȯ���ϴ� �÷���

    Transform obstacle;
    RaycastHit otherObs;

    RaycastHit grabHit;

    public float mirror1WallAttachedDir;

    [SerializeField] float distanceFromPlayer;

    private void FixedUpdate()
    {
        OffPull();
        MoveFunc();

    }


    private SphereCollider sphereCollider; // �ʵ� ����


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        sphereCollider = holder.GetComponent<SphereCollider>(); // Start �޼��忡�� �ʱ�ȭ


    }
    private void OnMove(InputValue value)
    {

        if (cameraSwitch.IsPlayer1Active)
        {
            Vector2 input = value.Get<Vector2>();
            moveDir = new Vector3(input.x, 0, input.y);

            //�ſ� ��ġ ���� ����
            //if(input.x == 1)
            //{
            //    isVerticalWall = false;
            //}
            //else if(input.y == 1)
            //{
            //    isVerticalWall = true;
            //}


            if (input.x  > 0 ) // ���������� ����
            {
                mirror1WallAttachedDir = 1;
            }
            else if (input.x < 0) // �������� ����
            {
                mirror1WallAttachedDir = 2;
            }
            else if (input.y > 0) // ���� ����
            {
                mirror1WallAttachedDir = 3;
            }
            else if (input.y < 0) // �Ʒ��� ����
            {
                mirror1WallAttachedDir = 4;
            }

        }

    }

    public void OnPull(InputValue value)
    {
        if (value.isPressed)
        {
            grabOn = true;
            Debug.Log("���");
            animator.SetTrigger("PullStart");

            if (Physics.Raycast(transform.position, transform.forward, out grabHit, 1f))
            {
                if (grabHit.collider.gameObject.CompareTag("Obstacle"))
                {
                    animator.SetBool("Pull", true);
                    Debug.Log("����");

                }

            }
        }
        else
        {
            grabOn = false;
            Debug.Log("����");


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
                    Debug.Log($"{grabDir}����x");
                }
                else if (grabDir.x < -0.9f && moveDir.x > 0f)
                {
                    bool X = true;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}����x");
                }
                else if (grabDir.z > 0.9f && moveDir.z < 0f)
                {
                    bool X = false;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}����z");
                }
                else if (grabDir.z < -0.9f && moveDir.z > 0f)
                {
                    bool X = false;
                    StartCoroutine(PullRoutine(grabDir, X));
                    Debug.Log($"{grabDir}����z");
                }
                else
                {
                    Debug.Log("������ ");
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



        Debug.Log("������޽�");
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

                if (obsRb != null) // Rigidbody�� null���� Ȯ���մϴ�.
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
                                if (obsRb != null) // �ı����� �ʾҴ��� �ٽ� Ȯ���մϴ�.
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
                            Debug.Log("��ֹ��� ���� ����");
                        }
                    }
                    else
                    {
                        while (time < 2)
                        {
                            animator.SetBool("Push", true);
                            time += Time.deltaTime * moveSpeed;
                            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / 2));
                            if (obsRb != null) // �ı����� �ʾҴ��� �ٽ� Ȯ���մϴ�.
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
                        Debug.Log("��ֹ��� ���� ����");
                    }
                }
                else
                {
                    Debug.Log("Rigidbody�� �ı��Ǿ����ϴ�.");
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
            Debug.Log("����");
        }
    }


    private void OnHold(InputValue value)
    {
        if (!mirrorHolding) // �ſ��� Ȯ�ε��� �ʾ��� ��
        {
            Hold();
        }
        //�ſ� ����
        else if(mirrorHolding)
        {
            UnHold();
        }
    }

    private void Hold()
    {
        Debug.Log("��� �õ�");
        GameObject mirrorObject = holder.GrabMirror();

        if (mirrorObject != null)
        {
            Debug.Log("�����");
            // �ſ� ������Ʈ�� Ȧ�� ����Ʈ�� �ڽ����� �����մϴ�.
            mirrorObject.transform.parent = holderPoint.transform;
            // �ſ��� ���� �������� (0, 0, 0)���� �����Ͽ� ��Ȯ�� ��ġ�� ��ġ�մϴ�.
            mirrorObject.transform.localPosition = Vector3.zero;
            mirrorObject.transform.localRotation = Quaternion.Euler(90, 0, 0);

            mirrorHolding = true;
        }
        else
        {
            Debug.Log("�������");
            mirrorHolding = false;
        }
    }

    private void UnHold()
    {
        Debug.Log("�ſ� ����");
        GameObject mirrorObject = holder.GetMirror();


        if (mirrorObject != null )
        {
            mirrorHolding = false;

            if (holder.WallLader()) //���� �ſ��� ���´ٸ�
            {
                //holdChecker = false;
                mirrorObject.transform.parent = null; // �θ� ������ �����Ͽ� �ڽĿ��� �����ϴ�.

                switch (mirror1WallAttachedDir)
                {
                    case 1: // ���������� ����
                        //mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                        mirrorObject.transform.forward = Vector3.right;
                        break;
                    case 2: // �������� ����
                        //mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, -90);
                        mirrorObject.transform.forward = Vector3.left;

                        break;
                    case 3: // ���� ����
                        //mirrorObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
                        mirrorObject.transform.forward = Vector3.up;

                        break;
                    case 4: // �Ʒ��� ����
                        //mirrorObject.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        mirrorObject.transform.forward = Vector3.down;

                        break;
                    default: // �� ���� ���
                        Debug.LogError("�ſ� ��ġ���� ����!");
                        break;
                }


                Vector3 forwardDirection = mirrorObject.transform.forward;
                mirrorObject.transform.forward = forwardDirection;
                mirror1.SetForwardDirection(forwardDirection); // forwardDirection ���� Mirror1 ��ũ��Ʈ�� �����մϴ�.

                // ��� ���
                Debug.Log($"mirrorObject.transform.forward: {forwardDirection}");

                //Debug.Log($"mirrorObject.transform.forward{mirrorObject.transform.forward}");
            }


            else // �ٴڿ� �ſ��� ���´ٸ�
            {
                // �ſ��� ���� ���� �� �տ� ��ֹ��� �ִ��� Ȯ���ϰ�, ������ �ſ��� ���� �ʽ��ϴ�.
                RaycastHit hit;
                if (Physics.Raycast(mirrorObject.transform.position, mirrorObject.transform.forward, out hit, distanceFromPlayer))
                {
                    Debug.Log("��ֹ��� �տ� �־� �ſ��� ���� �� �����ϴ�.");
                    // ��ֹ��� �տ� ������ �ſ��� ���� �ʰ� �����մϴ�.
                    return;
                }

                // �ſ��� �ٴڿ� �����ϴ�.
                mirrorObject.transform.parent = null; // �θ� ������ �����Ͽ� �ڽĿ��� �����ϴ�.
                mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Vector3 mirrorPosition = transform.position + transform.forward * distanceFromPlayer;
                mirrorObject.transform.position = mirrorPosition;
                mirror1.wallChecker = false;
            }
        
        }
        else
        {
            Debug.LogWarning("���� ������Ʈ�� ��ȿ���� �ʽ��ϴ�.");
        }
    }
}