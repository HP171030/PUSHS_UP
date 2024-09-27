using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    private Rigidbody rb;
    private bool isGrounded = false;
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask destroyStone;
    [SerializeField] LayerMask destroyObs;
    [SerializeField] ParticleSystem destroyEffect;
    private float torqueMagnitude = 1f;
    private float torqueIncrement = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(DestroyThis());
    }

    private void OnCollisionEnter( Collision collision )
    {
        if ( ground.Contain(collision.gameObject.layer) ) // ¹Ù´Ú¿¡ ´ê¾ÒÀ» ¶§¸¸ ½ÇÇà
        {
            isGrounded = true;
        }
        else if ( destroyStone.Contain(collision.gameObject.layer))
        {
            Instantiate(destroyEffect,transform.position, Quaternion.identity); 

            Destroy(gameObject);
            if ( destroyObs.Contain(collision.gameObject.layer) )
            {
               RaycastHit[] rays = Physics.RaycastAll(transform.position, Vector3.down, 10f, LayerMask.GetMask("Wall"));
                if(rays.Length == 0 )
                {
                    Destroy(collision.gameObject);
                }

            }
        }
    }
    private void FixedUpdate()
    {
        if ( isGrounded )
        {
            rb.AddForce(Vector3.back * 5f, ForceMode.Acceleration); 
            rb.AddTorque(new Vector3(-1,0,0) * torqueMagnitude, ForceMode.Impulse);
            torqueMagnitude += torqueIncrement;
        }
    }
    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(3f);
        if(gameObject!=null )
        Destroy(gameObject);
    }
}