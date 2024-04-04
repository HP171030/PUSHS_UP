using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Vector3 mirrorEnterDirection; // �ſ￡ ������ ������ �����ϴ� ����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mirror")) // �ſ￡ ����� ��
        {
            // �ſ��� Transform ������Ʈ�� ������
            Transform mirrorTransform = other.GetComponent<Transform>();

            // �ſ��� ǥ���� ��Ÿ���� ��� ���� ����
            Vector3 mirrorNormal = mirrorTransform.forward;

            // �ſ��� ǥ��� ��ֹ��� �浹 ������ ���
            Vector3 collisionPoint = other.ClosestPoint(transform.position);

            // �ſ� ǥ��� �浹 ������ �����ϴ� ���͸� ���
            Vector3 collisionVector = collisionPoint - mirrorTransform.position;

            // �ſ��� ���� ���Ϳ� �����Ͽ� ���� ������ ���
            float dotProduct = Vector3.Dot(collisionVector, mirrorNormal);
            mirrorEnterDirection = dotProduct > 0 ? mirrorNormal : -mirrorNormal;

        }
    }

    // �ſ￡ ������ ������ ��ȯ�ϴ� �޼���
    public Vector3 GetMirrorEnterDirection()
    {
        Debug.Log($"mirrorEnterDirection  {mirrorEnterDirection}");
        return mirrorEnterDirection; 
    }
}
