using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Mirror1 : MonoBehaviour
{
    [SerializeField] Transform mirror2; // �ſ�2�� Transform�� �����ϴ� ����
    [SerializeField] float Mirror2OffsetZ = 20f; // Y�� ������
    [SerializeField] float ObstacleOffsetY = -5f; // Y�� ������
    [SerializeField] float moveDuration = 2f; // �̵��ϴ� �ð�

    //[SerializeField] Holder holder;
    [SerializeField] A_PlayerController a_PlayerController;
    

    float wallMirror2OffsetX;
    float wallMirror2Offsety;
    private void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (a_PlayerController.WallMirrorChecker) //�ſ�1�� ���϶� (����)�� �ٿ��� �� �ſ�2 ��ġ ����
        {
            if (input.x > 0) // ������
            {
                wallMirror2OffsetX = -1;
                //mirror2.position = mirror2.position + new Vector3(-1, 0, 0);
            }
            else if (input.x < 0) // ����
            {
                wallMirror2OffsetX = 1;
                //mirror2.position = mirror2.position + new Vector3(1, 0, 0);
            }
            else if (input.y > 0) // ��
            {
                wallMirror2Offsety = -1;
                //mirror2.position = mirror2.position + new Vector3(0, 0, -1);
            }
            else if (input.y < 0) // ��
            {
                wallMirror2Offsety = 1;
                //mirror2.position = mirror2.position + new Vector3(0, 0, 1);
            }
        }

    }





    void Update()
    {
        //if(holder.WallLader())
        //{
        //    Debug.Log("WallLader");
        //    mirror2.position = mirror2.position + new Vector3(wallMirror2OffsetX, 0, wallMirror2Offsety);
        //}
         //���� �پ����� �ʴٸ�
        {
            Vector3 newPosition = transform.position;
            newPosition.z += Mirror2OffsetZ;
            mirror2.position = newPosition;
            if (a_PlayerController.WallMirrorChecker)
            {
                Debug.Log("WallLader");
                mirror2.position = mirror2.position + new Vector3(wallMirror2OffsetX, 0, wallMirror2Offsety);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("�ſ� Ʈ���� ����");
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
