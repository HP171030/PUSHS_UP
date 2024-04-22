
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameSceneLoader : BaseScene
{
    [SerializeField] protected CinemachineVirtualCamera[] thisSceneCine;
    [SerializeField] public int clearValue;
    [SerializeField] public int switchCount;
    [SerializeField] public bool bossSceneloader = false;
    [SerializeField] protected CameraSwitch cam;
    [SerializeField] GameObject CheckerBoss;
    [SerializeField] protected bool MainScene = false;
    [SerializeField] GameObject video;


    public override IEnumerator LoadingRoutine()
    {
        if(cam!= null)
        cam.IsPlayer1Active = true;
        Manager.game.StepAction = 0;
        Manager.game.doorSwitch = switchCount;
        if ( cam.player2Camera.Follow == null | cam.player1Camera.Follow == null )
        {
            Manager.game.isEnter = true;
            Debug.Log("Only1Player");
        }
        else
        {
            Manager.game.isEnter = false;
        }

        Manager.game.boomAction = 3;
        Manager.game.cines = thisSceneCine;
       Manager.game.clearValue = clearValue;
        Manager.game.playerController = FindObjectOfType<YHP_PlayerController>();
        Manager.game.player2Controller = FindObjectOfType<Player2Controller>();
        Manager.game.bossScene = bossSceneloader;

        if ( MainScene )
        {
            Manager.ui.StageUi.SetActive(false);
        }
        else
        {
            Manager.ui.StageUi.SetActive(true) ;
        }
        Debug.Log(Manager.ui.bossStepChecker.name);
        CheckerBoss = Manager.ui.bossStepChecker;


        cam = FindObjectOfType<CameraSwitch>();
       
        if ( thisSceneCine [0] != null )
            cam.player1Camera = thisSceneCine [0];
        if ( thisSceneCine [1] !=null )
        cam.player2Camera = thisSceneCine [1];

        if(Manager.game.playerController != null )
        Manager.game.playerController.cameraSwitch = cam;

        if(Manager.game.player2Controller != null )
            Manager.game.player2Controller.cameraSwitch = cam;

       yield return null;
        if( bossSceneloader )
        {
            CheckerBoss.SetActive(true);
        }
        else
        {
            CheckerBoss.SetActive(false);
        }
        if ( video != null )
        {
            video.transform.SetParent(Manager.ui.canvas.transform, true);
            RectTransform rect = video.gameObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;


        }

       

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
        if ( thisSceneCine [0] != null )
            cam.player1Camera = thisSceneCine [0];
        if ( thisSceneCine [1] != null )
            cam.player2Camera = thisSceneCine [1];

        if ( Manager.game.playerController != null )
            Manager.game.playerController.cameraSwitch = cam;

        if ( Manager.game.player2Controller != null )
            Manager.game.player2Controller.cameraSwitch = cam;
        CheckerBoss = Manager.ui.bossStepChecker;
        if ( bossSceneloader )
        {
            CheckerBoss.SetActive(true);
        }
        else
        {
            CheckerBoss.SetActive(false);
        }
    }




}
