using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : MonoBehaviour
{
    [SerializeField] float range;

    private GameObject mirror; // 거울을 저장할 변수



    // 거울을 반환하는 메서드
    public GameObject GrabMirror()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);
        for (int i = 0; i < colliders.Length; i++)
        {

            if (colliders[i].gameObject.CompareTag("Mirror"))
            {
                Debug.Log("거울 감지");
                mirror = colliders[i].gameObject;
                return mirror;
            }
        }

        return null;
    }

    public GameObject GetMirror()
    {
        return mirror;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }




    public bool WallLader()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                return true;
            }
        }

        return false;
    }

    public bool FrontObstacleLader()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                return true;
            }
        }

        return false;
    }

    public bool FrontMirrorLader()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Mirror"))
            {
                Debug.Log("mirrorLader");
                return true;
            }
        }
       
        return false;
    }

    public bool MoveDisableCheckerLader()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.CompareTag("MoveDisable"))
            {
                return true;
            }
        }

        return false;
    }

}
