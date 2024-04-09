using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowlingStone : MonoBehaviour
{
    [SerializeField] LayerMask destroyStone;
    [SerializeField] LayerMask destroyObs;
    [SerializeField] ParticleSystem destroyEffect;


    private void OnCollisionEnter( Collision collision )
    {
        if ( destroyStone.Contain(collision.gameObject.layer) )            //¹æÇØ¹° ÆÄ±«
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
            if ( destroyObs.Contain(collision.gameObject.layer) )
            {
                Destroy(collision.gameObject);
            }
        }
    }
    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(3f);
        if ( gameObject != null )
            Destroy(gameObject);
    }
}
