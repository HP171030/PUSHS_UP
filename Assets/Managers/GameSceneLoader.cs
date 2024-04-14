
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneLoader : BaseScene
{
    [SerializeField] CinemachineVirtualCamera[] thisSceneCine;
    [SerializeField] public int clearValue;
    [SerializeField] public int switchCount;
    [SerializeField] bool bossSceneloader = false;

    public override IEnumerator LoadingRoutine()
    {
        Manager.game.doorSwitch = switchCount;
        Manager.game.isEnter = false;
        Manager.game.boomAction = 3;
        Manager.game.cines = thisSceneCine;
       Manager.game.clearValue = clearValue;
        Manager.game.playerController = FindObjectOfType<PlayerController>();
        Manager.game.player2Controller = FindObjectOfType<Player2Controller>();
        Manager.game.bossScene = bossSceneloader;
        yield return null;
        
    }
    private void Start()
    {

        Manager.game.doorSwitch = switchCount;
        Manager.game.isEnter = false;
        Manager.game.clearValue = clearValue;
        Manager.game.cines = thisSceneCine;
        Manager.game.playerController = FindObjectOfType<PlayerController>();
        Manager.game.player2Controller = FindObjectOfType<Player2Controller>();
        Manager.game.bossScene = bossSceneloader;
    }

}
