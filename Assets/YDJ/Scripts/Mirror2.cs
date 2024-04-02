using UnityEngine;

public class Mirror2 : MonoBehaviour
{
    [SerializeField] float moveDuration = 2f; // ��ֹ� �̵� ���� �ð�
    [SerializeField] float moveHeight = 5f; // ��ֹ��� �̵��� ����
    [SerializeField] float groundHeight = 0.5f; // ������ ����

    Vector3 targetPosition; // ��ֹ��� �̵��� ��ǥ ��ġ
    Vector3 startPosition; // ��ֹ��� �ʱ� ��ġ
    float startTime; // �̵� �ִϸ��̼� ���� �ð�
    bool isMoving = false; // ��ֹ� �̵� �� ����

    void OnEnable()
    {
        // �̷�1���� �������� ��ֹ� �̺�Ʈ ������ ���
        MirrorEvent.OnObstacleDrop += HandleObstacleDrop;
    }

    void OnDisable()
    {
        // �̷�1���� �������� ��ֹ� �̺�Ʈ ������ ����
        MirrorEvent.OnObstacleDrop -= HandleObstacleDrop;
    }

    // �̷�1���� �������� ��ֹ� �̺�Ʈ ó��
    void HandleObstacleDrop(GameObject obstacle)
    {
        // �ʱ� ��ġ ����
        startPosition = transform.position;

        // ��ǥ ��ġ ���� (���� ��ġ���� ���� �̵�)
        targetPosition = startPosition + Vector3.up * moveHeight;

        // �̵� �ִϸ��̼� ���� �ð� ����
        startTime = Time.time;

        // ��ֹ� �̵� ���·� ����
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            // �̵� ���൵ ���
            float progress = (Time.time - startTime) / moveDuration;

            // �����Ͽ� ��ġ �̵�
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

            // ��ֹ��� y ��ġ�� ���� ���̿� �����ϸ� �̵��� ����ϴ�.
            if (transform.position.y <= groundHeight)
            {
                isMoving = false; // �̵� �Ϸ� �� ���� ����
            }
        }
    }
}
