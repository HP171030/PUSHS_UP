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

    [SerializeField] YHP_PlayerController YDJ_PlayerController;
    //[SerializeField] Obstacle obstacleScript;

    public bool wallChecker;
    public bool WallChecker { get { return wallChecker; } }

    public bool obstacleChecker;
    public bool ObstacleChecker { get { return obstacleChecker; } }


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
            mirror2.position = newPosition;
            obstacleChecker = false;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Debug.Log("거울에 장애물 닿음");
            obstacleChecker = true;
            if (!YDJ_PlayerController.MirrorHolding)
            {
                StartCoroutine(RespawnObstacle(other.gameObject));
            }
        }
        else
        {
            obstacleChecker = false;
        }


    }



    IEnumerator RespawnObstacle(GameObject obstacle)
    {
        Collider obstacleCollider = obstacle.GetComponent<Collider>();

        if (!wallChecker)
            obstacleCollider.enabled = false;
        else
        {
            Rigidbody rb = obstacle.GetComponent<Rigidbody>();
            rb.isKinematic = true;


        }


        yield return new WaitForSeconds(2f);

        Vector3 startPosition;
        if (wallChecker)
        {

            Debug.Log($"forwardDirection{forwardDirection}");
            // 거울에 들어간 방향을 가져옴
            //Vector3 mirrorEnterDirection = obstacleScript.GetMirrorEnterDirection();

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

        Vector3 endPosition = mirror2.position;
        float startTime = Time.time;
        while (Time.time - startTime < moveDuration)
        {
            float fraction = (Time.time - startTime) / moveDuration;
            obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, fraction);
            yield return null;
        }

        obstacle.transform.position = mirror2.position;
        obstacleCollider.enabled = true;
    }
}
