using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine.WSA;

public class PlayerController_1 : MonoBehaviour
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
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] LayerMask wall;
    [SerializeField] LayerMask ground;
    [SerializeField] Holder holder;
    [SerializeField] GameObject holderPoint;
    LayerMask layerMask;

    [SerializeField] CameraSwitch cameraSwitch;

    [Header("Property")]
    [SerializeField] float distanceFromPlayer;

    private bool wallMirrorChecker;
    public bool WallMirrorChecker { get { return wallMirrorChecker; } }

    bool isVerticalWall = true;

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
        if (cameraSwitch.IsPlayer1Active)
        {
            Vector2 input = value.Get<Vector2>();
            moveDir = new Vector3(input.x, 0, input.y);

            //�ſ� ��ġ ���� ����
            if (input.x == 1)
            {
                isVerticalWall = false;
            }
            else if (input.y == 1)
            {
                isVerticalWall = true;
            }

        }

    }

    public void OnPull(InputValue value)
    {
        if (value.isPressed && cameraSwitch.IsPlayer1Active)
        {
            grabOn = true;
            Debug.Log("���");
            animator.SetTrigger("PullStart");

            if (Physics.Raycast(transform.position, transform.forward, out grabHit, 1f))
            {
                if (obstacleLayer.Contain(grabHit.collider.gameObject.layer))
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
                if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit hit, 1.5f))
                {
                    if (hit.collider != null)
                    {
                        Debug.Log("�ڿ� ������ ");
                        moveOn = false;
                        return;
                    }
                }
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

        Debug.Log(hitArray.Count);
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
        if (Physics.Raycast(transform.position, moveDirValue, out hit, 1f))
        {

            if (obstacleLayer.Contain(hit.collider.gameObject.layer))
            {

                obstacle = hit.transform;
                Vector3 obsStartPos = obstacle.position;
                Vector3 obsTargetPos = obstacle.position + moveDirValue * moveDistance;

                float time = 0;

                LayerMask layer = obstacleLayer | wall;
                if (Physics.BoxCast(obstacle.position, new Vector3(0.5f, 0.5f, 0.5f), moveDirValue, out RaycastHit hitInfo, Quaternion.identity, 1f, layer))
                {
                    Debug.Log(hitInfo.collider.gameObject.name);
                    moveOn = false;
                    if (moveDir.magnitude < 1 || PreMoveDir != moveDir)
                    {
                        animator.SetBool("Push", false);
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

                            Debug.Log("�ڿ�");
                            moveOn = false;
                            yield break;
                        }

                        animator.SetBool("Push", true);
                        time += Time.deltaTime * moveSpeed;
                        rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / 2));
                        hit.rigidbody.MovePosition(Vector3.Lerp(obsStartPos, obsTargetPos, time / 2));
                        yield return null;
                    }
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

                    Debug.Log("��ֹ��� ���� ����");
                }

            }
            else if (wall.Contain(hit.collider.gameObject.layer))
            {
                Debug.Log("�տ� ��");
                moveOn = false;
                yield return null;

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
    //    Debug.Log("����");
    //    sphereCollider.enabled = false;
    //}


    public bool mirrorHolding = false; // �ſ��� �ִ��� Ȯ���ϴ� �÷���

    private void OnHold(InputValue value)
    {
        if (!mirrorHolding) // �ſ��� Ȯ�ε��� �ʾ��� ��
        {
            Hold();
        }
        //�ſ� ����
        else if (mirrorHolding)
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
            wallMirrorChecker = false;
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
        mirrorHolding = false;

        // �ſ��� Ȧ�� ����Ʈ���� ������ϴ�.
        GameObject mirrorObject = holder.GetMirror();


        if (mirrorObject != null)
        {
            mirrorObject.transform.parent = null; // �θ� ������ �����Ͽ� �ڽĿ��� �����ϴ�.
            if (holder.WallLader()) //���� �ſ��� ���´ٸ�
            {
                wallMirrorChecker = true;


                if (isVerticalWall) // �� �Ʒ� ���� �ſ��� ���´ٸ�
                {
                    // �� �Ʒ� ���� �°� ȸ�� ������ �����մϴ�.
                    mirrorObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    Vector3 mirrorPosition = holderPoint.transform.position;
                    // �ſ��� ��ġ�� Ȧ�� ����Ʈ�� ��ġ�� �����մϴ�.
                    mirrorObject.transform.position = mirrorPosition;
                }
                else // ���� ������ ���� �ſ��� ���´ٸ�
                {
                    // ���� ������ ���� �°� ȸ�� ������ �����մϴ�.
                    mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    Vector3 mirrorPosition = holderPoint.transform.position;
                    // �ſ��� ��ġ�� Ȧ�� ����Ʈ�� ��ġ�� �����մϴ�.
                }


                ////���� ������ ��
                //mirrorObject.transform.localRotation = Quaternion.Euler( 0, 0, 90);
                //Vector3 mirrorPosition = holderPoint.transform.position;

                ////�յ� ��
                //if ()
                //{
                //    mirrorObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
                //    Vector3 mirrorPosition = holderPoint.transform.position;
                //}
                //Vector3 mirrorPosition = transform.position + transform.forward * distanceFromPlayer; // �÷��̾� �տ� ������ �Ÿ���ŭ ������ ��ġ
                //mirrorObject.transform.position = mirrorPosition;
            }


            else // �ٴڿ� �ſ��� ���´ٸ�
            {
                mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Vector3 mirrorPosition = transform.position + transform.forward * distanceFromPlayer; // �÷��̾� �տ� ������ �Ÿ���ŭ ������ ��ġ
                mirrorObject.transform.position = mirrorPosition;
            }
        }
        else
        {
            Debug.LogWarning("���� ������Ʈ�� ��ȿ���� �ʽ��ϴ�.");
        }
    }

}
