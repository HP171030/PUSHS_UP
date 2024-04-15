using UnityEngine;
using UnityEngine.UI;

public class StepCountUI : MonoBehaviour
{
    
    public Text stepText;
    public Image strikethroughImage3;
    public Image strikethroughImage2;
    public Image strikethroughImage1;
    public int MissionCount = 3;


    private void Start()
    {
        // GameManager의 stepUpdate 이벤트에 UI 업데이트 함수를 연결합니다.
        Manager.game.stepUpdate += UpdateStepText;
        // 초기 텍스트 표시
        UpdateStepText();
    }

    public void UpdateStepText()
    {
        stepText.text = Manager.game.StepAction.ToString();

        if (Manager.game.StepAction >= 50)
        {
            // 텍스트에 취소선 이미지를 표시합니다.
            strikethroughImage3.gameObject.SetActive(true);
            MissionCount = 2;
        }

        if (Manager.game.StepAction >= 100)
        {
            // 텍스트에 취소선 이미지를 표시합니다.
            strikethroughImage2.gameObject.SetActive(true);
            MissionCount = 1;
        }

        if (Manager.game.StepAction >= 150)
        {
            // 텍스트에 취소선 이미지를 표시합니다.
            strikethroughImage1.gameObject.SetActive(true);
            MissionCount = 1;
        }

    }

    private void OnDestroy()
    {
        // 스크립트가 파괴될 때 이벤트를 해제합니다.
        Manager.game.stepUpdate -= UpdateStepText;
    }
}