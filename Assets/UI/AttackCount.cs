using UnityEngine;
using UnityEngine.UI;

public class AttackCount : MonoBehaviour
{
    
    public Text AttackText;
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
        float A = Manager.game.StepAction;
        AttackText.text = (10 - A % 10).ToString();
    }

    private void OnDestroy()
    {
        // ��ũ��Ʈ�� �ı��� �� �̺�Ʈ�� �����մϴ�.
        Manager.game.stepUpdate -= UpdateStepText;
    }
}