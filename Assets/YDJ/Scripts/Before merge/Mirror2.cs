using UnityEngine;

public class Mirror2 : MonoBehaviour
{
    [SerializeField] float moveDuration = 2f; // 장애물 이동 지속 시간
    [SerializeField] float moveHeight = 5f; // 장애물이 이동할 높이
    [SerializeField] float groundHeight = 0.5f; // 지면의 높이

    Vector3 targetPosition; // 장애물이 이동할 목표 위치
    Vector3 startPosition; // 장애물의 초기 위치
    float startTime; // 이동 애니메이션 시작 시간
    bool isMoving = false; // 장애물 이동 중 여부

    void OnEnable()
    {
        // 미러1에서 떨어지는 장애물 이벤트 리스너 등록
        MirrorEvent.OnObstacleDrop += HandleObstacleDrop;
    }

    void OnDisable()
    {
        // 미러1에서 떨어지는 장애물 이벤트 리스너 해제
        MirrorEvent.OnObstacleDrop -= HandleObstacleDrop;
    }

    // 미러1에서 떨어지는 장애물 이벤트 처리
    void HandleObstacleDrop(GameObject obstacle)
    {
        // 초기 위치 설정
        startPosition = transform.position;

        // 목표 위치 설정 (현재 위치에서 위로 이동)
        targetPosition = startPosition + Vector3.up * moveHeight;

        // 이동 애니메이션 시작 시간 설정
        startTime = Time.time;

        // 장애물 이동 상태로 설정
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            // 이동 진행도 계산
            float progress = (Time.time - startTime) / moveDuration;

            // 보간하여 위치 이동
            transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

            // 장애물의 y 위치가 지면 높이에 도달하면 이동을 멈춥니다.
            if (transform.position.y <= groundHeight)
            {
                isMoving = false; // 이동 완료 후 상태 변경
            }
        }
    }
}
