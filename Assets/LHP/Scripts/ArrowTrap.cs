using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField]LayerMask player;
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
        RaycastHit [] players = Physics.BoxCastAll(arrow.transform.position, new Vector3(1, 1, 1) / 2f, arrow.transform.forward);
        
            foreach(RaycastHit p in players )
        {
            if(player.Contain(p.collider.gameObject.layer))
            {
                Debug.Log("화살에 맞음 GameOver");
            }
            else
            {
                Debug.Log("안맞음");
            }
        }
            
        
        while ( time < targetTime )
        {
            time += Time.deltaTime;
            arrow.transform.position = Vector3.Lerp(startPos, targetPos, time/targetTime);
        yield return null;
        }
        arrow.SetActive(false);
        Destroy( arrow );
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        RaycastHit hit;
        if ( Physics.BoxCast(arrow.transform.position, new Vector3(1, 1, 1) / 2f, arrow.transform.forward, out hit, Quaternion.identity, distance) )
        {
            // 충돌 지점까지의 선 그리기
            Gizmos.DrawLine(arrow.transform.position, hit.point);
            // 충돌한 지점에 큐브 형태의 기즈모 그리기
            Gizmos.DrawWireCube(hit.point, Vector3.one);
        }
        else
        {
            // 박스 캐스트가 충돌하지 않았을 때, 예상되는 박스의 위치와 크기에 기즈모 그리기
            Vector3 boxCenter = arrow.transform.position + arrow.transform.forward * distance;
            Gizmos.DrawWireCube(boxCenter, new Vector3(1, 1, 1));
        }
    }
}
