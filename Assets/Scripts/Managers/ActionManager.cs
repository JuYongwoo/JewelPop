using System;
using UnityEngine;

public class ActionManager
{
    public event Func<Transform> GetJokerGoalTranformEvent;
    public event Action<GameObject, GameObject> inputBlockChangeEvent;
    public event Action<int, int> SetScoreUIEvent;
    public event Action<int> SetCurrentStageUIEvent;
    public event Func<bool> GetIsInMotionEvent;
    public event Action<bool> SetIsInMotionEvent;
    public event Func<bool> GetIsBoardChangedEvent;
    public event Action<bool> SetIsBoardCangedEvent;
    public event Action ShowResultPopupEvent;
    public event Action StageSceneInputControllerEvent;
    public event Action<int> DeltaScoreEvent;

    public Transform OnGetJokerGoalTranform()
    {
        return GetJokerGoalTranformEvent?.Invoke() ?? null;
    }
    public void OnInputBlockChange(GameObject a, GameObject b)
    {
        inputBlockChangeEvent?.Invoke(a, b);
    }

    public void OnSetScoreUI(int a, int b)
    {
        SetScoreUIEvent?.Invoke(a, b);
    }

    public void OnSetCurrentStageUI(int a)
    {
        SetCurrentStageUIEvent?.Invoke(a);
    }

    public bool OnGetIsInMotion()
    {
        return GetIsInMotionEvent?.Invoke() ?? false;
    }

    public void OnSetIsInMotion(bool a)
    {
        SetIsInMotionEvent?.Invoke(a);
    }

    public bool OnGetIsBoardChanged()
    {
        return GetIsBoardChangedEvent?.Invoke() ?? false;
    }

    public void OnSetIsBoardChanged(bool a)
    {
        SetIsBoardCangedEvent?.Invoke(a);
    }

    public void OnShowResultPopup()
    {
        ShowResultPopupEvent?.Invoke();
    }

    public void OnStageSceneInputController()
    {
        StageSceneInputControllerEvent?.Invoke();
    }

    public void OnDeltaScore(int a)
    {
        DeltaScoreEvent?.Invoke(a);
    }
}
