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

    [SerializeField] GameObject Mirror1WallPoint;
    [SerializeField] YHP_PlayerController YDJ_PlayerController;
    //[SerializeField] Obstacle obstacleScript;

    public bool wallChecker;
    public bool WallChecker { get { return wallChecker; } }

    public bool obstacleChecker;
    public bool ObstacleChecker { get { return obstacleChecker; } }

    public bool mirror1InObstacleChecker;
    public bool Mirror1InObstacleChecker { get { return mirror1InObstacleChecker; } }


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
            newPosition.y = 0;
            mirror2.position = newPosition;
            obstacleChecker = false;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            //if (Vector3.Distance(transform.position, other.transform.position) < 0.5f)
            //{
            //    Debug.Log("�ſ�� ��ֹ��� �Ÿ��� 1 �̸��Դϴ�.");
            //    return;

            //}

                Debug.Log("�ſ￡ ��ֹ� ����");
            //obstacleChecker = true;
            if (!YDJ_PlayerController.MirrorHolding)
            {
                StartCoroutine(MirrorInObstacle(other.gameObject));
            }
        }
        else
        {
            obstacleChecker = false;
        }



    }


    IEnumerator MirrorInObstacle(GameObject obstacle)
    {
        Rigidbody obstacleRigidbody = obstacle.GetComponent<Rigidbody>();
        Collider obstacleCollider = obstacle.GetComponent<Collider>();

        Vector3 endPosition;
        float time = 0;
        float targetTime = 1f;
        Vector3 startPosition = transform.position;
        //Vector3 startPosition = Mirror1WallPoint.transform.position;
        startPosition.y = 0;

        if (wallChecker) // �� �ſ�1
        {
            //gameObject.layer == LayerMask.NameToLayer("Wall")
            //Debug.Log("�� �ſ�1�� ��ֹ� ����");
            //CollisionManager collisionManager = GetComponent<CollisionManager>();
            //collisionManager.IgnoreCollision("Obstacle", "Wall", false);
            //collisionManager.IgnoreCollision("Player", "Obstacle", false);


            //obstacleRigidbody.isKinematic = true;

            if (forwardDirection.x > 0) // ���������� ����
            {
                // ���� ��ġ ���: �ſ￡�� ��ֹ��� ���� ����� �ݴ�� �̵�
                endPosition = transform.position + new Vector3(ObstacleOffsetX, 0f, 0f);
                Debug.Log("��ֹ��� �����ʿ��� ���Խ��ϴ�.");
            }
            else if (forwardDirection.x < 0) // �������� ����
            {
                endPosition = transform.position - new Vector3(ObstacleOffsetX, 0f, 0f);
                Debug.Log("��ֹ��� ���ʿ��� ���Խ��ϴ�.");
            }
            else if (forwardDirection.y > 0) // ���� ����
            {
                endPosition = transform.position + new Vector3(0, 0f, ObstacleOffsetZ);
                Debug.Log("��ֹ��� ������ ���Խ��ϴ�.");
            }
            else // �Ʒ��� ����
            {
                endPosition = transform.position - new Vector3(0, 0f, ObstacleOffsetZ);
                Debug.Log("��ֹ��� �Ʒ����� ���Խ��ϴ�.");
            }

            while (time < targetTime)
            {
                time += Time.deltaTime;
                Debug.Log($"obstacle in{obstacle.transform.position}");
                obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, time / targetTime);
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            StartCoroutine(MirrorOutObstacle(obstacle));
        }




        else // �ٴ� �ſ�1
        {
            endPosition = transform.position + new Vector3(0f, ObstacleOffsetY, 0f);

            while (time < targetTime)
            {
                time += Time.deltaTime;
                Debug.Log($"obstacle in{obstacle.transform.position}");
                obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, time / targetTime);
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            StartCoroutine(MirrorOutObstacle(obstacle));
        }











    }


    IEnumerator MirrorOutObstacle(GameObject obstacle)
    {
        Vector3 endPosition = mirror2.position;
        float time = 0;
        float targetTime = 1f;
        Vector3 startPosition;

        if (wallChecker)
        {

            Debug.Log($"forwardDirection{forwardDirection}");
            // �ſ￡ �� ������ ������
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



        while (time < targetTime)
        {
            time += Time.deltaTime;
            Debug.Log($"obstacle out{obstacle.transform.position}");
            obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, time/targetTime);
            yield return null;
        }
    }    
}


