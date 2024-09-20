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
    [SerializeField] Image pushIconImage;
    [SerializeField] Image pullIconImage;
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
    public void ChangePushPull(bool pull )
    {
        pushIconImage.enabled = !pull;
        pullIconImage.enabled = pull;
    }
}
