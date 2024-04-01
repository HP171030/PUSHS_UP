using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera player1Camera;
    [SerializeField] CinemachineVirtualCamera player2Camera;

    private bool isPlayer1Active = true;
    public bool IsPlayer1Active {  get { return isPlayer1Active; } }

    void Start()
    {
        // 초기에는 플레이어 1의 카메라가 활성화됩니다.
        player1Camera.Priority = 10;
        player2Camera.Priority = 0;
    }

    private void OnChange(InputValue value)
    {
        Debug.Log("카메라 스위치");
        Change();
    }

    private void Change()
    {

        // 플레이어 1의 카메라가 활성화되어 있으면 플레이어 2의 카메라로 전환합니다.
        if (isPlayer1Active)
        {

            player1Camera.Priority = 0;
            player2Camera.Priority = 10;
            isPlayer1Active = false;
        }
        // 그렇지 않으면 플레이어 1의 카메라로 전환합니다.
        else
        {
            player1Camera.Priority = 10;
            player2Camera.Priority = 0;
            isPlayer1Active = true;
        }
    }
}
