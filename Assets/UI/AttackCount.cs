using UnityEngine;
using UnityEngine.UI;

public class AttackCount : MonoBehaviour
{
    
    public Text AttackText;
    private void Start()
    {
        // GameManager�� stepUpdate �̺�Ʈ�� UI ������Ʈ �Լ��� �����մϴ�.
        Manager.game.stepUpdate += UpdateStepText;
        Manager.game.stepUpdate += stepDecrease;
        // �ʱ� �ؽ�Ʈ ǥ��
        UpdateStepText();
    }

    private void UpdateStepText()
    {
        // UI Text�� stepCount ���� ǥ���մϴ�.
        int A = Manager.game.patternStep - 1;
        if ( A < 0 )
            A = 0;
        Debug.Log(A);
        AttackText.text =A.ToString();
    }

    public void stepDecrease()
    {

        if(Manager.game.patternStep > 0)
        Manager.game.patternStep--;
        Debug.Log("downCount");
    }
    private void OnDestroy()
    {
        // ��ũ��Ʈ�� �ı��� �� �̺�Ʈ�� �����մϴ�.
        Manager.game.stepUpdate -= UpdateStepText;
        Manager.game.stepUpdate -= stepDecrease;
    }
}