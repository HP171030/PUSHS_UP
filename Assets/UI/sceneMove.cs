using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class SceneMover : MonoBehaviour
{
    public int sNumber;
    public void NextSceneWithNum()
    {
        // �� ��ȣ�� �̿��ؼ� �� �̵�
        UnitySceneManager.LoadScene(sNumber);  // 0 ��° �� �ε�
    }

    public void gotoRobby()
    {
        // �� ��ȣ�� �̿��ؼ� �� �̵�
        UnitySceneManager.LoadScene(22);  // 0 ��° �� �ε�
    }
}