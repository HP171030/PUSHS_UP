using Cinemachine;
using System.Collections;
using UnityEngine;

public class GameSceneLoaderOnly2Player : BaseScene
{
    [SerializeField] protected CinemachineVirtualCamera [] thisSceneCine;
    [SerializeField] public int clearValue;
    [SerializeField] public int switchCount;
    [SerializeField] public bool bossSceneloader = false;
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
        if ( thisSceneCine [0] != null )
            cam.player1Camera = thisSceneCine [0];
        if ( thisSceneCine [1] != null )
            cam.player2Camera = thisSceneCine [1];
        yield return null;

        if ( thisSceneCine [0] == null )
        {
            cam.IsPlayer1Active = false;
            yield return null;
        }

        Manager.game.player2Controller.cameraSwitch = cam;

    }
    private void Start()
    {
        cam = FindObjectOfType<CameraSwitch>();
        cam.IsPlayer1Active = false;
    }


}