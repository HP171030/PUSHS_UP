using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restart : MonoBehaviour
{
    public void RestartFunc()
    {
        Manager.scene.RestartScene();
    }
}
