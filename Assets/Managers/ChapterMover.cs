using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterMover : MonoBehaviour
{
    [SerializeField] LayerMask player;
    [SerializeField] CameraSwitch camSwitch;
    [SerializeField] GameSceneLoader gameSceneLoader;
    [SerializeField] GameSceneLoaderOnly2Player gameSceneLoaderOnly2Player;

    private int previousMissionCount; // ���� MissionCount ���� ������ ����
    int curSceneNum;

    public void Start()
    {
        curSceneNum = Manager.scene.GetSceneNumber();
        Debug.Log($"ScnenNum{curSceneNum}");
        gameSceneLoader = FindObjectOfType<GameSceneLoader>();
            gameSceneLoaderOnly2Player = FindObjectOfType<GameSceneLoaderOnly2Player>();
       

            camSwitch = FindObjectOfType<CameraSwitch>();
            Debug.Log($"camSwitch : {camSwitch}");

        
        // ���� MissionCount ���� �ε��մϴ�.
        previousMissionCount = PlayerPrefs.GetInt($"PreviousMissionCount {curSceneNum}");

    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.Contain(other.gameObject.layer))
        {
            
            if (Manager.game.clearValue > 1)
            {
                Debug.Log("in1");
                camSwitch.CharacterChange();
                Manager.game.clearValue--;
                Manager.game.isEnter = true;
                
            }
            else
            {
               
                

                if (Manager.game.StepAction < previousMissionCount || previousMissionCount == 0)
                {
                    PlayerPrefs.SetInt($"stageNumber {curSceneNum}", Manager.game.StepAction);
                    PlayerPrefs.SetInt($"PreviousMissionCount {curSceneNum}", Manager.game.StepAction);
                }

                // �� ����Ǿ����� Ȯ��
                int savedStepCount = PlayerPrefs.GetInt($"stageNumber {curSceneNum}");
                Debug.Log($"����� StepCount:{savedStepCount}");
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