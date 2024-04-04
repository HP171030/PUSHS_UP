using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    [SerializeField]GameObject particle;
    [SerializeField] GameObject door;
    [SerializeField] LayerMask player;
    [SerializeField] float openTime;
    bool isNotOpen = false;


    private void OnTriggerEnter( Collider other )
    {
        if ( player.Contain(other.gameObject.layer) && !isNotOpen)
        {
            particle.SetActive(true);
            
            isNotOpen = true;
            StartCoroutine(OpenDoor());
            
        }
    }
    IEnumerator OpenDoor()
    {
        float time = 0;
        Vector3 startPos = door.transform.position;
        Vector3 targetPos = door.transform.position - new Vector3(0, 4f, 0);
        while (time < openTime )
        {
            time += Time.deltaTime;
            door.transform.position = Vector3.Lerp(startPos, targetPos, time / openTime);
            yield return null;
        }
           
    }
}
