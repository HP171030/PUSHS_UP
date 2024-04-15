using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class SceneLoader : MonoBehaviour
{
    // 원래 씬의 이름을 저장합니다.
    private string originalSceneName;

    public void loadStart()
    {
        // 최초 씬의 이름을 저장합니다.
        originalSceneName = UnitySceneManager.GetActiveScene().name;
        // 원래 씬에서 로딩 작업을 시작합니다.
        StartCoroutine(LoadOriginalScene());
    }

    IEnumerator LoadOriginalScene()
    {
        // 여기서는 예시로 3초간 로딩을 시뮬레이션합니다.
        yield return new WaitForSeconds(3f);

        // 로딩이 끝나면 원래 씬으로 돌아갑니다.
        UnitySceneManager.LoadScene(originalSceneName);
    }
}