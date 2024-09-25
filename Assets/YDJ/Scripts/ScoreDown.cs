using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDown : MonoBehaviour
{
    public Text scoreText; // UI Text 객체를 저장할 변수

    private void OnEnable()
    {
        Manager.game.boomUpdate += UpdateScoreText;
        Debug.Log("Score update");
    }
    private void OnDisable()
    {
        Manager.game.boomUpdate -= UpdateScoreText; 
    }


    // 텍스트를 업데이트하는 함수
    void UpdateScoreText()
    {
        scoreText.text = Manager.game.boomAction.ToString(); // 텍스트 업데이트
    }
}
