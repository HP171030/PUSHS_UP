using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class YHP_PlayerController : MonoBehaviour
{
    public Vector3 moveDir;

    bool moveOn;
    public bool inputKey = true;
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
    [SerializeField] LayerMask mirror;
    [SerializeField] LayerMask crystal;
    [SerializeField] public bool onIce = false;
    [SerializeField] Holder holder;
    [SerializeField] Mirror1 mirror1;
    [SerializeField] GameObject mirror1Image;

    LayerMask layerMask;

    [SerializeField] CameraSwitch cameraSwitch;

    Transform obstacle;
    RaycastHit otherObs;

    Vector3 debugVec;

    RaycastHit grabHit;



    [Header("Property")]
    [SerializeField] GameObject holderPoint;
    [SerializeField] float distanceFromPlayer;
    [SerializeField] float wallMirrorOffset;
    [SerializeField] float unHoldwallMirrorOffset;



    public bool mirrorHolding = false; // �ſ��� �ִ��� Ȯ���ϴ� �÷���
    public bool MirrorHolding { get { return mirrorHolding; } }

    public bool wallMirrorBumpChecker;
    public bool WallMirrorBumpChecker { get { return wallMirrorBumpChecker; } }



    public float mirror1WallAttachedDir;







    private void FixedUpdate()
    {
        OffPull();
        MoveFunc();

        //if(mirror1.WallChecker && mirrorHolding)
        //{
        //    Debug.Log("Ȧ������Ʈ �ָ��α�");
        //    Vector3 mirrorPosition = transform.position + transform.forward * wallMirrorOffset;
        //    holder.transform.position = mirrorPosition;
        //}
        //else
        //{
        //    Vector3 mirrorPosition = transform.position + transform.forward * distanceFromPlayer;
        //    holder.transform.position = mirrorPosition;
        //}


    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        layerMask = ~(1 << LayerMask.NameToLayer("Ground"));
        inputKey = true;


    }
    private void OnMove(InputValue value)
    {
        if (cameraSwitch.IsPlayer1Active && !onIce && inputKey)
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

            if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, out grabHit, 1.5f))
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
            if (!pullOn)
                moveOn = false;
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

        // �ſ��� ��� �ִ� ��쿡�� �ſ��� ���̾ �����Ͽ� ���̾��ũ�� �����մϴ�.
        LayerMask layerMask = mirrorHolding ? ~(1 << LayerMask.NameToLayer("Ground")) | mirror : ~(1 << LayerMask.NameToLayer("Ground"));

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
                pullOn = true;
                if (grabHit.collider != null)
                {
                    Debug.Log($"���� {grabHit.collider.gameObject.name}");
                    Vector3 grabDir = (grabHit.collider.gameObject.transform.position - transform.position).normalized;
                    if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), -transform.forward, out RaycastHit hit, 1.5f))
                    {
                        if (hit.collider != null)
                        {
                            Debug.Log($"�ڿ� ������ {hit.collider.gameObject.name} ");
                            moveOn = false;
                            return;
                        }
                    }

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
                        pullOn = false;
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
        moveOn = true;
        LayerMask backObsWall = wall | obstacleLayer;
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit somethingBack, 1f, backObsWall))
        {
            Debug.Log($"�ڿ� ���� {somethingBack.collider.gameObject.name}");
            yield break;
        }
        Vector3 startPos = transform.position;
        Vector3 targetPos = pullDir;
        Vector3 grabStartPos = grabHit.collider.transform.position;
        Vector3 grabTargetPos = pullDir;


        if (X)
        {
            Collider[] pullTarget = Physics.OverlapSphere(transform.position - transform.forward * 2, 0.5f, ground | wall);
            Tile tile;

            if (pullTarget.Length > 0)
            {
                foreach (Collider col in pullTarget)
                {
                    if (wall.Contain(col.gameObject.layer))
                    {
                        Debug.Log("�ڿ� ����");
                        yield break;
                    }
                    tile = col.gameObject.GetComponent<Tile>();

                    Debug.Log($"�ڿ� Ÿ�� �̸��� {tile.name}");
                    targetPos = tile.middlePoint.position;
                    debugVec = transform.position - transform.forward * 2;
                }
            }
        }
        else if (!X)
        {
            Collider[] pullTarget = Physics.OverlapSphere(transform.position - transform.forward * 2, 0.5f, ground | wall);
            Tile tile;
            if (pullTarget.Length > 0)
            {
                foreach (Collider col in pullTarget)
                {
                    if (wall.Contain(col.gameObject.layer))
                    {
                        Debug.Log("�ڿ� ����");
                        yield break;
                    }
                    tile = col.gameObject.GetComponent<Tile>();
                    Debug.Log($"�ڿ� Ÿ�� �̸��� {tile.name}");
                    targetPos = tile.middlePoint.position;
                    debugVec = transform.position - transform.forward * 2;
                }
            }
        }
        Collider[] grabPullTarget = Physics.OverlapSphere(transform.position, 0.5f, ground);
        Tile grabTile;
        if (grabPullTarget.Length > 0)
        {
            foreach (Collider col in grabPullTarget)
            {
                grabTile = col.gameObject.GetComponent<Tile>();

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

        while (time < pullSpeed)
        {

            time += Time.deltaTime;
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, time / pullSpeed));
            grabHit.rigidbody.MovePosition(Vector3.Lerp(grabStartPos, grabTargetPos, time / pullSpeed));
            yield return null;


            if (time >= pullSpeed)
            {
                foreach (RaycastHit hit in hitArray)
                {
                    hit.collider.gameObject.transform.SetParent(null, true);
                    hit.rigidbody.isKinematic = false;
                }
                Manager.game.StepAction++;

            }


    private void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 1f))
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.yellow;
        }
        moveOn = false;
        pullOn = false;

        Gizmos.DrawRay(ray);
        if (debugVec != null)
            Gizmos.DrawWireSphere(debugVec, 0.5f);
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 1f, 0) + moveDir, new Vector3(0.2f, 0.2f, 0.2f));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (debugVec != null)
            Gizmos.DrawWireSphere(debugVec, 0.5f);

    }

    private IEnumerator BumpTimer()
    {
        Debug.Log("1�� ������ ��������");
         yield return new WaitForSeconds(2f);
        wallMirrorBumpChecker = false;
    }






    private IEnumerator MoveRoutine(Vector3 moveDirValue)
    {
        LayerMask WandObslayer = obstacleLayer | wall | mirror;
        Vector3 targetPos = transform.position + moveDirValue * moveDistance;
        debugVec = targetPos;
        Collider[] tiles = Physics.OverlapSphere(targetPos, 0.5f, ground);


        if (tiles.Length > 0)
        {

            foreach (Collider tile in tiles)
            {

                Tile tileIns = tile.GetComponent<Tile>();

                if (tileIns != null)
                {
                    Debug.Log($"True ,{tileIns.gameObject.name} ");
                    Collider[] isBlank = Physics.OverlapSphere(tileIns.transform.position, 0.5f, WandObslayer);

                    if (isBlank.Length == 0)
                    {
                        targetPos = tileIns.middlePoint.position;
                        debugVec = targetPos;
                        Debug.Log(tileIns.gameObject.name);
                    }
                }
                else
                {
                    Debug.Log("WhatThe");
                }

            }
        }
        Vector3 startPos = transform.position;
        RaycastHit hit;
        Vector3 PreMoveDir = moveDir;

        if (Physics.BoxCast(transform.position + new Vector3(0, 1f, 0), new Vector3(0.2f, 0.2f, 0.2f), moveDirValue * 2f, out hit, Quaternion.identity, 1f))
        {

            if (obstacleLayer.Contain(hit.collider.gameObject.layer) && !mirrorHolding)
            {
                Debug.Log("asff");
                obstacle = hit.transform;
                Vector3 obsStartPos = obstacle.position;
                Vector3 obsTargetPos = obstacle.position + moveDirValue * moveDistance;
                Collider[] colliders = Physics.OverlapSphere(obsTargetPos, 0.5f, ground);
                if (colliders.Length > 0)
                {
                    foreach (Collider collider in colliders)
                    {
                        Tile obsTile = collider.GetComponent<Tile>();
                        obsTargetPos = obsTile.middlePoint.position;

                    }
                }

                    }
                }

                float time = 0;

                LayerMask layer = obstacleLayer | wall | crystal;
                if (Physics.BoxCast(obstacle.position, new Vector3(0.5f, 0.5f, 0.5f), moveDirValue, out RaycastHit hitInfo, Quaternion.identity, 0.7f, layer))
                {
                    Debug.Log($"�ڿ� {hitInfo.collider.gameObject.name}�� �ִ�");
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
                    List<RaycastHit> pushHitArray = new List<RaycastHit>(Physics.RaycastAll(hit.collider.transform.position, hit.collider.transform.up, 10f, obstacleLayer));
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
                            animator.SetBool("Push", false);
                            moveOn = false;
                            yield break;
                        }

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
                        //  hits.rigidbody.isKinematic = false;
                    }

                    Debug.Log("��ֹ��� ���� ����");
                }
            }

            else if (obstacleLayer.Contain(hit.collider.gameObject.layer) && mirrorHolding)
            {
                Debug.Log("�ſ��� ��� ��ֹ��� �� �� �����ϴ�.");
                moveOn = false;
                yield return null;
            }
            else if (holder.FrontMirrorLader() && !mirrorHolding && !mirror1.WallChecker)
            {
                Debug.Log("�ſ��� ��� �� �� �����ϴ�.FrontMirrorLader");
                moveOn = false;
                yield return null;
            }
            else if (holder.FrontObstacleLader() && mirrorHolding)
            {
                Debug.Log("�ſ��� ��� ��ֹ��� �� �� �����ϴ�. FrontObstacleLader");
                moveOn = false;
                yield return null;
            }
            else if (wall.Contain(hit.collider.gameObject.layer))
            {
                Debug.Log($"wall name is {hit.collider.gameObject.name}");
                moveOn = false;
                yield return null;

            }

            else if (mirror.Contain(hit.collider.gameObject.layer) && !mirrorHolding && !mirror1.WallChecker)
            {
                Debug.Log($"�ſ��� �տ� �־ �̵��� �� �����ϴ�.");
                moveOn = false;
                yield return null;
            }
            //else if (obstacleLayer.Contain(hit.collider.gameObject.layer) && mirrorHolding)
            //{
            //    Debug.Log($"�ſ��� ��� ��ֹ��� �� �� �����ϴ�");
            //    moveOn = false;
            //    yield return null;
            //}
            else
            {
                float time = 0;
                while (time < 1)
                {
                    if (Physics.Raycast(transform.position, transform.forward, out RaycastHit isWall, 0.2f, wall))
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
                if (moveDir.magnitude < 1 || PreMoveDir != moveDir)
                {
                    animator.SetBool("Push", false);
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
                if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), transform.forward, out RaycastHit hitinfo, 1f))
                {
                    moveOn = false;
                    yield break;
                }

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
        Debug.Log("��ǲ ����Ʈ");
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

            mirror1Image.transform.localPosition = Vector3.zero;
            Vector3 newPosition = mirror1Image.transform.localPosition;
            newPosition.y += wallMirrorOffset;
            mirror1Image.transform.localPosition = newPosition;
        }
        else
        {
            Debug.Log("�������");
            mirrorHolding = false;
        }
    }

    private void UnHold()
    {

        GameObject mirrorObject = holder.GetMirror();


        if (mirrorObject != null)
        {


            if (holder.WallLader() && !mirror1.MoveDisableChecker) //���� �ſ��� ���´ٸ�
            {
                //holdChecker = false;
                mirrorObject.transform.parent = null; // �θ� ������ �����Ͽ� �ڽĿ��� �����ϴ�.


                switch (mirror1WallAttachedDir)
                {
                    case 1: // ���������� ����
                        //mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
                        mirrorObject.transform.forward = Vector3.right;

                        //newmirror1ImegePosition.y -= wallMirrorOffset;
                        Debug.Log("���������� ��");
                        break;
                    case 2: // �������� ����
                        //mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, -90);
                        mirrorObject.transform.forward = Vector3.left;
                        //newmirror1ImegePosition.y += wallMirrorOffset;
                        Debug.Log("�������� ��");

                        break;
                    case 3: // ���� ����
                        //mirrorObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
                        mirrorObject.transform.forward = Vector3.up;
                        //newmirror1ImegePosition.x -= wallMirrorOffset;
                        Debug.Log("���� ��");

                        break;
                    case 4: // �Ʒ��� ����
                        //mirrorObject.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        mirrorObject.transform.forward = Vector3.down;
                        //newmirror1ImegePosition.x += wallMirrorOffset;
                        Debug.Log("�Ʒ��� ��");

                        break;
                    default: // �� ���� ���
                        Debug.LogError("�ſ� ��ġ���� ����!");
                        break;
                }

                mirrorHolding = false;
                Vector3 forwardDirection = mirrorObject.transform.forward;
                mirrorObject.transform.forward = forwardDirection;
                mirror1.SetForwardDirection(forwardDirection); // forwardDirection ���� Mirror1 ��ũ��Ʈ�� �����մϴ�.



                // �ſ��� ���� ���� �� mirror1Image�� �ø��մϴ�.
                if (mirror1WallAttachedDir == 1) // ������
                {
                    Vector3 newPosition = mirror1Image.transform.localPosition;
                    newPosition.y -= unHoldwallMirrorOffset;
                    mirror1Image.transform.localPosition = newPosition;

                    //Vector3 newPosition1 = mirror1Image.transform.localPosition;
                    //newPosition1.y -= unHoldwallMirrorOffset;
                    //mirror1.transform.localPosition = newPosition1;
                }
                else if (mirror1WallAttachedDir == 2) // ����
                {
                    Vector3 newPosition = mirror1Image.transform.localPosition;
                    newPosition.y -= unHoldwallMirrorOffset;
                    mirror1Image.transform.localPosition = newPosition;

                    //Vector3 newPosition1 = mirror1Image.transform.localPosition;
                    //newPosition1.y -= unHoldwallMirrorOffset;
                    //mirror1.transform.localPosition = newPosition1;
                }

                // ��� ���
                Debug.Log($"mirrorObject.transform.forward: {forwardDirection}");
                Debug.Log("�ſ� ���� ����");
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
                if (mirror1.MoveDisableChecker)
                {
                    mirror1.wallChecker = false;
                    Debug.Log("������ �� ���� ��ֹ��� �տ� �־� �ſ��� ���� �� �����ϴ�.");
                    return;
                }



                // �ſ��� �ٴڿ� �����ϴ�.
                mirrorObject.transform.parent = null; // �θ� ������ �����Ͽ� �ڽĿ��� �����ϴ�.
                mirrorObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
                Vector3 mirrorPosition = transform.position + transform.forward * distanceFromPlayer;
                mirrorObject.transform.position = mirrorPosition;
                mirrorHolding = false;
                mirror1Image.transform.localPosition = Vector3.zero;
                mirror1.wallChecker = false;



                Debug.Log("�ſ� �ٴڿ� ����");
            }

        }
        else
        {
            Debug.LogWarning("���� ������Ʈ�� ��ȿ���� �ʽ��ϴ�.");
        }
    }
}



