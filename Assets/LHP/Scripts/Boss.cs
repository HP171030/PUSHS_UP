using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] float patternTime;
    public enum Pattern { Idle, Pattern1, Pattern2, Pattern3 }
    public Pattern curState;
    public bool alert;
    protected int patternCount;
   protected bool onPattern = false;

    [SerializeField] public LayerMask tile;
    [SerializeField] public LayerMask player;
    [SerializeField] public LayerMask obstacle;
    protected virtual void Start()
    {
        curState = Pattern.Idle;
        Manager.game.stepUpdate += StepCounter;
    }


    protected virtual void Update()
    {

       
    }

    protected virtual void IdleState()
    {
        if(!onPattern)
        StartCoroutine(PatternStart());

    }

    IEnumerator PatternStart()
    {
        onPattern = true;
        yield return new WaitForSeconds(patternTime);
       
        int patternRange = Random.Range(0, 3);

        switch ( patternRange )
        {
            case 0: curState = Pattern.Pattern1;
               
                break;
                case 1: curState = Pattern.Pattern2;
                
                break;
            case 2: curState = Pattern.Pattern3;                    
                
                break;

        }
    }

    public IEnumerator AlertTime(Renderer tileRenderer)
    {
      
        float time = 0;
        float endTime = 1;
        Color startColor = Color.white;
        Color endColor = Color.red;
        while ( alert )
        {
            while ( time < endTime )
            {
                time += Time.deltaTime;
                tileRenderer.material.color = Color.Lerp(startColor, endColor, time / endTime);
                yield return null;
                if ( !alert )
                    yield break;
            }
            Color temp = startColor;
            startColor = endColor;
            endColor = temp;

            time = 0;
            yield return null;
        }
        tileRenderer.material.color = Color.white;
    }
    
    public void StepCounter()
    {
        Debug.Log("StepCounting");
        if ( patternCount > 0 )
        {
            patternCount--;
            Debug.Log(patternCount);
        }
  
    }
}
