using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] Player2Controller controller;
    [SerializeField] LayerMask ground;
    [SerializeField] LayerMask OnObs;
    [SerializeField] LayerMask player;
    [SerializeField] bool outGround;
    [SerializeField] Animator animator;
    [SerializeField] bool isDown;



    private void OnTriggerStay( Collider other )
    {
       
        if ( ground.Contain(other.gameObject.layer) || OnObs.Contain(other.gameObject.layer)&&!controller.downingBox)
        {
           outGround = true;
            controller.onGround = true;
           
            
            isDown = false;

        }
        else if ( ground.Contain(other.gameObject.layer) )
        {
            controller.ontheBox = false;
        }
    }

    private void OnTriggerExit( Collider other )
    {
        if ( ground.Contain(other.gameObject.layer) || OnObs.Contain(other.gameObject.layer) )
        {
            outGround = false;
          
        }
            
    }

    private void FixedUpdate()
    {
        if (!outGround )
        {
            controller.onGround = false;
            if ( !controller.onClimb &&!isDown)
            {
                isDown = true;
             //   StartCoroutine(controller.DownAnim());
                
            }
                
        }

    }

}
