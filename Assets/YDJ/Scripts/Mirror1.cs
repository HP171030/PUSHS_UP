using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;


public class Mirror1 : MonoBehaviour
{
    [SerializeField] Transform mirror2Transform; // �ſ�2�� Transform�� �����ϴ� ����

    [Header("Property")]
    [SerializeField] float Mirror2OffsetZ; // Y�� ������
    [SerializeField] float ObstacleOffsetY = -5f; // Y�� ������
    [SerializeField] float ObstacleOffsetX;
    [SerializeField] float ObstacleOffsetZ;
    [SerializeField] float ObstacleInMinrrorCoroutineTime;
    [SerializeField] float WallMirror2Offset;

    [SerializeField] float XOffset;
    [SerializeField] Transform MapA;
    [SerializeField] Transform MapB;
    
    Vector3 mirrorZPos;
    [SerializeField] Transform transmissionPos;
    public Mirror2 Mirror2 { get { return mirror2; } }
    public GameObject mirrorImage;
    public bool attached;

    [Header("Config")]

    //[SerializeField] WallCollider wallCollider;

    [SerializeField] YHP_PlayerController YHP_PlayerController;
    [SerializeField] Mirror2 mirror2;
    [SerializeField] Mirror2 mirror2Instance;
    //[SerializeField] Obstacle obstacleScript;

    public bool wallChecker;
    public bool WallChecker { get { return wallChecker; } }

    public bool obstacleChecker;
    public bool ObstacleChecker { get { return obstacleChecker; } }

    public bool moveDisableChecker;
    public bool MoveDisableChecker { get { return moveDisableChecker; } }

    public bool mirror1InObstacleChecker;
    public bool Mirror1InObstacleChecker { get { return mirror1InObstacleChecker; } }

    public bool mirrorObstacleAttachedChecker = false;
    public bool MirrorObstacleAttachedChecker { get { return mirrorObstacleAttachedChecker; } }


    private bool IsWallExit = true;

    //private bool wallMirrorAttachedChecker = false;

    private Vector3 forwardDirection;
    private void OnDisable()
    {
        Debug.Log("disable");
    }
    public void SetForwardDirection( Vector3 direction )
    {
        forwardDirection = direction;

        // �ſ��� ���� �پ��ִ� ���
        if ( wallChecker )
        {
            Debug.Log($"forwardDirection{forwardDirection}");
            // �ſ￡ �� ������ ������
            //Vector3 mirrorEnterDirection = obstacleScript.GetMirrorEnterDirection();

            if ( forwardDirection.x > 0 ) // ������
            {
                transform.localRotation = Quaternion.Euler(0, 0, 90);

            }
            else if ( forwardDirection.x < 0 ) // ����
            {
                transform.localRotation = Quaternion.Euler(0, 0, -90);
            }
            else if ( forwardDirection.y > 0 ) // ��
            {
                transform.localRotation = Quaternion.Euler(90, 0, 0);
            }
            else // �Ʒ�
            {
                transform.localRotation = Quaternion.Euler(-90, 0, 0);
            }
        }
        else
        {
            // ���� �پ����� ���� ��� �⺻ �����̼��� �����մϴ�.
            transform.localRotation = Quaternion.identity;
        }
    }

    private void Awake()
    {
        YHP_PlayerController = FindObjectOfType<YHP_PlayerController>();
        Debug.Log(Vector3.Distance(MapA.transform.position, MapB.transform.position) );
        Mirror2OffsetZ = Vector3.Distance(MapA.transform.position, MapB.transform.position);
        mirror2 = Instantiate(mirror2Instance, transform.position, Quaternion.identity) ;
        mirror2Transform = mirror2.transform;

    }

   public Vector3 endPosition;
   void Update()
    {
        mirrorZPos.z = Mirror2OffsetZ;
       

        RaycastHit [] tiles = Physics.RaycastAll(transform.position + mirrorZPos, Vector3.down,2f,LayerMask.GetMask("Ground"));
        if ( tiles.Length > 0 )
        {
            foreach ( RaycastHit tile in tiles )
            {
                Tile t = tile.collider.GetComponent<Tile>();
                if ( t != null )
                {
                    endPosition = t.middlePoint.position;
                    Debug.DrawLine(transform.position + mirrorZPos, transform.position + mirrorZPos + new Vector3(0, 2, 0));
                }
            }
        }

            mirror2Transform.position = endPosition;

    }


    private void OnTriggerEnter( Collider other )
    {


        // && IsWallExit
        if ( other.gameObject.layer == LayerMask.NameToLayer("Wall") )
        {

            Debug.Log("���� ����aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            wallChecker = true;

        }
        //if (YDJ_PlayerController.mirror1WallAttachedDir == 1 || YDJ_PlayerController.mirror1WallAttachedDir == 2) // ������ , ����
        {
            //switch (YDJ_PlayerController.Mirror1WallAttachedDir)
            //{
            //    case 1: // ���������� ����

            //        Vector3 Position = mirror2.transform.position;
            //        Position.z += ZOffset;
            //        mirror2.transform.localPosition = Position;
            //        //newmirror1ImegePosition.y -= wallMirrorOffset;
            //        break;
            //    case 2: // �������� ����
            //        mirror2.transform.forward = Vector3.left;
            //        //newmirror1ImegePosition.y += wallMirrorOffset;
            //        Debug.Log("�������� ��");

            //        break;
            //    case 3: // ���� ����
            //        mirror2.transform.forward = Vector3.up;
            //        //newmirror1ImegePosition.x -= wallMirrorOffset;
            //        Debug.Log("���� ��");

            //        break;
            //    case 4: // �Ʒ��� ����
            //        mirror2.transform.forward = Vector3.down;
            //        //newmirror1ImegePosition.x += wallMirrorOffset;
            //        Debug.Log("�Ʒ��� ��");

            //        break;
            //    default: // �� ���� ���
            //        Debug.LogError("�ſ� ��ġ���� ����!");
            //        break;
            //}




            {

                obstacleChecker = false;

                IsWallExit = false;
            }
            if ( other.gameObject.CompareTag("MoveDisable") )
            {

                moveDisableChecker = true;
                wallChecker = false;
            }





        }
    }

    //private IEnumerator AlreadyMap2ObstacleTimer()
    //{
    //    Debug.Log("1�� ������ AlreadyMap2ObstacleTimer����");
    //    yield return new WaitForSeconds(1f);
    //    YDJ_PlayerController.AlreadyMap2Obstacle = false;
    //}

    private void OnTriggerStay( Collider other )
    {


        //if (mirror2.ObstacleChecker) // �ſ�2�� ��ֹ� ���� �� ���ſ� �۵�����
        //{
        //    return;
        //}

        if ( other.gameObject.layer == LayerMask.NameToLayer("Obstacle") && !YHP_PlayerController.mirrorHolding )
        {

            if ( wallChecker && !mirror2.ObstacleChecker ) //���ſ�
            {
                Debug.Log("���ſ￡ ����");

                //if (mirror2.obstacleChecker)
                //{
                //    YDJ_PlayerController.AlreadyMap2Obstacle = true;
                //    //AlreadyMap2ObstacleTimer();
                //}

                if ( YHP_PlayerController.wallMirrorBumpChecker )
                {
                    Debug.Log("���� ---------------------------------------------");
                 //   obstacleChecker = false;
                 //   StartCoroutine(MirrorInObstacleWall(other.gameObject));
                }
            }
            else if ( !wallChecker && !attached )
            {
                Debug.Log("�ٴڰſ�");
                StartCoroutine(MirrorInObstacleGround(other.gameObject));
            }
            //else
            //{
            //    YDJ_PlayerController.wallMirrorBumpChecker = false;
            //    obstacleChecker = false;
        }



        if ( other.gameObject.layer == LayerMask.NameToLayer("Wall") )
        {

            wallChecker = true;
            switch ( YHP_PlayerController.Mirror1WallAttachedDir )
            {
                case 1: // ���������� ����

                    Vector3 newPosition = transform.position;
                    newPosition.z += Mirror2OffsetZ;
                    newPosition.x -= XOffset;
                    newPosition.y = 0;
                    mirror2.transform.position = newPosition;
                    break;
                case 2: // �������� ����
                    newPosition = transform.position;
                    newPosition.z += Mirror2OffsetZ;
                    newPosition.x += XOffset;
                    newPosition.y = 0;
                    mirror2.transform.position = newPosition;

                    break;
                case 3: // ���� ����
                    newPosition = transform.position;
                    newPosition.z += Mirror2OffsetZ;
                    newPosition.z -= XOffset;
                    newPosition.y = 0;
                    mirror2.transform.position = newPosition;

                    break;
                case 4: // �Ʒ��� ����
                    newPosition = transform.position;
                    newPosition.z += Mirror2OffsetZ;
                    newPosition.z += XOffset;
                    newPosition.y = 0;
                    mirror2.transform.position = newPosition;

                    break;
                default: // �� ���� ���
                    Debug.LogError("�ſ� ��ġ���� ����!");
                    break;
            }


        }

    }

    private void OnTriggerExit( Collider other )
    {
        IsWallExit = true;
        //obstacleChecker = false;
        mirrorObstacleAttachedChecker = false;
        moveDisableChecker = false;
        //wallChecker = false;

        if ( other.gameObject.layer == LayerMask.NameToLayer("Wall") )
        {

            Debug.Log("������ ������");
            wallChecker = false;

        }
    }


    public IEnumerator MirrorInObstacleWall( GameObject obstacle ) //��ֹ� ����
    {
        Debug.Log("�ڷ�ƾ ��");
        //Rigidbody obstacleRigidbody = obstacle.GetComponent<Rigidbody>();
        //Collider obstacleCollider = obstacle.GetComponent<Collider>();

        //Vector3 endPosition;
        //float time = 0;
        //float targetTime = 1f;
        //Vector3 startPosition = transform.position;
        ////Vector3 startPosition = Mirror1WallPoint.transform.position;
        //startPosition.y = 0;



        //gameObject.layer == LayerMask.NameToLayer("Wall")
        //Debug.Log("�� �ſ�1�� ��ֹ� ����");
        //CollisionManager collisionManager = GetComponent<CollisionManager>();
        //collisionManager.IgnoreCollision("Obstacle", "Wall", false);
        //collisionManager.IgnoreCollision("Player", "Obstacle", false);

        Debug.Log("���ſ�");

        //if (forwardDirection.x > 0) // ���������� ����
        //{
        //    // ���� ��ġ ���: �ſ￡�� ��ֹ��� ���� ����� �ݴ�� �̵�
        //    endPosition = transform.position + new Vector3(ObstacleOffsetX, 0f, 0f);
        //    Debug.Log("��ֹ��� �����ʿ��� ���Խ��ϴ�.");
        //}
        //else if (forwardDirection.x < 0) // �������� ����
        //{
        //    endPosition = transform.position - new Vector3(ObstacleOffsetX, 0f, 0f);
        //    Debug.Log("��ֹ��� ���ʿ��� ���Խ��ϴ�.");
        //}
        //else if (forwardDirection.y > 0) // ���� ����
        //{
        //    endPosition = transform.position + new Vector3(0, 0f, ObstacleOffsetZ);
        //    Debug.Log("��ֹ��� ������ ���Խ��ϴ�.");
        //}
        //else // �Ʒ��� ����
        //{
        //    endPosition = transform.position - new Vector3(0, 0f, ObstacleOffsetZ);
        //    Debug.Log("��ֹ��� �Ʒ����� ���Խ��ϴ�.");
        //}

        //while (time < targetTime)
        //{
        //    time += Time.deltaTime;
        //    //Debug.Log($"obstacle in{obstacle.transform.position}");
        //    obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, time / targetTime);
        //    yield return null;
        //}
        obstacle.transform.position = mirror2.transform.position;
        yield return null;
        //yield return new WaitForSeconds(ObstacleInMinrrorCoroutineTime);
        StartCoroutine(MirrorOutObstacle(obstacle));

        //yield return new WaitForSeconds(ObstacleInMinrrorCoroutineTime);

        //StartCoroutine(MirrorOutObstacle(obstacle));


    }


    public IEnumerator MirrorInObstacleGround( GameObject obstacle ) //��ֹ� ����
    {
        Manager.game.PlayerControllStop();
        Debug.Log("�ڷ�ƾ ��");
        Rigidbody obstacleRigidbody = obstacle.GetComponent<Rigidbody>();
        Collider obstacleCollider = obstacle.GetComponent<Collider>();
       // obstacleRigidbody.isKinematic = false;

        Vector3 endPosition;
        float time = 0;
        float targetTime = 1f;
        Vector3 startPosition = transform.position;
        //Vector3 startPosition = Mirror1WallPoint.transform.position;
        startPosition.y = 0;



        Debug.Log("���ſ�");

        endPosition = transform.position + new Vector3(0f, ObstacleOffsetY, 0f);

        while ( time < targetTime )
        {
            time += Time.deltaTime;
            //Debug.Log($"obstacle in{obstacle.transform.position}");
            obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, time / targetTime);
            yield return null;
        }

        yield return new WaitForSeconds(ObstacleInMinrrorCoroutineTime);

        StartCoroutine(MirrorOutObstacle(obstacle));

    }







    IEnumerator MirrorOutObstacle( GameObject obstacle ) // ��ֹ� ������
    {

       
        float time = 0;
        float targetTime = 1f;
        Vector3 startPosition;

        if ( wallChecker )
        {

            Debug.Log($"forwardDirection{forwardDirection}");
            // �ſ￡ �� ������ ������
            if ( forwardDirection.x > 0 ) // ���������� ����
            {
                // ���� ��ġ ���: �ſ￡�� ��ֹ��� ���� ����� �ݴ�� �̵�
                startPosition = mirror2Transform.position + new Vector3(ObstacleOffsetX, 0f, 0f);
                Debug.Log("��ֹ��� �����ʿ��� ���Խ��ϴ�.");
            }
            else if ( forwardDirection.x < 0 ) // �������� ����
            {
                startPosition = mirror2Transform.position - new Vector3(ObstacleOffsetX, 0f, 0f);
                Debug.Log("��ֹ��� ���ʿ��� ���Խ��ϴ�.");
            }
            else if ( forwardDirection.y > 0 ) // ���� ����
            {
                startPosition = mirror2Transform.position + new Vector3(0, 0f, ObstacleOffsetZ);
                Debug.Log("��ֹ��� ������ ���Խ��ϴ�.");
            }
            else // �Ʒ��� ����
            {
                startPosition = mirror2Transform.position - new Vector3(0, 0f, ObstacleOffsetZ);
                Debug.Log("��ֹ��� �Ʒ����� ���Խ��ϴ�.");
            }
        }

        else                //���� ����
        {
            startPosition = mirror2Transform.position + new Vector3(0f, ObstacleOffsetY, 0f);
            RaycastHit [] rays = Physics.BoxCastAll(startPosition, new Vector3(1, 1, 1), Vector3.up, Quaternion.identity);
            obstacle.transform.position = startPosition;
            foreach ( RaycastHit ray in rays )
            {
                if ( ray.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle") || ray.collider.gameObject.CompareTag("Player") && ray.collider.gameObject != obstacle )
                {
                    Rigidbody rb = ray.collider.gameObject.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.constraints |= RigidbodyConstraints.FreezePositionX;
                    rb.constraints |= RigidbodyConstraints.FreezePositionZ;
                    ray.transform.SetParent(obstacle.transform, true);
                    
                }
            }

            while ( time < targetTime )
            {
                time += Time.deltaTime;
                //Debug.Log($"obstacle out{obstacle.transform.position}");
                foreach( RaycastHit ray in rays )
                {
                    obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, time / targetTime);
                }

                yield return null;

            }
            foreach ( RaycastHit ray in rays )
            {
                if ( ray.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle") || ray.collider.gameObject.CompareTag("Player") && ray.collider.gameObject != obstacle )
                {
                    Rigidbody rb = ray.collider.gameObject.GetComponent<Rigidbody>();
                    rb.isKinematic = false;
                    rb.constraints &= ~RigidbodyConstraints.FreezePositionX;
                    rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
                    ray.transform.SetParent(null, true);

                }

            }

        }                           //���� ��



        Manager.game.PlayerControllerOn();
    }

}


