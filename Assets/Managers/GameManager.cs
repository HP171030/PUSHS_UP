using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    private int boomCount = 3;
    private int stepCount = 0;

    public CinemachineVirtualCamera cine;
    public UnityAction stepUpdate;




    public int boomAction { get { return boomCount; } set {  boomCount = value; Debug.Log(boomCount); } }
    public int StepAction { get { return stepCount; } set { stepCount = value; stepUpdate?.Invoke(); } }



    public void ShakeCam()
    {
        if(cine != null )
        {
            CinemachineBasicMultiChannelPerlin perlin = cine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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
