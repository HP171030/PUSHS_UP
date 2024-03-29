using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror1 : MonoBehaviour
{

    private Vector3 spawnPosition;
    [SerializeField] Transform spawnMirror;




    void Update()
    {
        Vector3 newPosition = transform.position;
        newPosition.x += 13f;
        spawnMirror.position = newPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) // 충돌한 오브젝트의 태그가 "Obstacle"인 경우에만 실행
        {
            RemoveObstacle(other.gameObject);
        }
    }

    void RemoveObstacle(GameObject obstacle)
    {
        // 장애물 제거
        Destroy(obstacle);
        // 다른 플레이어가 있는 땅에 장애물을 다시 소환
        RespawnObstacle(obstacle);
    }

    void RespawnObstacle(GameObject obstacle)
    {
        spawnPosition = spawnMirror.position;
        // 장애물을 초기 위치에 다시 생성
        Instantiate(obstacle, spawnPosition, Quaternion.identity);
    }
}
