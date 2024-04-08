using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager instance;
   public static Manager Ins {  get { return instance; } }

    [SerializeField] GameManager gameManager;
    [SerializeField] SceneManager sceneManager;
    [SerializeField] PoolManager poolManager;
    [SerializeField] UIManager uIManager;
    [SerializeField] SoundManager soundManager;

    public static GameManager game { get { return instance.gameManager; } }
    public static SceneManager scene { get { return instance.sceneManager; } }
    public static SoundManager sound { get { return instance.soundManager; } }
    public static UIManager ui { get { return instance.uIManager; } }
    public static PoolManager pool { get { return instance.poolManager; } }
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
