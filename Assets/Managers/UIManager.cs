using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public CloseButton closeButton;
    public GameObject bossStepChecker;
    public GameObject StageUi;
    public GameObject menuClose;
    public GameObject confirmClose;
    public Canvas canvas;
    [SerializeField] GameObject boomUi;
    [SerializeField] GameObject pushPullUi;
    [SerializeField] List<Image> iconImages;
    [SerializeField] Image pushIconImage;
    [SerializeField] Image pullIconImage;
    [SerializeField] Image mirrorImage;
    public void UiClose()
    {

        menuClose.SetActive(false);
        confirmClose.SetActive(false);
    }
    public void ChangeP1P2ICon(bool p1On)
    {
        boomUi.SetActive(!p1On);
        pushPullUi.SetActive(p1On);    
    }
    public void ChangeIcon(string iconName )
    {
        Debug.Log(iconName);
        foreach(var item in iconImages )
        {
            if( item.name != iconName )
            {
                item.enabled = false;
            }
            else
            {
                item.enabled = true;
            }
            
        }


    }
}
