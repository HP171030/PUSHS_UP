using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterMover : MonoBehaviour
{
   [SerializeField] LayerMask player;
    [SerializeField] CameraSwitch camSwitch;

    public void Start()
    {
        camSwitch = FindObjectOfType<CameraSwitch>();
        int curSceneNum = Manager.scene.GetSceneNumber();
        PlayerPrefs.SetInt("stageNumber"+ curSceneNum,2);
        int SaveData = PlayerPrefs.GetInt("stageNumber" + curSceneNum);
        print("아이고 난!" + SaveData);

    }
    private void OnTriggerEnter( Collider other )
    {
        if (player.Contain(other.gameObject.layer) )
        {
            Destroy(other.gameObject);
            if(Manager.game.clearValue > 1 )
            {
                camSwitch.Change();
                Manager.game.clearValue--;
                Manager.game.isEnter = true;
            }
            else
            {
                int curSceneNum = Manager.scene.GetSceneNumber();
                Manager.scene.LoadScene(curSceneNum + 1);
                Manager.game.isEnter = false;

            }
           
        }
    }
}
