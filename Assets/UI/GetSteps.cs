using UnityEngine;
using UnityEngine.UI;

public class StepCountUI : MonoBehaviour
{
    public GameManager gameManager;
    public Text stepText;

    private void Start()
    {
        // GameManager�� stepUpdate �̺�Ʈ�� UI ������Ʈ �Լ��� �����մϴ�.
        gameManager.stepUpdate += UpdateStepText;
        // �ʱ� �ؽ�Ʈ ǥ��
        UpdateStepText();
    }

    private void UpdateStepText()
    {
        // UI Text�� stepCount ���� ǥ���մϴ�.
        stepText.text = gameManager.StepAction.ToString();
    }

    private void OnDestroy()
    {
        // ��ũ��Ʈ�� �ı��� �� �̺�Ʈ�� �����մϴ�.
        gameManager.stepUpdate -= UpdateStepText;
    }
}