using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class ClearLoaderScript : MonoBehaviour
{

    public GameObject show3Stars;
    public GameObject show2Stars;
    public GameObject show1Stars;
    public int stageSceneNumber;

    // Start is called before the first frame update
   public void Start()
    {   
        int SaveData = PlayerPrefs.GetInt($"stageNumber {stageSceneNumber}");
        if( SaveData == 0 )
        {
            return;
        }
        Debug.Log($"{stageSceneNumber}Stage Score: {SaveData}");

        if (SaveData < 150)
        {
            show1Stars.SetActive(true);
        }
        if (SaveData < 100)
        {
            show2Stars.SetActive(true);
        }
        if (SaveData < 50)
        {
            show3Stars.SetActive(true);
        }
    }

}
