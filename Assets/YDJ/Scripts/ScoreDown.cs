using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDown : MonoBehaviour
{
    public Text scoreText; // UI Text 객체를 저장할 변수
    private int score = 3; // 초기 점수 설정


    // 버튼을 클릭할 때 호출될 함수
    public void DecreaseScore()
    {
        if (score > 0)
        {
            score -= 1; // 점수 감소
            UpdateScoreText(); // 텍스트 업데이트
        }
    }

    // 텍스트를 업데이트하는 함수
    void UpdateScoreText()
    {
        scoreText.text = score.ToString(); // 텍스트 업데이트
    }
}
