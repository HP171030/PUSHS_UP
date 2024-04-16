using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterMover : MonoBehaviour
{
    [SerializeField] LayerMask player;
    [SerializeField] CameraSwitch camSwitch;
    [SerializeField] StepCountUI stepCountUI; // StepCountUI ���� ���� �߰�

    private int previousMissionCount; // ���� MissionCount ���� ������ ����

    public void Start()
    {
        camSwitch = FindObjectOfType<CameraSwitch>();
        // StepCountUI ��ü�� ã�� �Ҵ�
        stepCountUI = FindObjectOfType<StepCountUI>();

        // ���� MissionCount ���� �ε��մϴ�.
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

                Manager.scene.LoadScene(curSceneNum + 1);
                Manager.game.isEnter = false;
            }
        }
    }
}