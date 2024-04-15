
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneLoader : BaseScene
{
    [SerializeField] protected CinemachineVirtualCamera[] thisSceneCine;
    [SerializeField] public int clearValue;
    [SerializeField] public int switchCount;
    [SerializeField] protected bool bossSceneloader = false;
    [SerializeField] protected CameraSwitch cam;

    public override IEnumerator LoadingRoutine()
    {
        Manager.game.StepAction = 0;
        Manager.game.doorSwitch = switchCount;
        Manager.game.isEnter = false;
        Manager.game.boomAction = 3;
        Manager.game.cines = thisSceneCine;
       Manager.game.clearValue = clearValue;
        Manager.game.playerController = FindObjectOfType<YHP_PlayerController>();
        Manager.game.player2Controller = FindObjectOfType<Player2Controller>();
        Manager.game.bossScene = bossSceneloader;
        cam = FindObjectOfType<CameraSwitch>();
        cam.player1Camera = thisSceneCine [0];
        if ( thisSceneCine [1] !=null )
        cam.player2Camera = thisSceneCine [1];
        yield return null;
        cam = Manager.game.player2Controller.cameraSwitch;
        
    }
    private void Start()
    {

        Manager.game.doorSwitch = switchCount;
        Manager.game.isEnter = false;
        Manager.game.clearValue = clearValue;
        Manager.game.cines = thisSceneCine;
        Manager.game.playerController = FindObjectOfType<YHP_PlayerController>();
        Manager.game.player2Controller = FindObjectOfType<Player2Controller>();
        Manager.game.bossScene = bossSceneloader;
        cam = FindObjectOfType<CameraSwitch>();
        cam.player1Camera = thisSceneCine [0];
        if ( thisSceneCine [1] != null )
            cam.player2Camera = thisSceneCine [1];
    }

}
