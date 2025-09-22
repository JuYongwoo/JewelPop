using System;
using UnityEngine;

public class ActionManager
{
    public Func<Transform> getJokerGoalTranform;
    public Action<GameObject, GameObject> inputBlockChangeAction;
    public Action<int> setScoreUI;
    public Action<bool> setIsInMotion;
    public Action<bool> setIsBoardChanged;

}
