using System;
using UnityEngine;

public class InputManager
{
    
    public void OnUpdate()
    {
        GameManager.instance.actionManager.OnStageSceneInputController();

    }
    
}
