using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    private int boomCount = 3;
    private int stepCount = 0;
    public int doorSwitch;

    public CinemachineVirtualCamera[] cines;
    public PlayerController playerController;
    public Player2Controller player2Controller;
    public UnityAction stepUpdate;

    public int clearValue;
    public bool isEnter = false;


    public int boomAction { get { return boomCount; } set {  boomCount = value; Debug.Log(boomCount); } }
    public int StepAction { get { return stepCount; } set { stepCount = value; stepUpdate?.Invoke(); } }


    public void PlayerControllStop()
    {
        Debug.Log("inputOff");
        playerController.inputKey = false;
        playerController.moveDir = Vector3.zero;
        player2Controller.inputKey = false;
        player2Controller.moveDir = Vector3.zero;

    }
    public void PlayerControllerOn()
    {
        Debug.Log("inputOn");
        playerController.inputKey = true;
        player2Controller.inputKey = true;  
    }
    public void ShakeCam()
    {
        if ( cines.Length > 0 )
        {
            foreach ( CinemachineVirtualCamera c in cines )
            {
                CinemachineBasicMultiChannelPerlin perlin = c.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                if ( perlin != null )
                {
                    perlin.m_AmplitudeGain = 2;
                    perlin.m_FrequencyGain = 2;
                    StartCoroutine(DampenShake(perlin));

                }
                else
                {
                    Debug.LogError("CinemachineBasicMultiChannelPerlin not found.");
                }
            }
        }
        else
        {
            Debug.LogError("CinemachineVirtualCamera not assigned.");
        }
        
    }
    private IEnumerator DampenShake( CinemachineBasicMultiChannelPerlin perlin )
    {
        yield return new WaitForSeconds(1f);
        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 0;
    }
}
