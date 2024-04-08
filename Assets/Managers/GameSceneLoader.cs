
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneLoader : BaseScene
{
    public override IEnumerator LoadingRoutine()
    {
        Manager.game.boomAction = 3;
        yield return null;
    }


}
