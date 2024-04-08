using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private int boomCount = 3;
    private int stepCount = 0;

    public UnityAction stepUpdate;
    
    


    public int boomAction { get { return boomCount; } set {  boomCount = value; Debug.Log(boomCount); } }
    public int StepAction { get { return stepCount; } set { stepCount = value; stepUpdate?.Invoke(); } }
}
