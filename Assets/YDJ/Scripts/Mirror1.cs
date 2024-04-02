using UnityEngine;
using System.Collections;

public class Mirror1 : MonoBehaviour
{
    [SerializeField] Transform mirror2; // 거울2의 Transform을 참조하는 변수
    [SerializeField] float Mirror2OffsetZ = 20f; // Y값 오프셋
    [SerializeField] float ObstacleOffsetY = -5f; // Y값 오프셋
    [SerializeField] float moveDuration = 2f; // 이동하는 시간


    void Update()
    {
        Vector3 newPosition = transform.position;
        newPosition.z += Mirror2OffsetZ;
        mirror2.position = newPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Obstacle"))
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            // 코루틴을 시작하여 장애물을 다시 생성
            StartCoroutine(RespawnObstacle(other.gameObject));
        }
    }

    IEnumerator RespawnObstacle(GameObject obstacle)
    {
        Collider obstacleCollider = obstacle.GetComponent<Collider>();
        obstacleCollider.enabled = false;

        yield return new WaitForSeconds(2f);

        //obstacle.transform.position = mirror2.position;

        //Vector3 newPosition = mirror2.position + new Vector3(0f, ObstacleOffsetY, 0f);
        //obstacle.transform.position = newPosition;

        // 거울2 아래로 이동
        Vector3 startPosition = mirror2.position + new Vector3(0f, ObstacleOffsetY, 0f);
        Vector3 endPosition = obstacle.transform.position = mirror2.position;
        float startTime = Time.time;
        while (Time.time - startTime < moveDuration)
        {
            float fraction = (Time.time - startTime) / moveDuration;
            obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, fraction);
            yield return null;
        }

        // 미러2 위치로 이동
        obstacle.transform.position = mirror2.position;

        obstacleCollider.enabled = true;
    }
}
