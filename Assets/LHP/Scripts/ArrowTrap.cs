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
                Debug.Log("ȭ�쿡 ���� GameOver");
            }
            else
            {
                Debug.Log("�ȸ���");
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
            // �浹 ���������� �� �׸���
            Gizmos.DrawLine(arrow.transform.position, hit.point);
            // �浹�� ������ ť�� ������ ����� �׸���
            Gizmos.DrawWireCube(hit.point, Vector3.one);
        }
        else
        {
            // �ڽ� ĳ��Ʈ�� �浹���� �ʾ��� ��, ����Ǵ� �ڽ��� ��ġ�� ũ�⿡ ����� �׸���
            Vector3 boxCenter = arrow.transform.position + arrow.transform.forward * distance;
            Gizmos.DrawWireCube(boxCenter, new Vector3(1, 1, 1));
        }
    }
}
