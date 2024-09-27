using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;


public class Mirror1 : MonoBehaviour
{
    [SerializeField] Transform mirror2Transform; // 거울2의 Transform을 참조하는 변수

    [Header("Property")]
    [SerializeField] float Mirror2OffsetZ; // Y값 오프셋
    [SerializeField] float ObstacleOffsetY = -5f; // Y값 오프셋
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

        // 거울이 벽에 붙어있는 경우
        if ( wallChecker )
        {
            Debug.Log($"forwardDirection{forwardDirection}");
            // 거울에 들어간 방향을 가져옴
            //Vector3 mirrorEnterDirection = obstacleScript.GetMirrorEnterDirection();

            if ( forwardDirection.x > 0 ) // 오른쪽
            {
                transform.localRotation = Quaternion.Euler(0, 0, 90);

            }
            else if ( forwardDirection.x < 0 ) // 왼쪽
            {
                transform.localRotation = Quaternion.Euler(0, 0, -90);
            }
            else if ( forwardDirection.y > 0 ) // 위
            {
                transform.localRotation = Quaternion.Euler(90, 0, 0);
            }
            else // 아래
            {
                transform.localRotation = Quaternion.Euler(-90, 0, 0);
            }
        }
        else
        {
            // 벽에 붙어있지 않은 경우 기본 로테이션을 유지합니다.
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

            Debug.Log("벽에 닿음aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            wallChecker = true;

        }
        //if (YDJ_PlayerController.mirror1WallAttachedDir == 1 || YDJ_PlayerController.mirror1WallAttachedDir == 2) // 오른쪽 , 왼쪽
        {
            //switch (YDJ_PlayerController.Mirror1WallAttachedDir)
            //{
            //    case 1: // 오른쪽으로 들어옴

            //        Vector3 Position = mirror2.transform.position;
            //        Position.z += ZOffset;
            //        mirror2.transform.localPosition = Position;
            //        //newmirror1ImegePosition.y -= wallMirrorOffset;
            //        break;
            //    case 2: // 왼쪽으로 들어옴
            //        mirror2.transform.forward = Vector3.left;
            //        //newmirror1ImegePosition.y += wallMirrorOffset;
            //        Debug.Log("왼쪽으로 둠");

            //        break;
            //    case 3: // 위로 들어옴
            //        mirror2.transform.forward = Vector3.up;
            //        //newmirror1ImegePosition.x -= wallMirrorOffset;
            //        Debug.Log("위로 둠");

            //        break;
            //    case 4: // 아래로 들어옴
            //        mirror2.transform.forward = Vector3.down;
            //        //newmirror1ImegePosition.x += wallMirrorOffset;
            //        Debug.Log("아래로 둠");

            //        break;
            //    default: // 그 외의 경우
            //        Debug.LogError("거울 설치방향 예외!");
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
    //    Debug.Log("1초 지나면 AlreadyMap2ObstacleTimer폴스");
    //    yield return new WaitForSeconds(1f);
    //    YDJ_PlayerController.AlreadyMap2Obstacle = false;
    //}

    private void OnTriggerStay( Collider other )
    {


        //if (mirror2.ObstacleChecker) // 거울2에 장애물 있을 시 벽거울 작동안함
        //{
        //    return;
        //}

        if ( other.gameObject.layer == LayerMask.NameToLayer("Obstacle") && !YHP_PlayerController.mirrorHolding )
        {

            if ( wallChecker && !mirror2.ObstacleChecker ) //벽거울
            {
                Debug.Log("벽거울에 닿음");

                //if (mirror2.obstacleChecker)
                //{
                //    YDJ_PlayerController.AlreadyMap2Obstacle = true;
                //    //AlreadyMap2ObstacleTimer();
                //}

                if ( YHP_PlayerController.wallMirrorBumpChecker )
                {
                    Debug.Log("범프 ---------------------------------------------");
                 //   obstacleChecker = false;
                 //   StartCoroutine(MirrorInObstacleWall(other.gameObject));
                }
            }
            else if ( !wallChecker && !attached )
            {
                Debug.Log("바닥거울");
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
                case 1: // 오른쪽으로 들어옴

                    Vector3 newPosition = transform.position;
                    newPosition.z += Mirror2OffsetZ;
                    newPosition.x -= XOffset;
                    newPosition.y = 0;
                    mirror2.transform.position = newPosition;
                    break;
                case 2: // 왼쪽으로 들어옴
                    newPosition = transform.position;
                    newPosition.z += Mirror2OffsetZ;
                    newPosition.x += XOffset;
                    newPosition.y = 0;
                    mirror2.transform.position = newPosition;

                    break;
                case 3: // 위로 들어옴
                    newPosition = transform.position;
                    newPosition.z += Mirror2OffsetZ;
                    newPosition.z -= XOffset;
                    newPosition.y = 0;
                    mirror2.transform.position = newPosition;

                    break;
                case 4: // 아래로 들어옴
                    newPosition = transform.position;
                    newPosition.z += Mirror2OffsetZ;
                    newPosition.z += XOffset;
                    newPosition.y = 0;
                    mirror2.transform.position = newPosition;

                    break;
                default: // 그 외의 경우
                    Debug.LogError("거울 설치방향 예외!");
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

            Debug.Log("벽에서 떨어짐");
            wallChecker = false;

        }
    }


    public IEnumerator MirrorInObstacleWall( GameObject obstacle ) //장애물 들어갈때
    {
        Debug.Log("코루틴 들어감");
        //Rigidbody obstacleRigidbody = obstacle.GetComponent<Rigidbody>();
        //Collider obstacleCollider = obstacle.GetComponent<Collider>();

        //Vector3 endPosition;
        //float time = 0;
        //float targetTime = 1f;
        //Vector3 startPosition = transform.position;
        ////Vector3 startPosition = Mirror1WallPoint.transform.position;
        //startPosition.y = 0;



        //gameObject.layer == LayerMask.NameToLayer("Wall")
        //Debug.Log("벽 거울1에 장애물 닿음");
        //CollisionManager collisionManager = GetComponent<CollisionManager>();
        //collisionManager.IgnoreCollision("Obstacle", "Wall", false);
        //collisionManager.IgnoreCollision("Player", "Obstacle", false);

        Debug.Log("벽거울");

        //if (forwardDirection.x > 0) // 오른쪽으로 들어옴
        //{
        //    // 시작 위치 계산: 거울에서 장애물이 들어온 방향과 반대로 이동
        //    endPosition = transform.position + new Vector3(ObstacleOffsetX, 0f, 0f);
        //    Debug.Log("장애물이 오른쪽에서 들어왔습니다.");
        //}
        //else if (forwardDirection.x < 0) // 왼쪽으로 들어옴
        //{
        //    endPosition = transform.position - new Vector3(ObstacleOffsetX, 0f, 0f);
        //    Debug.Log("장애물이 왼쪽에서 들어왔습니다.");
        //}
        //else if (forwardDirection.y > 0) // 위로 들어옴
        //{
        //    endPosition = transform.position + new Vector3(0, 0f, ObstacleOffsetZ);
        //    Debug.Log("장애물이 위에서 들어왔습니다.");
        //}
        //else // 아래로 들어옴
        //{
        //    endPosition = transform.position - new Vector3(0, 0f, ObstacleOffsetZ);
        //    Debug.Log("장애물이 아래에서 들어왔습니다.");
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


    public IEnumerator MirrorInObstacleGround( GameObject obstacle ) //장애물 들어갈때
    {
        Manager.game.PlayerControllStop();
        Debug.Log("코루틴 들어감");
        Rigidbody obstacleRigidbody = obstacle.GetComponent<Rigidbody>();
        Collider obstacleCollider = obstacle.GetComponent<Collider>();
       // obstacleRigidbody.isKinematic = false;

        Vector3 endPosition;
        float time = 0;
        float targetTime = 1f;
        Vector3 startPosition = transform.position;
        //Vector3 startPosition = Mirror1WallPoint.transform.position;
        startPosition.y = 0;



        Debug.Log("땅거울");

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







    IEnumerator MirrorOutObstacle( GameObject obstacle ) // 장애물 나갈때
    {

       
        float time = 0;
        float targetTime = 1f;
        Vector3 startPosition;

        if ( wallChecker )
        {

            Debug.Log($"forwardDirection{forwardDirection}");
            // 거울에 들어간 방향을 가져옴
            if ( forwardDirection.x > 0 ) // 오른쪽으로 들어옴
            {
                // 시작 위치 계산: 거울에서 장애물이 들어온 방향과 반대로 이동
                startPosition = mirror2Transform.position + new Vector3(ObstacleOffsetX, 0f, 0f);
                Debug.Log("장애물이 오른쪽에서 들어왔습니다.");
            }
            else if ( forwardDirection.x < 0 ) // 왼쪽으로 들어옴
            {
                startPosition = mirror2Transform.position - new Vector3(ObstacleOffsetX, 0f, 0f);
                Debug.Log("장애물이 왼쪽에서 들어왔습니다.");
            }
            else if ( forwardDirection.y > 0 ) // 위로 들어옴
            {
                startPosition = mirror2Transform.position + new Vector3(0, 0f, ObstacleOffsetZ);
                Debug.Log("장애물이 위에서 들어왔습니다.");
            }
            else // 아래로 들어옴
            {
                startPosition = mirror2Transform.position - new Vector3(0, 0f, ObstacleOffsetZ);
                Debug.Log("장애물이 아래에서 들어왔습니다.");
            }
        }

        else                //변경 시작
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

        }                           //변경 끝



        Manager.game.PlayerControllerOn();
    }

}


