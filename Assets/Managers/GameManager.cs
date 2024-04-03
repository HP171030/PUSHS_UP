using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int boomCount = 3;
    
    


    public int boomAction { get { return boomCount; } set {  boomCount = value; Debug.Log(boomCount); } }
}
