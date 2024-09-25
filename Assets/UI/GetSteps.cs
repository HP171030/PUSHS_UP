using UnityEngine;
using UnityEngine.UI;

public class StepCountUI : MonoBehaviour
{
    
    public Text stepText;
    public Image strikethroughImage3;
    public Image strikethroughImage2;
    public Image strikethroughImage1;


    private void Start()
    {
        // GameManager�� stepUpdate �̺�Ʈ�� UI ������Ʈ �Լ��� �����մϴ�.
        Manager.game.stepUpdate += UpdateStepText;
        // �ʱ� �ؽ�Ʈ ǥ��
        UpdateStepText();
    }

    public void UpdateStepText()
    {
        stepText.text = Manager.game.StepAction.ToString();

        if (Manager.game.StepAction >= 50)
        {
            // �ؽ�Ʈ�� ��Ҽ� �̹����� ǥ���մϴ�.
            strikethroughImage3.gameObject.SetActive(true);
        }

        if (Manager.game.StepAction >= 100)
        {
            // �ؽ�Ʈ�� ��Ҽ� �̹����� ǥ���մϴ�.
            strikethroughImage2.gameObject.SetActive(true);
        }

        if (Manager.game.StepAction >= 150)
        {
            // �ؽ�Ʈ�� ��Ҽ� �̹����� ǥ���մϴ�.
            strikethroughImage1.gameObject.SetActive(true);
        }

    }
    public void StepImageInit()
    {
        strikethroughImage1.gameObject.SetActive(false);
        strikethroughImage2.gameObject.SetActive(false);
        strikethroughImage3.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        // ��ũ��Ʈ�� �ı��� �� �̺�Ʈ�� �����մϴ�.
        Manager.game.stepUpdate -= UpdateStepText;
    }
}