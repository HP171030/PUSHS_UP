using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField]LayerMask player;
    [SerializeField] LayerMask obs;
    [SerializeField] Transform arrowStartPos;
    [SerializeField] GameObject arrow;
    [SerializeField] float distance;
    [SerializeField] float targetTime;
    [SerializeField] Renderer matRenderer;
    public bool inButton;
    bool startButton;


    private void Start()
    {
        Manager.game.stepUpdate += StartRouine;
        matRenderer.material.color = Color.red;
    }
    private void OnTriggerEnter( Collider other )
    {
       

        if ( player.Contain(other.gameObject.layer) )
        {
            matRenderer.material.color = Color.black;
            inButton = true;
        }
    }

    public void StartRouine()
    {
        if ( startButton )
        {
            Debug.Log(inButton); ;
            GameObject arrowIns = Instantiate(arrow, arrowStartPos.position, Quaternion.identity);
            arrowIns.transform.forward = arrowStartPos.transform.forward;
            StartCoroutine(Shoot(arrowIns));
            inButton = false;
            startButton = false;
        }
        if ( inButton )
        {
            startButton = true;
        }
    }
    private void OnTriggerExit( Collider other )
    {
        if ( player.Contain(other.gameObject.layer))
        {
            matRenderer.material.color = Color.red;
            Debug.Log("exit");
        }
    }

    IEnumerator Shoot(GameObject arrow)
    {
        arrow.SetActive(true);
        float time = 0;
        Vector3 startPos = arrow.transform.position;
        Vector3 targetPos = arrow.transform.position + arrow.transform.forward* distance;
        if ( Physics.BoxCast(arrow.transform.position, new Vector3(1, 1, 1) / 2f, arrow.transform.forward, out RaycastHit obsOrPlayer, Quaternion.identity, distance, player | obs) )
        {
            if ( obs.Contain(obsOrPlayer.collider.gameObject.layer) )
            {
                targetPos = obsOrPlayer.collider.transform.position;   
                while ( time < targetTime )
                {
                    time += Time.deltaTime;
                    arrow.transform.position = Vector3.Lerp(startPos, targetPos, time / targetTime);
                    yield return null;
                }
                Destroy(obsOrPlayer.collider.gameObject);
                arrow.SetActive(false);
                Destroy(arrow);
            }
            else
            {
                while ( time < targetTime )
                {
                    time += Time.deltaTime;
                    arrow.transform.position = Vector3.Lerp(startPos, targetPos, time / targetTime);
                    yield return null;
                }
                arrow.SetActive(false);
                Destroy(arrow);
                Manager.game.GameOver();

            }
        }
        else
        {
            while ( time < targetTime )
            {
                time += Time.deltaTime;
                arrow.transform.position = Vector3.Lerp(startPos, targetPos, time / targetTime);
                yield return null;
            }
            arrow.SetActive(false);
            Destroy(arrow);
        }



    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        // 박스 캐스트가 충돌하지 않았을 때, 예상되는 박스의 위치와 크기에 기즈모 그리기
        Vector3 boxCenter = arrow.transform.position + arrow.transform.forward * distance;
        Gizmos.DrawLine(arrow.transform.position, arrow.transform.position + arrow.transform.forward * distance);
        Gizmos.DrawWireCube(boxCenter, new Vector3(1, 1, 1));
        return;

    }
}
