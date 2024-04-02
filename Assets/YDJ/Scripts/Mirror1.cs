using UnityEngine;
using System.Collections;

public class Mirror1 : MonoBehaviour
{
    [SerializeField] Transform mirror2; // �ſ�2�� Transform�� �����ϴ� ����
    [SerializeField] float Mirror2OffsetZ = 20f; // Y�� ������
    [SerializeField] float ObstacleOffsetY = -5f; // Y�� ������
    [SerializeField] float moveDuration = 2f; // �̵��ϴ� �ð�


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
            // �ڷ�ƾ�� �����Ͽ� ��ֹ��� �ٽ� ����
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

        // �ſ�2 �Ʒ��� �̵�
        Vector3 startPosition = mirror2.position + new Vector3(0f, ObstacleOffsetY, 0f);
        Vector3 endPosition = obstacle.transform.position = mirror2.position;
        float startTime = Time.time;
        while (Time.time - startTime < moveDuration)
        {
            float fraction = (Time.time - startTime) / moveDuration;
            obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, fraction);
            yield return null;
        }

        // �̷�2 ��ġ�� �̵�
        obstacle.transform.position = mirror2.position;

        obstacleCollider.enabled = true;
    }
}
