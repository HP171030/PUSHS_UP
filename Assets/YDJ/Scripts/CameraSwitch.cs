using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] public CinemachineVirtualCamera player1Camera;
    [SerializeField] public CinemachineVirtualCamera player2Camera;

    [SerializeField] bool isPlayer1Active;
    public bool IsPlayer1Active { get { return isPlayer1Active; } set { IconSwitch(value); isPlayer1Active = value; } }

    void Start()
    {
        if ( player1Camera.Follow == null )
        {
            Change();
            Manager.game.isEnter = true;
        }
        else
        {

        }

    }
    void IconSwitch(bool isP1)
    {
        Manager.ui.ChangeP1P2ICon(isP1);

    }
    private void OnChange(InputValue value)
    {
        if (!Manager.game.isEnter )
        {
            Change();
        }
        else
        {
            Debug.Log("isNotChange");
        }
    }

    public void Change()
    {
        if( player1Camera.Follow == null || player2Camera.Follow == null )
        {
            return;
        }
        if (isPlayer1Active)
        {
            Debug.Log(player1Camera.Priority);
            Debug.Log("p2On");
            player1Camera.Priority = 0;
            player2Camera.Priority = 10;
            
            IsPlayer1Active = false;
            Manager.game.boomUpdate();
        }
        // 그렇지 않으면 플레이어 1의 카메라로 전환합니다.
        else if(!isPlayer1Active)
        {
            Debug.Log("p1On");
            player1Camera.Priority = 10;
            Debug.Log(player1Camera.Priority);
            Debug.Log(player1Camera.name);
            while ( player2Camera.Priority == 10 )
            {

                player2Camera.Priority = 0;
            }
          
            IsPlayer1Active = true;

        }
        else
        {
            Debug.Log("?");
        }

        
    }
}
