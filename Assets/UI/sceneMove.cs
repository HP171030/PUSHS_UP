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
        // �� ��ȣ�� �̿��ؼ� �� �̵�
        button.interactable = false;
        Manager.scene.LoadScene(sNumber+1);


    }
    public void gotoRobby()
    {
        // �� ��ȣ�� �̿��ؼ� �� �̵�
        button.interactable = false;
        UnitySceneManager.LoadScene(26);  // 0 ��° �� �ε�
            Manager.ui.UiClose();
    }
}