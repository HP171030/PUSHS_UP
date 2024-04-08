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

    public bool mirrorHolding = false; // �ſ��� �ִ��� Ȯ���ϴ� �÷���
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

            if (input.x > 0) // ���������� ����
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


        //��Ʈ ����ũ�� �̷�Ȧ���̸� �̷� �Ҵ� ���Ҵ� ���ϱ�
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

                            Debug.Log("�ڿ�");
                            moveOn = false;
                            yield break;
                        }
                        if (mirrorHolding)
                        {
                            Debug.Log("�ſ� ����־ ��ֹ� ����");
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

                    Debug.Log("��ֹ��� ���� ����");
                }

            }
            else if (wall.Contain(hit.collider.gameObject.layer))
            {
                Debug.Log("�տ� ��");
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
                        Debug.Log("���ſ��̶� ������ �� ����");
                    }
                    else
                    {
                        Debug.Log("�տ� �ſ�");
                        moveOn = false;
                        yield return null;
                    }
                }

            }
        }
        else // ����
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
        if (!mirrorHolding) // �ſ��� Ȯ�ε��� �ʾ��� ��
        {
            Hold();
        }
        //�ſ� ����
        else if (mirrorHolding && !holder.FrontObstacleLader())
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


        if (mirrorObject != null)
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

                if (mirror1.ObstacleChecker)
                {
                    Debug.Log("��ֹ��� �տ� �־� �ſ��� ���� �� �����ϴ�.");
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



