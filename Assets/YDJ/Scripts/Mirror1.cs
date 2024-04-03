using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror1 : MonoBehaviour
{
    [SerializeField] Transform mirror2; // 미러2의 위치를 참조하는 변수
    [SerializeField] float zOffset; // Z 위치를 조절할 오프셋 변수
    [SerializeField] float delay; // 장애물 제거 후 재생성까지의 대기 시간
    new Collider collider; // 장애물의 콜라이더 컴포넌트 참조

    void Update()
    {
        // 거울1 거울2 위치 동기화
        Vector3 newPosition = transform.position;
        newPosition.z += zOffset;
        mirror2.position = newPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) // 충돌한 오브젝트의 태그가 "Obstacle"인 경우에만 실행
        {
            // 미러1에서 장애물이 떨어졌을 때 이벤트 호출
            MirrorEvent.ObstacleDropped(other.gameObject);

            collider = other.GetComponent<Collider>(); // 충돌한 오브젝트의 콜라이더 참조
            StartCoroutine(RemoveObstacleDelayed(other.gameObject)); // 지정된 시간 후에 장애물 제거
        }
    }

    IEnumerator RemoveObstacleDelayed(GameObject obstacle)
    {
        Destroy(collider); // 장애물의 콜라이더 제거
        yield return new WaitForSeconds(delay); // 지정된 시간(초)만큼 대기

        RespawnObstacle(obstacle);
        //RemoveObstacle(obstacle); // 장애물 제거 함수 호출
    }

    //void RemoveObstacle(GameObject obstacle)
    //{
    //    Destroy(obstacle); // 충돌한 장애물 제거
    //    RespawnObstacle(obstacle); // 장애물 콜라이더를 다시 생성하기 위해 함수 호출
    //}

    void RespawnObstacle(GameObject obstacle)
    {
        obstacle.transform.position = mirror2.position; // 미러2 위치로 장애물 이동
        collider = obstacle.AddComponent<BoxCollider>(); // 박스 콜라이더를 추가하여 활성화
    }
}
