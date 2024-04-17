using UnityEngine;
using UnityEngine.UI;

public class AttackCount : MonoBehaviour
{
    
    public Text AttackText;
    private void Start()
    {
        // GameManager의 stepUpdate 이벤트에 UI 업데이트 함수를 연결합니다.
        Manager.game.stepUpdate += UpdateStepText;
        Manager.game.stepUpdate += stepDecrease;
        // 초기 텍스트 표시
        UpdateStepText();
    }

    private void UpdateStepText()
    {
        // UI Text에 stepCount 값을 표시합니다.
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
        // 스크립트가 파괴될 때 이벤트를 해제합니다.
        Manager.game.stepUpdate -= UpdateStepText;
        Manager.game.stepUpdate -= stepDecrease;
    }
}