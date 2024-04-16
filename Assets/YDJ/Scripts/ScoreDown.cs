using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDown : MonoBehaviour
{
    public Text scoreText; // UI Text ��ü�� ������ ����
    private int score = 3; // �ʱ� ���� ����


    // ��ư�� Ŭ���� �� ȣ��� �Լ�
    public void DecreaseScore()
    {
        if (score > 0)
        {
            score -= 1; // ���� ����
            UpdateScoreText(); // �ؽ�Ʈ ������Ʈ
        }
    }

    // �ؽ�Ʈ�� ������Ʈ�ϴ� �Լ�
    void UpdateScoreText()
    {
        scoreText.text = score.ToString(); // �ؽ�Ʈ ������Ʈ
    }
}
