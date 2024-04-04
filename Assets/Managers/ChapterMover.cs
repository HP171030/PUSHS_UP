using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterMover : MonoBehaviour
{
   [SerializeField] LayerMask player;
    [SerializeField] bool isEnter= false;
    private void OnTriggerEnter( Collider other )
    {
        if ( !isEnter && player.Contain(other.gameObject.layer))

        {
            int curSceneNum = Manager.scene.GetSceneNumber();
            Manager.scene.LoadScene(curSceneNum + 1);
        }
    }
}
