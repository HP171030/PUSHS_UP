using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Vector3 mirrorEnterDirection; // 거울에 진입한 방향을 저장하는 변수

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mirror")) // 거울에 닿았을 때
        {
            // 거울의 Transform 컴포넌트를 가져옴
            Transform mirrorTransform = other.GetComponent<Transform>();

            // 거울의 표면을 나타내는 평면 법선 벡터
            Vector3 mirrorNormal = mirrorTransform.forward;

            // 거울의 표면과 장애물의 충돌 지점을 계산
            Vector3 collisionPoint = other.ClosestPoint(transform.position);

            // 거울 표면과 충돌 지점을 연결하는 벡터를 계산
            Vector3 collisionVector = collisionPoint - mirrorTransform.position;

            // 거울의 법선 벡터와 내적하여 진입 방향을 계산
            float dotProduct = Vector3.Dot(collisionVector, mirrorNormal);
            mirrorEnterDirection = dotProduct > 0 ? mirrorNormal : -mirrorNormal;

        }
    }

    // 거울에 진입한 방향을 반환하는 메서드
    public Vector3 GetMirrorEnterDirection()
    {
        Debug.Log($"mirrorEnterDirection  {mirrorEnterDirection}");
        return mirrorEnterDirection; 
    }
}
