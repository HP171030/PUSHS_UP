using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror2 : MonoBehaviour
{
    [SerializeField] Mirror1 mirror1;
    [SerializeField] YHP_PlayerController YHP_PlayerController;
    public bool obstacleChecker;
    public bool ObstacleChecker{get { return obstacleChecker; } }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Debug.Log("obstacleChecker = true;");
            obstacleChecker = true;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            obstacleChecker = false;
            //YHP_PlayerController.AlreadyMap2Obstacle = false;
        }
    }
}
