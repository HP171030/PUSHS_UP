using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterMover : MonoBehaviour
{
    [SerializeField] LayerMask player;
    [SerializeField] CameraSwitch camSwitch;
    [SerializeField] StepCountUI stepCountUI; // StepCountUI 참조 변수 추가

    private int previousMissionCount; // 이전 MissionCount 값을 저장할 변수

    public void Start()
    {
        camSwitch = FindObjectOfType<CameraSwitch>();
        // StepCountUI 객체를 찾아 할당
        stepCountUI = FindObjectOfType<StepCountUI>();

        // 이전 MissionCount 값을 로드합니다.
        previousMissionCount = PlayerPrefs.GetInt("PreviousMissionCount", stepCountUI.MissionCount);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.Contain(other.gameObject.layer))
        {
            Destroy(other.gameObject);
            if (Manager.game.clearValue > 1)
            {
                Debug.Log("in1");
                camSwitch.Change();
                Manager.game.clearValue--;
                Manager.game.isEnter = true;
            }
            else
            {
                Debug.Log("inTat);");
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

                Manager.scene.LoadScene(curSceneNum + 1);
                Manager.game.isEnter = false;
            }
        }
    }
}