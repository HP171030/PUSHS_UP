using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterMover : MonoBehaviour
{
    [SerializeField] LayerMask player;
    [SerializeField] CameraSwitch camSwitch;
    [SerializeField] StepCountUI stepCountUI; // StepCountUI ���� ���� �߰�
    [SerializeField] GameSceneLoader gameSceneLoader;
    [SerializeField] GameSceneLoaderOnly2Player gameSceneLoaderOnly2Player;

    private int previousMissionCount; // ���� MissionCount ���� ������ ����

    public void Start()
    {
        int curSceneNum = Manager.scene.GetSceneNumber();
        Debug.Log($"ScnenNum{curSceneNum}");
        gameSceneLoader = FindObjectOfType<GameSceneLoader>();
        if ( gameSceneLoader != null )
        {
            Debug.Log("None1Loader");
            gameSceneLoaderOnly2Player = FindObjectOfType<GameSceneLoaderOnly2Player>();
        }
        while(camSwitch == null )
        {
            camSwitch = FindObjectOfType<CameraSwitch>();
            Debug.Log("camSwitchf Searching");
        }
        

        // StepCountUI ��ü�� ã�� �Ҵ�
        stepCountUI = FindObjectOfType<StepCountUI>();

        // ���� MissionCount ���� �ε��մϴ�.
        previousMissionCount = PlayerPrefs.GetInt("PreviousMissionCount", stepCountUI.MissionCount);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.Contain(other.gameObject.layer))
        {
            
            if (Manager.game.clearValue > 1)
            {
                Debug.Log("in1");
                camSwitch.Change();
                Manager.game.clearValue--;
                Manager.game.isEnter = true;
                
            }
            else
            {
                
                // �� �ѹ� Ȯ�� �� ����
                int curSceneNum = Manager.scene.GetSceneNumber();
                
                // ���ο� MissionCount ���� ���� ������ ū ��쿡�� �����մϴ�.
                if (stepCountUI.MissionCount >= previousMissionCount)
                {
                    PlayerPrefs.SetInt("stageNumber" + curSceneNum, stepCountUI.MissionCount);
                    // ���� MissionCount ���� ������Ʈ�մϴ�.
                    previousMissionCount = stepCountUI.MissionCount;
                    PlayerPrefs.SetInt("PreviousMissionCount", previousMissionCount);
                }

                // �� ����Ǿ����� Ȯ��
                int savedMissionCount = PlayerPrefs.GetInt("stageNumber" + curSceneNum);
                print("����� MissionCount: " + savedMissionCount);
                if ( gameSceneLoader != null )
                {
                    if ( !gameSceneLoader.bossSceneloader )
                    {
                        Manager.scene.LoadScene(curSceneNum + 1);

                    }
                    else
                    {
                        Manager.scene.LoadScene(26);
                    }
                }
                else
                {
                    if ( !gameSceneLoaderOnly2Player.bossSceneloader )
                    {
                        Manager.scene.LoadScene(curSceneNum + 1);

                    }
                    else
                    {
                        Manager.scene.LoadScene(26);
                    }
                }

                Manager.game.isEnter = false;
            }
            Destroy(other.gameObject);
        }
    }
}