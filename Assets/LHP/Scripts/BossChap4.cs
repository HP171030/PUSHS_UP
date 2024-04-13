using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChap4 : Boss
{
    Animator anim;

    Collider [] safeAreas = null;
    [SerializeField] Tile [] AllTile;

    [SerializeField] Tile [] RowInExplode;

    [SerializeField] List<Renderer> renderers;

    [SerializeField] GameObject allTile;
    [SerializeField] Transform explodeParent;
    [SerializeField] Transform [] explodeRange;

    [SerializeField] Transform mapMidPoint;

    Collider [] P2Colliders;

    [SerializeField] ParticleSystem pattern1Effect;
    [SerializeField] Dictionary<int, bool> pattern1Bool;


    [SerializeField] Transform [] statues;
    [SerializeField] GameObject shine;

    RaycastHit [] tileAlert = null;

    bool targetTile = false;
    bool alertBool = false;


    [SerializeField] Vector3 [] debugVectors;
    [SerializeField] Vector3 debugVector;


    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();


        AllTile = allTile.GetComponentsInChildren<Tile>();
        explodeRange = explodeParent.gameObject.GetComponentsInChildren<Transform>();
      //  pattern3Range = pattern3RangeParent.gameObject.GetComponentsInChildren<Transform>();


        pattern1Bool = new Dictionary<int, bool>();




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
                    patternCount = 10;
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

                    patternCount = 10;

                    targetTile = true;

                }
               P2Colliders =  Physics.OverlapBox(mapMidPoint.position, new Vector3(7.5f, 1, 9), Quaternion.identity, tile);
                
                foreach(Collider col in P2Colliders )
                {
                    Renderer renderers = col.GetComponent<Renderer>();
                    if(renderers != null )
                    StartCoroutine(AlertTile(renderers, Color.red, 1f));
                }

                Pattern2Attack();


                break;


            case Pattern.Pattern3:
                alert = true;
                onPattern = true;
                if ( !targetTile )
                {


                    patternCount = 10;
                    targetTile = true;
                    int safeAreaIndex = Random.Range(0, statues.Length);
                    safeAreas = Physics.OverlapBox(statues [safeAreaIndex].position, new Vector3(2, 1, 2), Quaternion.identity, tile);
                    shine.transform.position = statues [safeAreaIndex].position;
                }
                
              
                
              
                foreach(Collider col in safeAreas )
                {
                    Renderer renderers = col.GetComponent<Renderer>();
                    Tile isNotAlert = col.GetComponent<Tile>();
                    if( isNotAlert != null )
                    isNotAlert.isTargetTile = true;

                    if ( renderers != null )
                    {
                        StartCoroutine(AlertTile(renderers, Color.yellow, 1f));
                    }
                }
                foreach(Tile tiles in AllTile )
                {
                    if ( !tiles.isTargetTile )
                    {
                        Renderer rend = tiles.GetComponent<Renderer>();
                        StartCoroutine(AlertTile(rend, Color.red, 1));
                    }
                }
                Pattern3Attack();

                break;


        }
    }
    private void OnDrawGizmos()
    {
        
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(mapMidPoint.position, new Vector3(15.5f, 1, 18));



    }
    private void ExplodeAttack()
    {
        if ( patternCount <= 0 )
        {

            StartCoroutine(WaitPattern());

            Debug.Log("Action");
            alert = false;
            targetTile = false;
            
            StartCoroutine(Pattern1Anim());


        }

    }




    IEnumerator RowAlert()
    {

        if ( !alertBool )
        {
            alert = true;
            pattern1Bool = new Dictionary<int, bool>();
            foreach ( Transform isRowRanges in explodeRange )
            {

                alertBool = true;

                tileAlert = Physics.BoxCastAll(isRowRanges.position, new Vector3(0.3f, 0.3f, 0.3f), Vector3.right, Quaternion.identity, 20f, tile);

                for ( int i = 0; i < 10; i++ )
                {
                    pattern1Bool [i] = false;
                    Renderer isRenderer = tileAlert [i].collider.gameObject.GetComponent<Renderer>();
                    StartCoroutine(AlertTile(isRenderer, Color.green, 2));

                }
                yield return new WaitForSeconds(0.1f);

                tileAlert = null;



            }
        }
    }


    IEnumerator Pattern1Anim()
    {
        anim.Play("SpiderSpit 0");
        yield return new WaitForSeconds(1);
        Tile [] tiles = new Tile [10];

        foreach ( Transform t in explodeRange )
        {
            for ( int i = 0; i < 10; i++ )
            {
                Collider [] col = Physics.OverlapSphere(t.position + new Vector3(i * 2, 0f, 0f), 0.5f, tile);
                //    debugVector = t.position + new Vector3(i * 2, 0f, 0f);
                yield return null;
                if ( col.Length > 0 && col != null )
                {
                    foreach ( Collider col2 in col )
                    {
                        Tile Istile = col2.gameObject.GetComponent<Tile>();
                        if ( Istile != null )
                        {
                            tiles [i] = Istile;
                        }
                    }
                }
            }
            for ( int i = 0; i < 10; i++ )
            {

                Collider [] isObjects = Physics.OverlapSphere(tiles [i].middlePoint.position, 0.5f, player | obstacle);
                if ( isObjects.Length > 0 )
                {
                    foreach ( Collider isObj in isObjects )
                    {
                        pattern1Bool [i] = true;
                        if ( player.Contain(isObj.gameObject.layer) )
                        {
                            Debug.Log("GameOver");
                        }
                        else if ( obstacle.Contain(isObj.gameObject.layer) )
                        {
                            Destroy(isObj.gameObject);
                        }
                    }
                }
                if ( !pattern1Bool [i] )
                    Instantiate(pattern1Effect, tiles [i].middlePoint.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
            yield return null;
        }


        tileAlert = null;
        curState = Pattern.Idle;
        onPattern = false;
        alertBool = false;
    }

    private void Pattern2Attack()
    {
        if ( patternCount <= 0 )
        {
            StartCoroutine(WaitPattern());
            Debug.Log("Action");
            alert = false;
            targetTile = false;
            anim.SetTrigger("Attack");

            Collider[] IsDamaged = Physics.OverlapBox(mapMidPoint.position, new Vector3(7.5f, 1, 9), Quaternion.identity, player|obstacle);
            foreach ( Collider col in IsDamaged )
                    {
                        if ( player.Contain(col.gameObject.layer) )
                        {
                            Debug.Log("GameOverOnSweap");
                        }
                        else if ( obstacle.Contain(col.gameObject.layer) )
                        {
                            Destroy(col.gameObject);
                        }

                    }
            curState = Pattern.Idle;
            onPattern = false;
        }
    }
    private void Pattern3Attack()
    {
        if ( patternCount <= 0 )
        {
            

            shine.transform.position = new Vector3(0, 0, -10);
            StartCoroutine(WaitPattern());
            Debug.Log("Action");
            alert = false;
            targetTile = false;
            anim.Play("SpiderUltimate");
            Manager.game.ShakeCam();

            foreach ( Tile tiles in AllTile )
            {
                if ( !tiles.isTargetTile )
                {
                    Collider [] p3Objects = Physics.OverlapSphere(tiles.middlePoint.position,1f, player|obstacle);

                    if ( p3Objects.Length > 0 )
                    {
                        foreach ( Collider col in p3Objects )
                        {
                            if ( player.Contain(col.gameObject.layer) )
                            {
                                Debug.Log("GameOverOnPattern3");
                            }
                            else if ( obstacle.Contain(col.gameObject.layer) )
                            {
                                Destroy(col.gameObject);
                            }

                        }

                    }
                }
            }
            

            foreach ( Collider col in safeAreas )
            {

                Tile isNotAlert = col.GetComponent<Tile>();
                if ( isNotAlert != null )
                    isNotAlert.isTargetTile = false;
            }
            curState = Pattern.Idle;
            onPattern = false;
        }
    }
}
