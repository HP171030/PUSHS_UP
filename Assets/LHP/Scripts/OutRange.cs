using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
   [SerializeField] LayerMask player;
    private void OnTriggerEnter( Collider other )
    {
        if(player.Contain(other.gameObject.layer))
        {
            Manager.game.GameOver();
        }
    }
}
