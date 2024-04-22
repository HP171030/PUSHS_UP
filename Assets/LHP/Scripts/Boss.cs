using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Animator anim;
    [SerializeField] float patternTime;
    public enum Pattern { Idle, Pattern1, Pattern2, Pattern3 ,Dead }
    public Pattern curState;
    public bool alert;
     protected int patternCount;
   protected bool onPattern = false;
    protected Tile [] mapATiles;
    [SerializeField] protected GameObject mapAtile;

    [SerializeField] protected GameObject ObstacleInstance;
    
    [SerializeField] public LayerMask tile;
    [SerializeField] public LayerMask player;
    [SerializeField] public LayerMask obstacle;

    [SerializeField] public int pattern1Count;
    [SerializeField] public int pattern2Count;
    [SerializeField] public int pattern3Count;

  protected bool isAlertP1 = false;
  protected bool isAlertP2 = false;
  protected bool isAlertP3 = false;

    [Header("Sound")]
    [SerializeField] protected AudioClip bossBGM;

    [SerializeField]protected AudioClip bossAttack1;
    [SerializeField]protected AudioClip bossAttack2;
    [SerializeField]protected AudioClip bossAttack3;
    [SerializeField]protected AudioClip bossDead;

    [SerializeField] protected AudioClip bossAttack1Sound;
    [SerializeField] protected ParticleSystem bossAttack1Effect;
    [SerializeField] protected ParticleSystem bossAttack2Effect;

    protected virtual void Start()
    {
        curState = Pattern.Idle;
        Manager.game.stepUpdate += StepCounter;
        mapATiles = mapAtile.GetComponentsInChildren<Tile>();
        Manager.sound.PlayBGM(bossBGM);
        Debug.Log("onBGM");
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
    {if(curState == Pattern.Dead )
        {
            Manager.sound.PlaySFX(bossDead);
            yield return null;
        }
        else
        {
        onPattern = true;
        yield return new WaitForSeconds(patternTime);
        int patternRange = Random.Range(0, 3);

        switch ( patternRange )
        {
            case 0: curState = Pattern.Pattern1;
               
                break;
                case 1: curState = Pattern.Pattern2;             // 패턴 2 ,3 으로 바꿀것;
                                                                 // 패턴 2 ,3 으로 바꿀것;
                break;                                           // 패턴 2 ,3 으로 바꿀것;
            case 2: curState = Pattern.Pattern3;                // 패턴 2 ,3 으로 바꿀것;
                
                break;

        }

        }
       
    }

    public IEnumerator AlertTile(Renderer tileRenderer,Color color,float endTime)
    {

        Debug.Log("AlertStart");
        float time = 0;
        
        Color startColor = Color.white;
        Color endColor = color;
        while ( alert)
        {
            while ( time < endTime )
            {
                time += Time.deltaTime;
                tileRenderer.material.color = Color.Lerp(startColor, endColor, time / endTime);
                yield return null;
                if ( !alert )
                {
                    tileRenderer.material.color = Color.white;
                    yield break;
                }
                   
            }
            Color temp = startColor;
            startColor = endColor;
            endColor = temp;
           
            time = 0;
           

        }
       
    }
    
    public void StepCounter()
    {
        
        if ( patternCount > 0 )
        {
            patternCount--;
           
        }
  
    }
    public void CreateObstacle()
    {
        int [] obsCreate = new int [2];                              //배열 2개
        for ( int i = 0; i < 2; i++ )
        {

            obsCreate [i] = Random.Range(0, mapATiles.Length);           //랜덤으로 배열 2개에 숫자 2개 할당

        }
        for ( int i = 0; i < obsCreate.Length; i++ )
        {
            Instantiate(ObstacleInstance, mapATiles [obsCreate [i]].middlePoint.position, Quaternion.identity);
        }


    }

    public IEnumerator WaitPattern()
    {
        Manager.game.PlayerControllStop();
        
        yield return new WaitForSeconds(2f);
        Manager.game.PlayerControllerOn();
        
        CreateObstacle();
    }
}
