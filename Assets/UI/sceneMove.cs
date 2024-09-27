using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
public class SceneMover : MonoBehaviour
{
    [SerializeField] GameObject uiClose;
    [SerializeField] bool isRobby = false;
    public int sNumber;
    Button button;
    private void Start()
    {
        button = GetComponent<Button>();

    }
    public void NextSceneWithNum()
    {
        // 씬 번호를 이용해서 씬 이동
        button.interactable = false;
        Manager.scene.LoadScene(sNumber+1);


    }
    public void gotoRobby()
    {
        // 씬 번호를 이용해서 씬 이동
        button.interactable = false;
        UnitySceneManager.LoadScene(26);  // 0 번째 씬 로드
            Manager.ui.UiClose();
    }
}