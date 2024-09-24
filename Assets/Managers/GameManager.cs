using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    private int boomCount = 3;
    private int stepCount = 0;
    private int patternCounter = 0;
    public int doorSwitch;
    public bool bossScene;
    public bool mainScene;
    
    public CinemachineVirtualCamera[] cines;
    public YHP_PlayerController playerController;
    public Player2Controller player2Controller;
    public UnityAction stepUpdate;
    public UnityAction boomUpdate;


    [SerializeField] AudioClip player2DeadSound;

    public GameObject restartButton;

    public int clearValue;
    public bool isEnter = false;

    public bool gameOver = false;


    public int boomAction { get { return boomCount; } set {  boomCount = value; boomUpdate?.Invoke() ; } }
    public int StepAction { get { return stepCount; } set { stepCount = value; stepUpdate?.Invoke(); } }

    public int patternStep { get { return patternCounter; } set { patternCounter = value;} }

    public void GameOver()
    {
        PlayerControllStop();
        gameOver = true;
        StartCoroutine(gameOverRoutine());
        Manager.sound.PlaySFX(player2DeadSound);
    }

    IEnumerator gameOverRoutine()
    {
        restartButton.SetActive(true);
        player2Controller.animator.Play("GameOver");
        yield return null;
        Time.timeScale = 0.3f;
        
    }
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
        if ( !gameOver )
        {
        Debug.Log("inputOn");
        playerController.inputKey = true;
        player2Controller.inputKey = true;  

        }
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