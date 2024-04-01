using System.Collections;
using UnityEngine;

public class Mirror1 : MonoBehaviour
{
    [SerializeField] Transform spawnMirror; // �ſ� ��ġ
    [SerializeField] float moveDuration; // �̵��ϴµ� �ɸ��� �ð�

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
        Vector3 targetPosition = spawnMirror.position; // ��ǥ ��ġ�� �ſ��� ��ġ�� ����

        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            obstacleTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��ֹ� �̵��� �Ϸ�Ǹ� ��ֹ��� ����
        Destroy(obstacleTransform.gameObject);
    }
}
