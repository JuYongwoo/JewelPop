using System;
using UnityEngine;

public class ActionManager
{
    public Func<Transform> getJokerGoalTranform;
    public Action<GameObject, GameObject> inputBlockChangeAction;
    static public Action<int, int> setScoreUI;
    static public Action<int> setCurrentStageUI;
    public Func<bool> getIsInMotion;
    public Action<bool> setIsInMotion;
    public Func<bool> getIsBoardChanged;
    public Action<bool> setIsBoardChanged;
    public Action showResultPopup;
    public Action StageSceneInputController;

}
