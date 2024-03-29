using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager instance;
   public static Manager Ins {  get { return instance; } }

    [SerializeField] GameManager gameManager;
    [SerializeField] SceneManager sceneManager;

    public static GameManager game { get { return instance.gameManager; } }
    public static SceneManager scene { get { return instance.sceneManager; } }

    private void Awake()
    {
        if(instance == null )
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
