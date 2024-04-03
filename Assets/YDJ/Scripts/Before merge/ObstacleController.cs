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
                // �̵��� �Ϸ�Ǹ� ���� ��ġ�� �缳���ϰ� �̵� ���¸� �����մϴ�.
                isMoving = false;
                transform.position = initialPosition;
            }
        }
    }

    public void MoveUp()
    {
        // �̵��� �����ϱ� ���� ȣ��� �޼����Դϴ�.
        isMoving = true;
    }
}
