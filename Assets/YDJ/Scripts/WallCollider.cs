using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollider : MonoBehaviour
{
    public bool wallMirrorAttachedChecker = false;
    public bool WallMirrorAttachedChecker { get { return wallMirrorAttachedChecker; } }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {

            wallMirrorAttachedChecker = true;
            Debug.Log(wallMirrorAttachedChecker);
        }
        else
        {
            //wallMirrorAttachedChecker = false;
        }
    }
}
