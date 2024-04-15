using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


public class SceneManager : MonoBehaviour
{
    [SerializeField] Image fade;
    [SerializeField] Image loadingImage;
    [SerializeField] Slider loadingBar;
    [SerializeField] float fadeTime;


    private BaseScene curScene;


    public BaseScene GetCurScene()
    {
        if (curScene == null )
        {
            curScene = FindObjectOfType<BaseScene>();
        }

        return curScene;
    }
    public int GetSceneNumber()
    {
        return UnitySceneManager.GetActiveScene().buildIndex;
    }
    public void LoadScene(int sceneNum )
    {
        StartCoroutine(LoadingRoutine(sceneNum));
    }
    public void RestartScene()
    {
        Debug.Log("Restart");
       int thisScene = GetSceneNumber();
        LoadScene(thisScene);
        Time.timeScale = 1f;
        Manager.ui.closeButton.CloseOnClick();
    }
    IEnumerator LoadingRoutine( int sceneNum )
    {
        yield return FadeOut();
        Manager.pool.ClearPool();
        /*      Manager.Sound.StopSFX();
                Manager.UI.ClearPopUpUI();
                Manager.UI.ClearWindowUI();
                Manager.UI.CloseInGameUI();
        */

        Time.timeScale = 0f;
        loadingImage.gameObject.SetActive(true);
        loadingBar.gameObject.SetActive(true);

        AsyncOperation oper = UnitySceneManager.LoadSceneAsync(sceneNum);
        while(oper.isDone == false )
        {
            loadingBar.value = oper.progress;
            yield return null;
        }
        BaseScene curScene = GetCurScene();

        yield return curScene.LoadingRoutine(); 

        loadingBar.gameObject.SetActive(false);
        Time.timeScale = 1f;
        loadingImage.gameObject.SetActive(false);

        yield return FadeIn();
    }
    

    IEnumerator FadeOut()
    {
        float rate = 0;

        Color fadeOutColor = new Color(fade.color.r, fade.color.g, fade.color.b, 1f);
        Color fadeInColor = new Color(fade.color.r, fade.color.g, fade.color.b, 0f);

        while(rate <= 1 )
        {
            rate += Time.deltaTime / fadeTime;
            fade.color= Color.Lerp(fadeInColor, fadeOutColor, rate);
            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        float rate = 0;
        Color fadeOutColor = new Color(fade.color.r, fade.color.g, fade.color.b, 1f);
        Color fadeInColor = new Color(fade.color.r, fade.color.g, fade.color.b, 0f);

        while ( rate <= 1 )
        {
            rate += Time.deltaTime / fadeTime;
            fade.color = Color.Lerp(fadeOutColor, fadeInColor, rate);
            yield return null;
        }
    }
}
