using System;
using UnityEngine;

public class ActionManager
{
    public event Func<Transform> getJokerGoalTranform;
    public event Action<GameObject, GameObject> inputBlockChangeAction;
    public event Action<int, int> setScoreUI;
    public event Action<int> setCurrentStageUI;
    public event Func<bool> getIsInMotion;
    public event Action<bool> setIsInMotion;
    public event Func<bool> getIsBoardChanged;
    public event Action<bool> setIsBoardChanged;
    public event Action showResultPopup;
    public event Action StageSceneInputController;
    public event Action<int> DeltaScore;

    public Transform getJokerGoalTranformM()
    {
        return getJokerGoalTranform?.Invoke() ?? null;
    }
    public void inputBlockChangeActionM(GameObject a, GameObject b)
    {
        inputBlockChangeAction?.Invoke(a, b);
    }

    public void setScoreUIM(int a, int b)
    {
        setScoreUI?.Invoke(a, b);
    }

    public void setCurrentStageUIM(int a)
    {
        setCurrentStageUI?.Invoke(a);
    }

    public bool getIsInMotionM()
    {
        return getIsInMotion?.Invoke() ?? false;
    }

    public void setIsInMotionM(bool a)
    {
        setIsInMotion?.Invoke(a);
    }

    public bool getIsBoardChangedM()
    {
        return getIsBoardChanged?.Invoke() ?? false;
    }

    public void setIsBoardChangedM(bool a)
    {
        setIsBoardChanged?.Invoke(a);
    }

    public void showResultPopupM()
    {
        showResultPopup?.Invoke();
    }

    public void StageSceneInputControllerM()
    {
        StageSceneInputController?.Invoke();
    }

    public void DeltaScoreM(int a)
    {
        DeltaScore?.Invoke(a);
    }
}
