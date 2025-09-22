using System;
using UnityEngine;

public class ActionManager
{
    public Func<Transform> getJokerGoalTranform;
    public Action<GameObject, GameObject> inputBlockChangeAction;
    public Action<int> setScoreUI;
    public Func<bool> getIsInMotion;
    public Action<bool> setIsInMotion;
    public Func<bool> getIsBoardChanged;
    public Action<bool> setIsBoardChanged;

}
