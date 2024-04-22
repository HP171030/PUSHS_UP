using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffDoor : MonoBehaviour
{
    [SerializeField] Renderer rend;

    private void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        rend.material.SetColor("_EmissionColor",Color.black);
       
    }


}
