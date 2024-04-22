using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class BossChap3 : Boss
{
   

    bool isAlert;
    [SerializeField] Tile [] AllTile;

    [SerializeField] Tile [] RowInExplode;

    [SerializeField] GameObject allTile;
    [SerializeField] Transform explodeParent;
    [SerializeField] Transform [] explodeRange;
    [SerializeField] GameObject pattern3RangeParent;
    [SerializeField] Transform [] pattern3Range;

    [SerializeField] ParticleSystem pattern1Effect;
    [SerializeField] Dictionary<int, bool> pattern1Bool;

    RaycastHit [] tileAlert = null;

    bool targetTile = false;
    bool alertBool = false;


    [SerializeField] Vector3[] debugVectors;
    [SerializeField] Vector3 debugVector;
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
       
        
        AllTile = allTile.GetComponentsInChildren<Tile>();
        explodeRange = explodeParent.gameObject.GetComponentsInChildren<Transform>();
        pattern3Range = pattern3RangeParent.gameObject.GetComponentsInChildren<Transform>();
        pattern1Bool = new Dictionary<int,bool>();




    }


    protected override void Update()
    {

        switch ( curState )
        {
            case Pattern.Idle:

                IdleState();
                break;

            case Pattern.Pattern1:


                onPattern = true;
              

                if ( !targetTile )
                {
                    patternCount = pattern1Count;
                    Manager.game.patternStep = pattern1Count;
                    targetTile = true;

                }

              
                    StartCoroutine(RowAlert());

                ExplodeAttack();

                break;


            case Pattern.Pattern2:
                alert = true;
                onPattern = true;
                if ( !targetTile )
                {
                    
                    patternCount = pattern2Count;
                    Manager.game.patternStep = pattern2Count;

                    targetTile = true;

                }
                if ( !isAlertP2 )
                {
                    foreach ( Tile tiles in AllTile )
                    {
                        if ( !tiles.isTargetTile )
                        {
                            Renderer rend = tiles.GetComponent<Renderer>();
                            StartCoroutine(AlertTile(rend, Color.red, 1));
                        }

                    }
                }
                isAlertP2 = true;

                SweapMap();


                break;


            case Pattern.Pattern3:
                alert = true;
                onPattern = true;
                if(!targetTile )
                {


                    patternCount = pattern3Count;
                    Manager.game.patternStep = pattern3Count;
                    targetTile = true;
                }
                if(!isAlertP3 )
                {
                           foreach( Transform tiles in pattern3Range )
                           {
                               Collider [] colliders = Physics.OverlapBox(tiles.position, new Vector3(2f, 1, 2f),Quaternion.identity,tile);
                               
                                  foreach( Collider collider in colliders )
                                  {
                                     
                                     Tile tile = collider.GetComponent<Tile>();
                                        Renderer thisRends = tile.GetComponent<Renderer>(); 

                                      if ( thisRends != null )
                                          StartCoroutine(AlertTile(thisRends, Color.red, 1));
                                  }
                 
                    
                              }

                }
                isAlertP3 = true;

                Pattern3Attack();

                break;


        }
    }
    private void OnDrawGizmos()
    {
       if(debugVector != null )
        {
            Gizmos.color = Color.yellow;
            foreach(Transform t in pattern3Range )
            {
                Gizmos.DrawWireCube(t.position, new Vector3(2f, 1, 2f));
            }
            
         
        }
        else
        {
            Debug.Log(null);
        }
       
        

    }
    private void ExplodeAttack()
    {
        if ( patternCount <= 0 )
        {

            StartCoroutine(WaitPattern());
            Manager.sound.PlaySFX(bossAttack1);
            Debug.Log("Action");
            alert = false;
            targetTile = false;
            anim.SetTrigger("Pattern1");
            StartCoroutine(Pattern1Anim());
            
           
        }

    }
  
    


    IEnumerator RowAlert()
    {

        if (! alertBool )
        {
            alert = true;

            foreach ( Transform isRowRanges in explodeRange )
            {

                alertBool = true;
                
                tileAlert = Physics.BoxCastAll(isRowRanges.position, new Vector3(0.3f, 0.3f, 0.3f), Vector3.right, Quaternion.identity, 20f, tile);
               
                for ( int i = 0; i < 10; i++ )
                {
                    pattern1Bool [i] = false;
                    Renderer isRenderer = tileAlert [i].collider.gameObject.GetComponent<Renderer>();
                    StartCoroutine(AlertTile(isRenderer, Color.blue,2));
                    
                }
                yield return new WaitForSeconds(0.1f);
               
                tileAlert = null;
               


            }
        }
    }

   
    IEnumerator Pattern1Anim()
    {
        Tile [] tiles = new Tile [10];

      foreach(Transform t in explodeRange )
        {
            for(int i = 0;i < 10;i++ )
            {
                Collider [] col = Physics.OverlapSphere(t.position + new Vector3 (i*2,0f,0f), 0.5f, tile);
            //    debugVector = t.position + new Vector3(i * 2, 0f, 0f);
                yield return null;
                if (col.Length > 0 &&col != null )
                {
                    foreach(Collider col2 in col )
                    {
                        Tile Istile = col2.gameObject.GetComponent<Tile>();
                        if( Istile != null )
                        {
                            tiles [i] = Istile;
                        }
                    }
                }
            }
            for(int i = 0; i < 10;i++ )
            {
                Manager.sound.PlaySFX(bossAttack1Sound);
               
                Collider [] isObjects = Physics.OverlapSphere(tiles [i].middlePoint.position, 0.5f, player | obstacle);
                if ( !pattern1Bool [i] )
                {
                    Instantiate(pattern1Effect, tiles [i].middlePoint.position + new Vector3(0, 1, 0), Quaternion.identity);
                    Manager.game.ShakeCam();
                    if ( isObjects.Length > 0 )
                    {
                        foreach ( Collider isObj in isObjects )
                        {
                            pattern1Bool [i] = true;
                            if ( player.Contain(isObj.gameObject.layer) )
                            {
                                Manager.game.GameOver();
                            }
                            else if ( obstacle.Contain(isObj.gameObject.layer) )
                            {
                                Destroy(isObj.gameObject);
                            }
                        }
                    }
                }
                  
              
            }
            yield return null;
        }


        tileAlert = null;
        curState = Pattern.Idle;
        onPattern = false;
        alertBool = false;
    }

    private void SweapMap()
    {
        if ( patternCount <= 0 )
        {
            StartCoroutine(WaitPattern());
            Manager.sound.PlaySFX(bossAttack2);
            Debug.Log("Action");
            alert = false;
            targetTile = false;
            anim.Play("atack3");
            Manager.game.ShakeCam();
            foreach ( Tile tiles in AllTile )
            {
                Transform tilePoint = tiles.middlePoint;
                Collider [] isSomething = Physics.OverlapSphere(tilePoint.gameObject.transform.position, 1f);
                if ( isSomething.Length > 0 )
                {
                    foreach ( Collider col in isSomething )
                    {
                        if ( player.Contain(col.gameObject.layer) )
                        {
                            Manager.game.GameOver();
                        }
                        else if ( obstacle.Contain(col.gameObject.layer) )
                        {
                            Destroy(col.gameObject);
                        }

                    }

                }
            }
            curState = Pattern.Idle;
            onPattern = false;
            isAlertP2 = false;
        }
    }
    private void Pattern3Attack()
    {
        if ( patternCount <= 0 )
        {
            StartCoroutine(WaitPattern());
            Manager.sound.PlaySFX(bossAttack3);
            Debug.Log("Action");
            alert = false;
            targetTile = false;
            anim.Play("atack shield");
            Manager.game.ShakeCam();

            foreach ( Transform tiles in pattern3Range )
            {
                Collider [] colliders = Physics.OverlapBox(tiles.position, new Vector3(2f, 1, 2f), Quaternion.identity, tile);

                foreach ( Collider collider in colliders )
                {

                    Tile tile = collider.GetComponent<Tile>();
                    Collider [] isIn = Physics.OverlapSphere(tile.middlePoint.position, 1f );
                    if ( isIn.Length > 0 )
                    {
                        foreach ( Collider col in isIn )
                        {
                            Instantiate(bossAttack1Effect, tile.middlePoint.position,Quaternion.identity);
                            Instantiate(bossAttack2Effect, tile.middlePoint.position, Quaternion.identity);
                            if ( player.Contain(col.gameObject.layer) )
                            {
                                Manager.game.GameOver();
                            }
                            else if ( obstacle.Contain(col.gameObject.layer) )
                            {
                                Destroy(col.gameObject);
                            }

                        }

                    }

                }


            }
            curState = Pattern.Idle;
            onPattern = false;
            isAlertP3 = false;
        }
    }
}
