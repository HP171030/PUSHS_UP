using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror1 : MonoBehaviour
{
    [SerializeField] Transform mirror2; // �̷�2�� ��ġ�� �����ϴ� ����
    [SerializeField] float zOffset; // Z ��ġ�� ������ ������ ����
    [SerializeField] float delay; // ��ֹ� ���� �� ����������� ��� �ð�
    new Collider collider; // ��ֹ��� �ݶ��̴� ������Ʈ ����

    void Update()
    {
        // �ſ�1 �ſ�2 ��ġ ����ȭ
        Vector3 newPosition = transform.position;
        newPosition.z += zOffset;
        mirror2.position = newPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) // �浹�� ������Ʈ�� �±װ� "Obstacle"�� ��쿡�� ����
        {
            // �̷�1���� ��ֹ��� �������� �� �̺�Ʈ ȣ��
            MirrorEvent.ObstacleDropped(other.gameObject);

            collider = other.GetComponent<Collider>(); // �浹�� ������Ʈ�� �ݶ��̴� ����
            StartCoroutine(RemoveObstacleDelayed(other.gameObject)); // ������ �ð� �Ŀ� ��ֹ� ����
        }
    }

    IEnumerator RemoveObstacleDelayed(GameObject obstacle)
    {
        Destroy(collider); // ��ֹ��� �ݶ��̴� ����
        yield return new WaitForSeconds(delay); // ������ �ð�(��)��ŭ ���

        RespawnObstacle(obstacle);
        //RemoveObstacle(obstacle); // ��ֹ� ���� �Լ� ȣ��
    }

    //void RemoveObstacle(GameObject obstacle)
    //{
    //    Destroy(obstacle); // �浹�� ��ֹ� ����
    //    RespawnObstacle(obstacle); // ��ֹ� �ݶ��̴��� �ٽ� �����ϱ� ���� �Լ� ȣ��
    //}

    void RespawnObstacle(GameObject obstacle)
    {
        obstacle.transform.position = mirror2.position; // �̷�2 ��ġ�� ��ֹ� �̵�
        collider = obstacle.AddComponent<BoxCollider>(); // �ڽ� �ݶ��̴��� �߰��Ͽ� Ȱ��ȭ
    }
}
