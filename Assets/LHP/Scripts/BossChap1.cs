using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChap1 : Boss
{
    Animator anim;
    [SerializeField] GameObject stoneSpawner;
    [SerializeField] Transform [] stoneSpawnerChildren;
    Tile [] sweapAllTile;

   [SerializeField] GameObject sweapTile;
    [SerializeField] float spawnHeight;
    Transform getStartSpawner;
    [SerializeField] GameObject stonePrefab;
    [SerializeField] GameObject fallingStonePrefab;
    RaycastHit [] tileAlert = null;
   [SerializeField] int [] stoneFall;
    [SerializeField] Tile [] isTiles;
    [SerializeField] int fallingStoneCount;

    bool targetTile = false;
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        sweapAllTile = sweapTile.GetComponentsInChildren<Tile>();
        

    }

    protected override void Update()
    {
       
        switch(curState)
        {
            case Pattern.Idle:
                IdleState();
                break;

            case Pattern.Pattern1:
                
             

                onPattern = true;
                alert = true;
                 stoneSpawnerChildren = stoneSpawner.GetComponentsInChildren<Transform>();
                int RandomLocation = Random.Range(0, 9);

                
               
                if ( !targetTile )
                {
                    patternCount = 10;
                    getStartSpawner = stoneSpawnerChildren [RandomLocation];

                    tileAlert = Physics.BoxCastAll(getStartSpawner.position, new Vector3(1, 1, 1), Vector3.back,Quaternion.identity,15f,tile);
                    targetTile = true;
                    Debug.Log(tileAlert.Length);
                }
                if(tileAlert.Length > 0 )
                {
                    foreach ( RaycastHit tiles in tileAlert )
                    {

                        Renderer tilesRend = tiles.collider.gameObject.GetComponent<Renderer>();
                        StartCoroutine(AlertTile(tilesRend, Color.red,1));
                    }
                }

                RollingStone();
                
                break;


                case Pattern.Pattern2:
               
                
                alert = true;
                onPattern = true;
                if ( !targetTile )
                {
                    sweapAllTile = sweapTile.GetComponentsInChildren<Tile>();
                    patternCount = 10;

                    targetTile = true;

                }
                foreach(Tile tiles in sweapAllTile )
                {
                    Renderer rend = tiles.GetComponent<Renderer>();
                    StartCoroutine(AlertTile(rend, Color.red, 1));
                }

                SweapMap();

                break;


            case Pattern.Pattern3:

                alert = true;
                onPattern = true;
                
                if ( !targetTile )
                {
                    stoneFall = new int [fallingStoneCount];
                    for ( int i = 0; i < fallingStoneCount; i++ )
                    {

                        stoneFall [i] = Random.Range(0, sweapAllTile.Length);
                      
                    }
                    
                    patternCount = 10;

                    targetTile = true;

                }
                isTiles = new Tile [stoneFall.Length]; // tiles 배열 초기화

                for ( int i = 0; i < isTiles.Length; i++ )
                {

                    isTiles [i] = sweapAllTile [stoneFall[i]]; 
                }
                foreach(Tile tile in isTiles )
                {
                    Renderer isFallRend = tile.GetComponent<Renderer>();
                    StartCoroutine(AlertTile(isFallRend, Color.red, 1));
                }

                Howling();
                break;


        }
    }

    private void RollingStone()
    {
        if(patternCount <= 0 )
        {
            
            StartCoroutine(WaitPattern());

            Debug.Log("Action");
            alert = false;
            targetTile = false;
        anim.Play("RollingStone");
            curState = Pattern.Idle;
            onPattern = false;
            
        }

    }
    public void RollingAnim()
    {
        Manager.game.ShakeCam();
        GameObject stone = Instantiate(stonePrefab, getStartSpawner.position + new Vector3(0,spawnHeight,0) , Quaternion.identity);
        foreach(RaycastHit onAttack in tileAlert )
        {
            Collider [] collider = Physics.OverlapSphere(onAttack.collider.transform.position + new Vector3(0, 1, 0), 1f, player);
            if(collider.Length > 0 )
            {
                Debug.Log("GameOver");
            }
        }
        
    }

    private void SweapMap()
    {
        if(patternCount <= 0 )
        {
            StartCoroutine(WaitPattern());
            Debug.Log("Action");
            alert = false;
            targetTile = false;
            anim.Play("Sweap");
            foreach ( Tile tiles in sweapAllTile )
            {
                Transform tilePoint = tiles.middlePoint;
                Collider [] isSomething = Physics.OverlapSphere(tilePoint.gameObject.transform.position, 1f);
                if(isSomething.Length > 0 )
                {
                    foreach (Collider col in isSomething )
                    {
                        if ( player.Contain(col.gameObject.layer) )
                        {
                            Debug.Log("GameOverOnSweap");
                        }
                        else if(obstacle.Contain(col.gameObject.layer) )
                        {
                            Destroy(col.gameObject);
                        }
                   
                    }
                    
                }
            }
            curState = Pattern.Idle;
            onPattern = false;
        }
    }
 
    public void Howling()
    {if(patternCount <= 0 )
        {
            StartCoroutine(WaitPattern());
            Debug.Log("Action");
            alert = false;
            targetTile = false;
        anim.Play("Howling");

            foreach(Tile tile in isTiles )
            {
                Transform tilePoint = tile.middlePoint;
                GameObject stone = Instantiate(fallingStonePrefab, tilePoint.position + new Vector3(0, spawnHeight, 0), Quaternion.identity);
                Collider [] colliders = Physics.OverlapSphere(tilePoint.gameObject.transform.position , 1f);
                if(colliders.Length > 0 )
                {
                    foreach(Collider col in colliders )
                    {
                        if ( player.Contain(col.gameObject.layer) )
                        {
                            Debug.Log("GameOverOnSweap");
                        }
                        
                    }
                }
            }
            curState = Pattern.Idle;
            onPattern = false;
        }
    }


}
