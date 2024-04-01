using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float moveDistance = 3.0f;
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private bool isMoving = false;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition + Vector3.up * moveDistance;
    }

    void Update()
    {
        if (isMoving)
        {
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (transform.position == targetPosition)
            {
                // 이동이 완료되면 원래 위치로 재설정하고 이동 상태를 중지합니다.
                isMoving = false;
                transform.position = initialPosition;
            }
        }
    }

    public void MoveUp()
    {
        // 이동을 시작하기 위해 호출될 메서드입니다.
        isMoving = true;
    }
}
