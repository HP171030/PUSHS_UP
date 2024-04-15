using UnityEngine;
using UnityEngine.UI;

public class AttackCount : MonoBehaviour
{
    public GameManager gameManager;
    public Text AttackText;
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
        float A = gameManager.StepAction;
        AttackText.text = (10 - A % 10).ToString();
    }

    private void OnDestroy()
    {
        // ��ũ��Ʈ�� �ı��� �� �̺�Ʈ�� �����մϴ�.
        gameManager.stepUpdate -= UpdateStepText;
    }
}