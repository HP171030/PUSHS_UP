using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class UIManager : MonoBehaviour
{
   public CloseButton closeButton;
    [SerializeField] public GameObject bossStepChecker;
    [SerializeField] public GameObject StageUi;
    [SerializeField] public GameObject menuClose;
    [SerializeField] public GameObject confirmClose;
    [SerializeField] public Canvas canvas;
    public void UiClose()
    {
        
        menuClose.SetActive(false);
        confirmClose.SetActive(false);
    }
   
}
