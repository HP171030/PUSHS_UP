using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class IceChunkTrap : MonoBehaviour
{
    [SerializeField] LayerMask player;
    PlayerController controller;
    Player2Controller player2Controller;
    [SerializeField]LayerMask iceLayer;

    Vector3 playerMoveDir;
    bool onIce = false;

    private void OnTriggerEnter( Collider other )
    {
        Debug.Log(other.gameObject.name);
        if ( player.Contain(other.gameObject.layer) )
        {
            Debug.Log("ENter");
            onIce = true;
            controller = other.gameObject.GetComponent<PlayerController>();

            if(controller != null )
            {
                playerMoveDir = controller.moveDir;
                controller.onIce = true;
                StartCoroutine(EnterIce(other.gameObject.GetComponent<Rigidbody>()));
            }
           else if(controller == null )
            {
                player2Controller = other.gameObject.GetComponent<Player2Controller>();
                playerMoveDir = player2Controller.moveDir;
                player2Controller.onIce = true;
                StartCoroutine(EnterIce(other.gameObject.GetComponent<Rigidbody>()));
            }
        }
    }

    public IEnumerator EnterIce( Rigidbody player )
    {
        if(controller != null )
        {
            Animator anim = controller.GetComponent<Animator>();
            anim.SetFloat("MoveSpeed", 0);
            anim.SetBool("OnIce", true);


            while ( onIce )
            {
                yield return new WaitForFixedUpdate();

                player.MovePosition(player.gameObject.transform.position + player.gameObject.transform.forward * 0.1f);
                if ( Physics.OverlapSphere(player.position, 1f, iceLayer).Length > 0 && !Physics.Raycast(player.position + new Vector3(0, 0.5f, 0), player.transform.forward, 1f) )
                {
                    yield return null;
                }
                else
                {
                    onIce = false;
                    controller.onIce = false;
                    controller.moveDir = Vector3.zero;
                    anim.SetFloat("MoveSpeed", 0);
                    anim.SetBool("OnIce", false);
                    yield return null;

                }
            }
        }
       else if(controller == null )
        {
            Animator anim2 = player2Controller.GetComponent<Animator>();
            anim2.SetFloat("MoveSpeed", 0);
            anim2.SetBool("OnIce", true);
 
            while ( onIce )
            {
                yield return new WaitForFixedUpdate();
                
                player.MovePosition(player.gameObject.transform.position + player.gameObject.transform.forward * 0.1f);
                if(Physics.OverlapSphere(player.position,1f,iceLayer).Length > 0 && !Physics.Raycast(player.position + new Vector3(0,0.5f,0),player.transform.forward,1f))
                {
                    yield return null;
                }
                else
                {
                onIce = false;
                player2Controller.onIce = false;
                player2Controller.moveDir = Vector3.zero;
                anim2.SetFloat("MoveSpeed", 0);
                    anim2.SetBool("OnIce", false);
                    yield return null;


                }
            }
        }
       


    }
}
