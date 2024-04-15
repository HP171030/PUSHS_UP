using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChap2 : Boss
{
    Animator anim;

    Tile [] frontRangeTile;
    Tile [] AllTile;
    [SerializeField] GameObject allTile;
    [SerializeField] GameObject frontFourRowTile;
    [SerializeField] float spawnHeight;
    [SerializeField] Transform startRightAttack;
    [SerializeField] GameObject fallingIcePrefab;
    RaycastHit [] tileAlert = null;
    int [] stoneFall;
    Tile [] isTiles;
    [SerializeField] int fallingthornsCount;

    bool targetTile = false;
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        frontRangeTile = frontFourRowTile.GetComponentsInChildren<Tile>();
        AllTile = allTile.GetComponentsInChildren<Tile>();

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
                alert = true;

                if ( !targetTile )
                {
                    patternCount = pattern1Count;
                    targetTile = true;
                    
                }
                if ( !isAlertP1 )
                {
                    foreach ( Tile tiles in frontRangeTile )
                    {

                        Renderer tilesRend = tiles.gameObject.GetComponent<Renderer>();
                        StartCoroutine(AlertTile(tilesRend, Color.red, 1));
                    }
                }
                isAlertP1 = true;
                    


                FrontAttack();

                break;


            case Pattern.Pattern2:


                alert = true;
                onPattern = true;
                if ( !targetTile )
                {
                    patternCount = pattern2Count;
                    //       startRightAttack = stoneSpawnerChildren [RandomLocation];

                    tileAlert = Physics.BoxCastAll(startRightAttack.position, new Vector3(5, 1, 1), Vector3.back, Quaternion.identity, 15f, tile);
                    targetTile = true;
                    
                    Debug.Log(tileAlert.Length);
                }
                if ( tileAlert.Length > 0 )
                {
                    if ( !isAlertP2 )
                    {
                        foreach ( RaycastHit tiles in tileAlert )
                        {

                            Renderer tilesRend = tiles.collider.gameObject.GetComponent<Renderer>();
                            StartCoroutine(AlertTile(tilesRend, Color.red, 1));
                        }

                    }
                    isAlertP2 = true;
                    
                }

                SideAttack();

                break;


            case Pattern.Pattern3:

                alert = true;
                onPattern = true;

                if ( !targetTile )
                {
                    stoneFall = new int [fallingthornsCount];
                    for ( int i = 0; i < fallingthornsCount; i++ )
                    {

                        stoneFall [i] = Random.Range(0, AllTile.Length);

                    }

                    patternCount = pattern3Count;

                    targetTile = true;

                }
                if (!isAlertP3 )
                {
                    isTiles = new Tile [stoneFall.Length]; // tiles 배열 초기화

                    for ( int i = 0; i < isTiles.Length; i++ )
                    {

                        isTiles [i] = AllTile [stoneFall [i]];
                    }
                    foreach ( Tile tile in isTiles )
                    {
                        Renderer isFallRend = tile.GetComponent<Renderer>();
                        StartCoroutine(AlertTile(isFallRend, Color.red, 1));
                    }
                }
                isAlertP3 = true;
                

                Howling();
                break;


        }
    }

    private void FrontAttack()
    {
        if ( patternCount <= 0 )
        {

            StartCoroutine(WaitPattern());

            Debug.Log("Action");
            alert = false;
            targetTile = false;
            anim.SetTrigger("Attack5");
            curState = Pattern.Idle;
            onPattern = false;
            isAlertP1 = false;
            foreach ( Tile tiles in frontRangeTile )
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
        }

    }
    private void SideAttack()
    {
        if ( patternCount <= 0 )
        {
            alert = false;
            targetTile = false;
            StartCoroutine(Run());
            Debug.Log("Action");
           


        }

    }

    public IEnumerator Run()
    {
        
        StartCoroutine(WaitPattern());
        curState = Pattern.Idle;
        anim.SetBool("Run Forward", true);
        float time = 0;
        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position + transform.forward * 5f;
        while(time < 1 )
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, time / 1);
            yield return null;
        }
        yield return new WaitForSeconds(0.7f);
        anim.SetBool("Run Forward", false);
        anim.SetTrigger("Attack1");
        foreach(RaycastHit rayedTile in tileAlert )
        {
            Tile tile = rayedTile.collider.gameObject.GetComponent<Tile>();

          Collider[] col =  Physics.OverlapSphere(tile.middlePoint.position, 1f, player);
            if( col.Length > 0 )
            {
                foreach(Collider Cplayer in col )
                {
                    if ( player.Contain(Cplayer.gameObject.layer) )
                    {
                        Manager.game.GameOver();
                    }
                }
            }
            
        }
        yield return new WaitForSeconds(0.7f);

        time = 0;
        anim.SetBool("WalkBackward",true);
        while(time < 2 )
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(targetPos,startPos, time / 2);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        anim.SetBool("WalkBackward", false);
       
        
        onPattern = false;
        isAlertP2 = false;
    }

    public void Howling()
    {
        if ( patternCount <= 0 )
        {
            StartCoroutine(WaitPattern());
            Debug.Log("Action");
            alert = false;
            targetTile = false;
            anim.SetTrigger("Buff");

            foreach ( Tile tile in isTiles )
            {
                Transform tilePoint = tile.middlePoint;
                GameObject stone = Instantiate(fallingIcePrefab, tilePoint.position + new Vector3(0, spawnHeight, 0), Quaternion.Euler(180,0,0));
                Collider [] colliders = Physics.OverlapSphere(tilePoint.gameObject.transform.position, 1f);
                if ( colliders.Length > 0 )
                {
                    foreach ( Collider col in colliders )
                    {
                        if ( player.Contain(col.gameObject.layer) )
                        {
                            Manager.game.GameOver();
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
