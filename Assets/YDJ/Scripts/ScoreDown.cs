using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDown : MonoBehaviour
{
    public Text scoreText; // UI Text ��ü�� ������ ����

    private void Start()
    {
        Manager.game.boomUpdate += UpdateScoreText;
    }

    private void OnEnable()
    {
        Manager.game.boomUpdate += UpdateScoreText;
    }
    private void OnDisable()
    {
        Manager.game.boomUpdate -= UpdateScoreText; 
    }


    // �ؽ�Ʈ�� ������Ʈ�ϴ� �Լ�
    void UpdateScoreText()
    {
        scoreText.text = Manager.game.boomAction.ToString(); // �ؽ�Ʈ ������Ʈ
    }
}
