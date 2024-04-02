using UnityEngine;
using System;

public class MirrorEvent : MonoBehaviour
{
    // 미러 이벤트 정의
    public static event Action<GameObject> OnObstacleDrop;

    // 미러1에서 장애물이 떨어졌을 때 호출되는 메서드
    public static void ObstacleDropped(GameObject obstacle)
    {
        // 이벤트 호출
        OnObstacleDrop?.Invoke(obstacle);
    }
}
