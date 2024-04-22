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

    public UnityEvent OnChangePlayer;

    [SerializeField] private bool isPlayer1Active = true;
    public bool IsPlayer1Active { get { return isPlayer1Active; } set { isPlayer1Active = value; } }

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
        if (isPlayer1Active && player2Camera.Follow !=null )
        {
            Debug.Log(player1Camera.Priority);
            Debug.Log("p2On");
            OnChangePlayer?.Invoke();
            player1Camera.Priority = 0;
            player2Camera.Priority = 10;
            
            isPlayer1Active = false;
        }
        // �׷��� ������ �÷��̾� 1�� ī�޶�� ��ȯ�մϴ�.
        else if(!isPlayer1Active)
        {
            Debug.Log("p1On");
            OnChangePlayer?.Invoke();
            player1Camera.Priority = 10;
            Debug.Log(player1Camera.Priority);
            Debug.Log(player1Camera.name);
            while ( player2Camera.Priority == 10 )
            {
                Debug.Log("�� �ȹٲ��");
                player2Camera.Priority = 0;
            }
          
            isPlayer1Active = true;
            Debug.Log("�� �ȹٲ��1");
            Debug.Log("�� �ȹٲ��2");
            Debug.Log("�� �ȹٲ��3");
            Debug.Log("�� �ȹٲ��4");
            Debug.Log("�� �ȹٲ��5");
            Debug.Log("�� �ȹٲ��6");
            Debug.Log("�� �ȹٲ��7");
        }
        else
        {
            Debug.Log("?");
        }

        
    }
}
