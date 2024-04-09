
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneLoader : BaseScene
{
    [SerializeField] CinemachineVirtualCamera thisSceneCine;
    public override IEnumerator LoadingRoutine()
    {
        Manager.game.boomAction = 3;
        Manager.game.cine = thisSceneCine;
        Manager.game.playerController = FindObjectOfType<PlayerController>();
        Manager.game.player2Controller = FindObjectOfType<Player2Controller>();
        yield return null;
        
    }
    private void Start()
    {
        Manager.game.cine = thisSceneCine;
        Manager.game.playerController = FindObjectOfType<PlayerController>();
        Manager.game.player2Controller = FindObjectOfType<Player2Controller>();
    }

}
