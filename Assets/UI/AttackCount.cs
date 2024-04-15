using UnityEngine;
using UnityEngine.UI;

public class AttackCount : MonoBehaviour
{
    
    public Text AttackText;
    private void Start()
    {
        // GameManager의 stepUpdate 이벤트에 UI 업데이트 함수를 연결합니다.
        Manager.game.stepUpdate += UpdateStepText;
        // 초기 텍스트 표시
        UpdateStepText();
    }

    private void UpdateStepText()
    {
        // UI Text에 stepCount 값을 표시합니다.
        float A = Manager.game.StepAction;
        AttackText.text = (10 - A % 10).ToString();
    }

    private void OnDestroy()
    {
        // 스크립트가 파괴될 때 이벤트를 해제합니다.
        Manager.game.stepUpdate -= UpdateStepText;
    }
}