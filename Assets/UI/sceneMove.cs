using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
public class SceneMover : MonoBehaviour
{
    [SerializeField] GameObject uiClose;
    public int sNumber;
    public void NextSceneWithNum()
    {
        // 씬 번호를 이용해서 씬 이동
        Manager.scene.LoadScene(sNumber+1);
        Manager.ui.UiClose();
    }
    public void gotoRobby()
    {
        // 씬 번호를 이용해서 씬 이동
        UnitySceneManager.LoadScene(26);  // 0 번째 씬 로드
    }
}