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
        // key : ����� �������� ���� �� / ���ڿ� (String) ����
        // value : ����� ������ / ����, �Ǽ�, ���ڿ� ����

        void Update()
        {

        }
    }
}
