using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] public CinemachineVirtualCamera player1Camera;
    [SerializeField] public CinemachineVirtualCamera player2Camera;

    [SerializeField] bool isPlayer1Active;
    public bool IsPlayer1Active { get { return isPlayer1Active; } set { IconSwitch(value); Debug.Log($"playerActive Change {value}"); isPlayer1Active = value; } }

    public void Init()
    {


        if ( player1Camera == null || player1Camera.Follow == null ||
             player2Camera == null || player2Camera.Follow )
        {
            /*if ( Manager.game.MainScene )
            {
                Debug.Log("cam return");
                return;
            }*/
            Debug.Log("You have character only 2P");
            InitCam();
            Manager.game.isEnter = true;
        }
        else
        {
            Debug.Log("Set Cam");
            player1Camera.Priority = 10;
            player2Camera.Priority = 0;

            IsPlayer1Active = true;
        }

    }
    void IconSwitch( bool isP1 )
    {
        Manager.ui.ChangeP1P2ICon(isP1);

    }
    private void OnChange( InputValue value )
    {
        Debug.Log("Try Change");
        if ( !Manager.game.isEnter )
        {
            TryChange();
        }
        else
        {
            Debug.Log("isNotChange");
        }
    }

    public void TryChange()
    {
        if ( player1Camera.Follow == null || player2Camera.Follow == null )
        {
            Debug.Log("Try");
            return;
        }
        CharacterChange();
    }
    public void InitCam()
    {
        if ( player1Camera == null || player1Camera.Follow == null )
        {
            IsPlayer1Active = false;
            if(player1Camera != null)
            player1Camera.Priority = 0;
            player2Camera.Priority = 10;
        }
        else
        {
            IsPlayer1Active = true;
            player1Camera.Priority = 10;
            if ( player2Camera != null )
                player2Camera.Priority = 0;
        }
    }
    public void CharacterChange()
    {
        Debug.Log(IsPlayer1Active);

        if ( IsPlayer1Active )
        {
            Debug.Log(player1Camera.Priority);
            Debug.Log("p2On");
            player1Camera.Priority = 0;
            player2Camera.Priority = 10;

            IsPlayer1Active = false;

            Manager.game.boomUpdate?.Invoke();
        }
        else
        {
            Debug.Log("p1On");
            player1Camera.Priority = 10;
            player2Camera.Priority = 0;

            IsPlayer1Active = true;

        }

    }
}
