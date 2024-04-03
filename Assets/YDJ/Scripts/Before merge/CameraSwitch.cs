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
        // �ʱ⿡�� �÷��̾� 1�� ī�޶� Ȱ��ȭ�˴ϴ�.
        player1Camera.Priority = 10;
        player2Camera.Priority = 0;
    }

    private void OnChange(InputValue value)
    {
        Debug.Log("ī�޶� ����ġ");
        Change();
    }

    private void Change()
    {

        // �÷��̾� 1�� ī�޶� Ȱ��ȭ�Ǿ� ������ �÷��̾� 2�� ī�޶�� ��ȯ�մϴ�.
        if (isPlayer1Active)
        {

            player1Camera.Priority = 0;
            player2Camera.Priority = 10;
            isPlayer1Active = false;
        }
        // �׷��� ������ �÷��̾� 1�� ī�޶�� ��ȯ�մϴ�.
        else
        {
            player1Camera.Priority = 10;
            player2Camera.Priority = 0;
            isPlayer1Active = true;
        }
    }
}
