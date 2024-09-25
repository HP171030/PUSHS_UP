using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror2 : MonoBehaviour
{

    public bool obstacleChecker;
    public bool ObstacleChecker { get { return obstacleChecker; } }



    private void OnTriggerStay(Collider other)
    {


        if (other.gameObject.CompareTag("MoveDisable"))
        {
            Debug.Log(other.gameObject.tag);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") || (other.gameObject.CompareTag("MoveDisable")))
        {
           
            obstacleChecker = true;

        }
        //else if (other.gameObject.CompareTag("MoveDisable"))
        //{
        //    Debug.Log("MoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisable");
        //    obstacleChecker = true;

        //}


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") || (other.gameObject.CompareTag("MoveDisable")))
        {
            obstacleChecker = false;
            //YHP_PlayerController.AlreadyMap2Obstacle = false;
        }
        //else if (other.gameObject.CompareTag("MoveDisable"))
        //{
        //    Debug.Log("obstacleChecker = true;");
        //    obstacleChecker = false;

        //}
    }
}
