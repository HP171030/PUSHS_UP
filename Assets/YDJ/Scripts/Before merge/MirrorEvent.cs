using UnityEngine;
using System;

public class MirrorEvent : MonoBehaviour
{
    // �̷� �̺�Ʈ ����
    public static event Action<GameObject> OnObstacleDrop;

    // �̷�1���� ��ֹ��� �������� �� ȣ��Ǵ� �޼���
    public static void ObstacleDropped(GameObject obstacle)
    {
        // �̺�Ʈ ȣ��
        OnObstacleDrop?.Invoke(obstacle);
    }
}
