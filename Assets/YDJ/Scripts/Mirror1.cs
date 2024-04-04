using UnityEngine;
using System.Collections;

public class Mirror1 : MonoBehaviour
{
    [SerializeField] Transform mirror2; // �ſ�2�� Transform�� �����ϴ� ����
    [SerializeField] float Mirror2OffsetX = 20f; // Y�� ������
    [SerializeField] float ObstacleOffsetY = -5f; // Y�� ������
    [SerializeField] float ObstacleOffsetX;
    [SerializeField] float ObstacleOffsetZ;
    [SerializeField] float moveDuration = 2f; // �̵��ϴ� �ð�

    [SerializeField] YHP_PlayerController YDJ_PlayerController;
    //[SerializeField] Obstacle obstacleScript;

    public bool wallChecker;
    public bool WallChecker { get { return wallChecker; } }

    public bool obstacleChecker;
    public bool ObstacleChecker { get { return obstacleChecker; } }


    private Vector3 forwardDirection;

    public void SetForwardDirection(Vector3 direction)
    {
        forwardDirection = direction;

        // �ſ��� ���� �پ��ִ� ���
        if (wallChecker)
        {
            Debug.Log($"forwardDirection{forwardDirection}");
            // �ſ￡ �� ������ ������
            //Vector3 mirrorEnterDirection = obstacleScript.GetMirrorEnterDirection();

            if (forwardDirection.x > 0) // ������
            {
                transform.localRotation = Quaternion.Euler(0, 0, 90);

            }
            else if (forwardDirection.x < 0) // ����
            {
                transform.localRotation = Quaternion.Euler(0, 0, -90);
            }
            else if (forwardDirection.y > 0) // ��
            {
                transform.localRotation = Quaternion.Euler(90, 0, 0);
            }
            else // �Ʒ�
            {
                transform.localRotation = Quaternion.Euler(-90, 0, 0);
            }
        }
        else
        {
            // ���� �پ����� ���� ��� �⺻ �����̼��� �����մϴ�.
            transform.localRotation = Quaternion.identity;
        }
    }

    void Update()
    {
        if (!wallChecker)
        {
            Vector3 newPosition = transform.position;
            newPosition.z += Mirror2OffsetX;
            mirror2.position = newPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Debug.Log("���� ����");
            wallChecker = true;

            Vector3 newPosition = transform.position;
            newPosition.z += Mirror2OffsetX;
            mirror2.position = newPosition;
            obstacleChecker = false;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Debug.Log("�ſ￡ ��ֹ� ����");
            obstacleChecker = true;
            if (!YDJ_PlayerController.MirrorHolding)
            {
                StartCoroutine(RespawnObstacle(other.gameObject));
            }
        }
        else
        {
            obstacleChecker = false;
        }


    }



    IEnumerator RespawnObstacle(GameObject obstacle)
    {
        Collider obstacleCollider = obstacle.GetComponent<Collider>();

        if (!wallChecker)
            obstacleCollider.enabled = false;
        else
        {
            Rigidbody rb = obstacle.GetComponent<Rigidbody>();
            rb.isKinematic = true;


        }


        yield return new WaitForSeconds(2f);

        Vector3 startPosition;
        if (wallChecker)
        {

            Debug.Log($"forwardDirection{forwardDirection}");
            // �ſ￡ �� ������ ������
            //Vector3 mirrorEnterDirection = obstacleScript.GetMirrorEnterDirection();

            if (forwardDirection.x > 0) // ���������� ����
            {
                // ���� ��ġ ���: �ſ￡�� ��ֹ��� ���� ����� �ݴ�� �̵�
                startPosition = mirror2.position + new Vector3(ObstacleOffsetX, 0f, 0f);
                Debug.Log("��ֹ��� �����ʿ��� ���Խ��ϴ�.");
            }
            else if (forwardDirection.x < 0) // �������� ����
            {
                startPosition = mirror2.position - new Vector3(ObstacleOffsetX, 0f, 0f);
                Debug.Log("��ֹ��� ���ʿ��� ���Խ��ϴ�.");
            }
            else if (forwardDirection.y > 0) // ���� ����
            {
                startPosition = mirror2.position + new Vector3(0, 0f, ObstacleOffsetZ);
                Debug.Log("��ֹ��� ������ ���Խ��ϴ�.");
            }
            else // �Ʒ��� ����
            {
                startPosition = mirror2.position - new Vector3(0, 0f, ObstacleOffsetZ);
                Debug.Log("��ֹ��� �Ʒ����� ���Խ��ϴ�.");
            }
        }

        else
        {
            startPosition = mirror2.position + new Vector3(0f, ObstacleOffsetY, 0f);
        }

        Vector3 endPosition = mirror2.position;
        float startTime = Time.time;
        while (Time.time - startTime < moveDuration)
        {
            float fraction = (Time.time - startTime) / moveDuration;
            obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, fraction);
            yield return null;
        }

        obstacle.transform.position = mirror2.position;
        obstacleCollider.enabled = true;
    }
}
