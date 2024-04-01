using System.Collections;
using UnityEngine;

public class Mirror1 : MonoBehaviour
{
    [SerializeField] Transform spawnMirror; // 거울 위치
    [SerializeField] float moveDuration; // 이동하는데 걸리는 시간

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            MoveObstacle(other.gameObject.transform);
        }
    }

    void MoveObstacle(Transform obstacleTransform)
    {
        StartCoroutine(MoveObstacleCoroutine(obstacleTransform));
    }

    IEnumerator MoveObstacleCoroutine(Transform obstacleTransform)
    {
        Vector3 startPosition = obstacleTransform.position;
        Vector3 targetPosition = spawnMirror.position; // 목표 위치는 거울의 위치로 설정

        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            obstacleTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 장애물 이동이 완료되면 장애물을 제거
        Destroy(obstacleTransform.gameObject);
    }
}
