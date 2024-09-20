using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterMover : MonoBehaviour
{
    [SerializeField] LayerMask player;
    [SerializeField] CameraSwitch camSwitch;
    [SerializeField] StepCountUI stepCountUI; // StepCountUI 참조 변수 추가
    [SerializeField] GameSceneLoader gameSceneLoader;
    [SerializeField] GameSceneLoaderOnly2Player gameSceneLoaderOnly2Player;

    private int previousMissionCount; // 이전 MissionCount 값을 저장할 변수

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
        

        // StepCountUI 객체를 찾아 할당
        stepCountUI = FindObjectOfType<StepCountUI>();

        // 이전 MissionCount 값을 로드합니다.
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
                
                // 씬 넘버 확인 및 저장
                int curSceneNum = Manager.scene.GetSceneNumber();
                
                // 새로운 MissionCount 값이 이전 값보다 큰 경우에만 저장합니다.
                if (stepCountUI.MissionCount >= previousMissionCount)
                {
                    PlayerPrefs.SetInt("stageNumber" + curSceneNum, stepCountUI.MissionCount);
                    // 이전 MissionCount 값을 업데이트합니다.
                    previousMissionCount = stepCountUI.MissionCount;
                    PlayerPrefs.SetInt("PreviousMissionCount", previousMissionCount);
                }

                // 잘 저장되었는지 확인
                int savedMissionCount = PlayerPrefs.GetInt("stageNumber" + curSceneNum);
                print("저장된 MissionCount: " + savedMissionCount);
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