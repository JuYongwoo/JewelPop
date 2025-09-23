using UnityEngine;
using System;

public class LevelManager<T> where T : JSONVars //������ ���� ���� ������ ����
{
    public enum GoalType
    {
        Joker
    }

    GoalType goalType = GoalType.Joker; //TODO JYW ��ǥ Ÿ�Ը��� ���� ��ǥ �ٸ��� �����ؾ��� - MapManager ���� ��� ������ �κп��� ó����
    
    
    private int currentStage = 0;

    private int currentScore = 0;
    private int goalScore = 0;

    public T currentLevel;


    public void setLevel(string json)
    {

        currentLevel = JsonUtility.FromJson<T>(json);


        goalScore = currentLevel.goalScore;
        goalType = Enum.Parse<GoalType>(currentLevel.goalType);
        currentStage = currentLevel.stage;


    }

    public void OnStart()
    {
        currentScore = 0;
        ActionManager.setCurrentStageUI(currentStage);
        ActionManager.setScoreUI(currentScore, goalScore);
        ManagerObject.instance.soundManager.PlaySound(Sounds.BGM1, true);
    }


    public void deltaScore(int delta) //���� ������ �ݵ�� �̰��� ���
    {
        currentScore += delta;
        ActionManager.setScoreUI(currentScore, goalScore);
    }

    public int getScore()
    {
        return currentScore;
    }


}

