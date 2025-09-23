using UnityEngine;
using System;

public class LevelManager<T> where T : JSONVars //점수와 같은 게임 정보를 관리
{
    public enum GoalType
    {
        Joker
    }

    GoalType goalType = GoalType.Joker; //TODO JYW 목표 타입마다 게임 목표 다르게 설정해야함 - MapManager 에서 블록 터지는 부분에서 처리함
    
    
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


    public void deltaScore(int delta) //점수 증가는 반드시 이것을 사용
    {
        currentScore += delta;
        ActionManager.setScoreUI(currentScore, goalScore);
    }

    public int getScore()
    {
        return currentScore;
    }


}

