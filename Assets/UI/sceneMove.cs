using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
public class SceneMover : MonoBehaviour
{
    public int sNumber;
    public void NextSceneWithNum()
    {
        // �� ��ȣ�� �̿��ؼ� �� �̵�
        Manager.scene.LoadScene(sNumber+1);
    }
    public void gotoRobby()
    {
        // �� ��ȣ�� �̿��ؼ� �� �̵�
        UnitySceneManager.LoadScene(26);  // 0 ��° �� �ε�
    }
}