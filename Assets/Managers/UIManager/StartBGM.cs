using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBGM : MonoBehaviour
{
    [SerializeField] AudioClip startBGM;
    private void Start()
    {
        Manager.sound.PlayBGM(startBGM);
    }
}
