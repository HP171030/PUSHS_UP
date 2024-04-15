using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager1 : MonoBehaviour
{
    public int score;
    // Start is called before the first frame update
    void Start()
    {
        //  score = 1;
        //   print(score);
        int SaveData = PlayerPrefs.GetInt("Map101c");
        print(SaveData);
        PlayerPrefs.SetInt("Map101c", 3);

        //Unity Save System
        // key : 저장된 데이터의 고유 값 / 문자열 (String) 형식
        // value : 저장된 데이터 / 정수, 실수, 문자열 형식

        void Update()
        {

        }
    }
}
