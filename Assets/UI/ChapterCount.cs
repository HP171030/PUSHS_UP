using UnityEngine;

public class StageController : MonoBehaviour
{
    public GameObject[] stages;
    private int currentStageIndex = 0;

    private void Start()
    {
        // ó������ ù ��° ���������� ���̵��� �����մϴ�.
        ShowStage(currentStageIndex);
    }

    public void NextStage()
    {
        // ���� ���������� �̵��մϴ�.
        currentStageIndex++;
        if (currentStageIndex >= stages.Length)
        {
            // ���� �������� �ε����� �迭�� ���̸� ����� ������ ���������� �����մϴ�.
            currentStageIndex = stages.Length - 1;
        }

        // ���� ���������� �����ݴϴ�.
        ShowStage(currentStageIndex);
        print(currentStageIndex);
    }

    public void PreviousStage()
    {
        // ���� ���������� �̵��մϴ�.
        currentStageIndex--;
        if (currentStageIndex < 0)
        {
            // ���� �������� �ε����� 0���� ������ ù ��° ���������� �����մϴ�.
            currentStageIndex = 0;
        }

        // ���� ���������� �����ݴϴ�.
        ShowStage(currentStageIndex);
        print(currentStageIndex);
    }

    private void ShowStage(int index)
    {
        // ��� ���������� ����ϴ�.
        foreach (GameObject stage in stages)
        {
            stage.SetActive(false);
        }

        // ���� ���������� �����ݴϴ�.
        stages[index].SetActive(true);
    }
}