using UnityEngine;
using UnityEngine.UI;

public class StepCountUI : MonoBehaviour
{
    
    public Text stepText;

    private void Start()
    {
        // GameManager�� stepUpdate �̺�Ʈ�� UI ������Ʈ �Լ��� �����մϴ�.
        Manager.game.stepUpdate += UpdateStepText;
        // �ʱ� �ؽ�Ʈ ǥ��
        UpdateStepText();
    }

    private void UpdateStepText()
    {
        // UI Text�� stepCount ���� ǥ���մϴ�.
        stepText.text = Manager.game.StepAction.ToString();
    }

    private void OnDestroy()
    {
        // ��ũ��Ʈ�� �ı��� �� �̺�Ʈ�� �����մϴ�.
        Manager.game.stepUpdate -= UpdateStepText;
    }
}