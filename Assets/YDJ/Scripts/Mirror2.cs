using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror2 : MonoBehaviour
{
    [SerializeField] Mirror1 Mirror1;
    [SerializeField] YHP_PlayerController YHP_PlayerController;
    public bool obstacleChecker;
    public bool ObstacleChecker { get { return obstacleChecker; } }


    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle") || (other.gameObject.CompareTag("MoveDisable")))
        {
            Debug.Log("obstacleChecker = true;");
            obstacleChecker = true;

        }
        //else if (other.gameObject.CompareTag("MoveDisable"))
        //{
        //    Debug.Log("MoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisableMoveDisable");
        //    obstacleChecker = true;

        //}
        Debug.Log(other.gameObject.tag);

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
