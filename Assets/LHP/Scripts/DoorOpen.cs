using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    [SerializeField]GameObject particle;
    [SerializeField] GameObject[] door;
    [SerializeField] LayerMask player;
    [SerializeField] float openTime;
    bool isNotOpen = false;
    bool isSwitching;


      private void OnTriggerEnter( Collider other )
       {
           if ( player.Contain(other.gameObject.layer) && !isNotOpen&&!isSwitching)
           {
               particle.SetActive(true);

            if(Manager.game.doorSwitch <= 1 )
            {
                isNotOpen = true;
                StartCoroutine(OpenDoor());
            }
            else
            {
                Debug.Log($"{other.gameObject.name}°¡ ´©¸§"  );
                isSwitching = true;
                Manager.game.doorSwitch--;
            }
              

           }
       }
    
    IEnumerator OpenDoor()
    {
        foreach ( GameObject go in door )
        {
            float time = 0;
            Vector3 startPos = go.transform.position;
            Vector3 targetPos = go.transform.position - new Vector3(0, 5f, 0);
            while ( time < openTime )
            {
                time += Time.deltaTime;
                go.transform.position = Vector3.Lerp(startPos, targetPos, time / openTime);
                yield return null;
            }

        }
        
           
    }
}
