using UnityEngine;
using System.Collections;


public class Mirror1 : MonoBehaviour
{
    [SerializeField] Transform mirror2; // 거울2의 Transform을 참조하는 변수
    [SerializeField] float Mirror2OffsetX = 20f; // Y값 오프셋
    [SerializeField] float ObstacleOffsetY = -5f; // Y값 오프셋
    [SerializeField] float ObstacleOffsetX;
    [SerializeField] float ObstacleOffsetZ;
    [SerializeField] float moveDuration = 2f; // 이동하는 시간

    [SerializeField] GameObject Mirror1WallPoint;
    [SerializeField] YHP_PlayerController YDJ_PlayerController;
    //[SerializeField] Obstacle obstacleScript;

    public bool wallChecker;
    public bool WallChecker { get { return wallChecker; } }

    public bool obstacleChecker;
    public bool ObstacleChecker { get { return obstacleChecker; } }

    public bool mirror1InObstacleChecker;
    public bool Mirror1InObstacleChecker { get { return mirror1InObstacleChecker; } }


    private Vector3 forwardDirection;

    public void SetForwardDirection(Vector3 direction)
    {
        forwardDirection = direction;

        // 거울이 벽에 붙어있는 경우
        if (wallChecker)
        {
            Debug.Log($"forwardDirection{forwardDirection}");
            // 거울에 들어간 방향을 가져옴
            //Vector3 mirrorEnterDirection = obstacleScript.GetMirrorEnterDirection();

            if (forwardDirection.x > 0) // 오른쪽
            {
                transform.localRotation = Quaternion.Euler(0, 0, 90);

            }
            else if (forwardDirection.x < 0) // 왼쪽
            {
                transform.localRotation = Quaternion.Euler(0, 0, -90);
            }
            else if (forwardDirection.y > 0) // 위
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

    void Update()
    {
        if (!wallChecker)
        {
            Vector3 newPosition = transform.position;
            newPosition.z += Mirror2OffsetX;
            mirror2.position = newPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Debug.Log("벽에 닿음");
            wallChecker = true;

            Vector3 newPosition = transform.position;
            newPosition.z += Mirror2OffsetX;
            newPosition.y = 0;
            mirror2.position = newPosition;
            obstacleChecker = false;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            //if (Vector3.Distance(transform.position, other.transform.position) < 0.5f)
            //{
            //    Debug.Log("거울과 장애물의 거리가 1 미만입니다.");
            //    return;

            //}

                Debug.Log("거울에 장애물 닿음");
            //obstacleChecker = true;
            if (!YDJ_PlayerController.MirrorHolding)
            {
                StartCoroutine(MirrorInObstacle(other.gameObject));
            }
        }
        else
        {
            obstacleChecker = false;
        }



    }


    IEnumerator MirrorInObstacle(GameObject obstacle)
    {
        Rigidbody obstacleRigidbody = obstacle.GetComponent<Rigidbody>();
        Collider obstacleCollider = obstacle.GetComponent<Collider>();

        Vector3 endPosition;
        float time = 0;
        float targetTime = 1f;
        Vector3 startPosition = transform.position;
        //Vector3 startPosition = Mirror1WallPoint.transform.position;
        startPosition.y = 0;

        if (wallChecker) // 벽 거울1
        {
            //gameObject.layer == LayerMask.NameToLayer("Wall")
            //Debug.Log("벽 거울1에 장애물 닿음");
            //CollisionManager collisionManager = GetComponent<CollisionManager>();
            //collisionManager.IgnoreCollision("Obstacle", "Wall", false);
            //collisionManager.IgnoreCollision("Player", "Obstacle", false);


            //obstacleRigidbody.isKinematic = true;

            if (forwardDirection.x > 0) // 오른쪽으로 들어옴
            {
                // 시작 위치 계산: 거울에서 장애물이 들어온 방향과 반대로 이동
                endPosition = transform.position + new Vector3(ObstacleOffsetX, 0f, 0f);
                Debug.Log("장애물이 오른쪽에서 들어왔습니다.");
            }
            else if (forwardDirection.x < 0) // 왼쪽으로 들어옴
            {
                endPosition = transform.position - new Vector3(ObstacleOffsetX, 0f, 0f);
                Debug.Log("장애물이 왼쪽에서 들어왔습니다.");
            }
            else if (forwardDirection.y > 0) // 위로 들어옴
            {
                endPosition = transform.position + new Vector3(0, 0f, ObstacleOffsetZ);
                Debug.Log("장애물이 위에서 들어왔습니다.");
            }
            else // 아래로 들어옴
            {
                endPosition = transform.position - new Vector3(0, 0f, ObstacleOffsetZ);
                Debug.Log("장애물이 아래에서 들어왔습니다.");
            }

            while (time < targetTime)
            {
                time += Time.deltaTime;
                Debug.Log($"obstacle in{obstacle.transform.position}");
                obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, time / targetTime);
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            StartCoroutine(MirrorOutObstacle(obstacle));
        }




        else // 바닥 거울1
        {
            endPosition = transform.position + new Vector3(0f, ObstacleOffsetY, 0f);

            while (time < targetTime)
            {
                time += Time.deltaTime;
                Debug.Log($"obstacle in{obstacle.transform.position}");
                obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, time / targetTime);
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            StartCoroutine(MirrorOutObstacle(obstacle));
        }











    }


    IEnumerator MirrorOutObstacle(GameObject obstacle)
    {
        Vector3 endPosition = mirror2.position;
        float time = 0;
        float targetTime = 1f;
        Vector3 startPosition;

        if (wallChecker)
        {

            Debug.Log($"forwardDirection{forwardDirection}");
            // 거울에 들어간 방향을 가져옴
            if (forwardDirection.x > 0) // 오른쪽으로 들어옴
            {
                // 시작 위치 계산: 거울에서 장애물이 들어온 방향과 반대로 이동
                startPosition = mirror2.position + new Vector3(ObstacleOffsetX, 0f, 0f);
                Debug.Log("장애물이 오른쪽에서 들어왔습니다.");
            }
            else if (forwardDirection.x < 0) // 왼쪽으로 들어옴
            {
                startPosition = mirror2.position - new Vector3(ObstacleOffsetX, 0f, 0f);
                Debug.Log("장애물이 왼쪽에서 들어왔습니다.");
            }
            else if (forwardDirection.y > 0) // 위로 들어옴
            {
                startPosition = mirror2.position + new Vector3(0, 0f, ObstacleOffsetZ);
                Debug.Log("장애물이 위에서 들어왔습니다.");
            }
            else // 아래로 들어옴
            {
                startPosition = mirror2.position - new Vector3(0, 0f, ObstacleOffsetZ);
                Debug.Log("장애물이 아래에서 들어왔습니다.");
            }
        }

        else
        {
            startPosition = mirror2.position + new Vector3(0f, ObstacleOffsetY, 0f);
        }



        while (time < targetTime)
        {
            time += Time.deltaTime;
            Debug.Log($"obstacle out{obstacle.transform.position}");
            obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, time/targetTime);
            yield return null;
        }
    }    
}


