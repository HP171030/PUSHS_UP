using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneLoaderOnly2Player : BaseScene
{
    [SerializeField] CameraSwitch switch2pl;
    private void Start()
    {
        switch2pl.IsPlayer1Active = false;
    }
    public override IEnumerator LoadingRoutine()
    {
        yield return null;
    }


}