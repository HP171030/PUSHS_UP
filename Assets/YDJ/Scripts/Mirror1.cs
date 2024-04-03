using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Mirror1 : MonoBehaviour
{
    [SerializeField] Transform mirror2; // 거울2의 Transform을 참조하는 변수
    [SerializeField] float Mirror2OffsetZ = 20f; // Y값 오프셋
    [SerializeField] float ObstacleOffsetY = -5f; // Y값 오프셋
    [SerializeField] float moveDuration = 2f; // 이동하는 시간

    //[SerializeField] Holder holder;
    [SerializeField] A_PlayerController a_PlayerController;
    

    float wallMirror2OffsetX;
    float wallMirror2Offsety;
    private void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (a_PlayerController.WallMirrorChecker) //거울1이 벽일때 (방향)에 붙였을 때 거울2 위치 설정
        {
            if (input.x > 0) // 오른쪽
            {
                wallMirror2OffsetX = -1;
                //mirror2.position = mirror2.position + new Vector3(-1, 0, 0);
            }
            else if (input.x < 0) // 왼쪽
            {
                wallMirror2OffsetX = 1;
                //mirror2.position = mirror2.position + new Vector3(1, 0, 0);
            }
            else if (input.y > 0) // 앞
            {
                wallMirror2Offsety = -1;
                //mirror2.position = mirror2.position + new Vector3(0, 0, -1);
            }
            else if (input.y < 0) // 뒤
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
         //벽에 붙어있지 않다면
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
        Debug.Log("거울 트리거 닿음");
        //if (other.CompareTag("Obstacle"))
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            // 코루틴을 시작하여 장애물을 다시 생성
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

        // 거울2 아래로 이동
        Vector3 startPosition = mirror2.position + new Vector3(0f, ObstacleOffsetY, 0f);
        Vector3 endPosition = obstacle.transform.position = mirror2.position;
        float startTime = Time.time;
        while (Time.time - startTime < moveDuration)
        {
            float fraction = (Time.time - startTime) / moveDuration;
            obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, fraction);
            yield return null;
        }

        // 미러2 위치로 이동
        obstacle.transform.position = mirror2.position;

        obstacleCollider.enabled = true;
    }
}
