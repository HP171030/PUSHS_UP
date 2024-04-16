using UnityEngine;

public class StageController : MonoBehaviour
{
    public GameObject[] stages;
    private int currentStageIndex = 0;

    private void Start()
    {
        // 처음에는 첫 번째 스테이지만 보이도록 설정합니다.
        ShowStage(currentStageIndex);
    }

    public void NextStage()
    {
        // 다음 스테이지로 이동합니다.
        currentStageIndex++;
        if (currentStageIndex >= stages.Length)
        {
            // 현재 스테이지 인덱스가 배열의 길이를 벗어나면 마지막 스테이지로 설정합니다.
            currentStageIndex = stages.Length - 1;
        }

        // 현재 스테이지를 보여줍니다.
        ShowStage(currentStageIndex);
        print(currentStageIndex);
    }

    public void PreviousStage()
    {
        // 이전 스테이지로 이동합니다.
        currentStageIndex--;
        if (currentStageIndex < 0)
        {
            // 현재 스테이지 인덱스가 0보다 작으면 첫 번째 스테이지로 설정합니다.
            currentStageIndex = 0;
        }

        // 현재 스테이지를 보여줍니다.
        ShowStage(currentStageIndex);
        print(currentStageIndex);
    }

    private void ShowStage(int index)
    {
        // 모든 스테이지를 숨깁니다.
        foreach (GameObject stage in stages)
        {
            stage.SetActive(false);
        }

        // 현재 스테이지를 보여줍니다.
        stages[index].SetActive(true);
    }
}