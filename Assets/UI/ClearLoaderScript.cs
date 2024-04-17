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
    public string stageNumber;

    // Start is called before the first frame update
   public void Start()
    {   
        int SaveData = PlayerPrefs.GetInt("stageNumber"+stageNumber+1);
        print(stageNumber + "+" +SaveData);

        if (SaveData >= 3)
        {
            show3Stars.SetActive(true);
        }
        if (SaveData >= 2)
        {
            show2Stars.SetActive(true);
        }
        if (SaveData >= 1)
        {
            show1Stars.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
