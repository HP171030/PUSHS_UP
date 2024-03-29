using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror1 : MonoBehaviour
{

    private Vector3 spawnPosition;
    [SerializeField] Transform spawnMirror;




    void Update()
    {
        Vector3 newPosition = transform.position;
        newPosition.x += 13f;
        spawnMirror.position = newPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) // �浹�� ������Ʈ�� �±װ� "Obstacle"�� ��쿡�� ����
        {
            RemoveObstacle(other.gameObject);
        }
    }

    void RemoveObstacle(GameObject obstacle)
    {
        // ��ֹ� ����
        Destroy(obstacle);
        // �ٸ� �÷��̾ �ִ� ���� ��ֹ��� �ٽ� ��ȯ
        RespawnObstacle(obstacle);
    }

    void RespawnObstacle(GameObject obstacle)
    {
        spawnPosition = spawnMirror.position;
        // ��ֹ��� �ʱ� ��ġ�� �ٽ� ����
        Instantiate(obstacle, spawnPosition, Quaternion.identity);
    }
}
