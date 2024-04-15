using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class SceneLoader : MonoBehaviour
{
    // ���� ���� �̸��� �����մϴ�.
    private string originalSceneName;

    public void loadStart()
    {
        // ���� ���� �̸��� �����մϴ�.
        originalSceneName = UnitySceneManager.GetActiveScene().name;
        // ���� ������ �ε� �۾��� �����մϴ�.
        StartCoroutine(LoadOriginalScene());
    }

    IEnumerator LoadOriginalScene()
    {
        // ���⼭�� ���÷� 3�ʰ� �ε��� �ùķ��̼��մϴ�.
        yield return new WaitForSeconds(3f);

        // �ε��� ������ ���� ������ ���ư��ϴ�.
        UnitySceneManager.LoadScene(originalSceneName);
    }
}