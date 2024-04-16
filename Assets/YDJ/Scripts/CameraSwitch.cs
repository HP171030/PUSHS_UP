using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] public CinemachineVirtualCamera player1Camera;
    [SerializeField] public CinemachineVirtualCamera player2Camera;

    [SerializeField] private bool isPlayer1Active = true;
    public bool IsPlayer1Active { get { return isPlayer1Active; } set { isPlayer1Active = value; } }

    void Start()
    {
        // �ʱ⿡�� �÷��̾� 1�� ī�޶� Ȱ��ȭ�˴ϴ�.
        //player1Camera.Priority = 10;
       // player2Camera.Priority = 0;
    }

    private void OnChange(InputValue value)
    {
        if ( player2Camera.Follow != null&& !Manager.game.isEnter )
        {
            Debug.Log(player2Camera.Follow);
            Change();
        }
        else
        {
            Debug.Log("isNotChange");
        }
        
        
    }

    public void Change()
    {


        if ( player2Camera.Follow == null || player1Camera.Follow == null )
        {
            Debug.Log("isNotChange");
        }
        else
        {
            // �÷��̾� 1�� ī�޶� Ȱ��ȭ�Ǿ� ������ �÷��̾� 2�� ī�޶�� ��ȯ�մϴ�.
            if ( isPlayer1Active )
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
}
