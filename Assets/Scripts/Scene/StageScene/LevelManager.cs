using UnityEngine;
using System;

public class LevelManager<T> where T : JSONVars //점수와 같은 게임 정보를 관리
{
    public enum GoalType
    {
        Joker
    }

    GoalType goalType = GoalType.Joker; //TODO 게임 확장 시 / JYW 목표 타입마다 게임 목표 다르게 설정
    
    
    private int currentStage = 0;

    private int currentScore = 0;
    private int goalScore = 0;

    public T currentLevel;

    public void Init(string json)
    {

        //JSON 토대로 레벨 데이터 초기화
        currentLevel = JsonUtility.FromJson<T>(json);
        goalScore = currentLevel.goalScore;
        goalType = Enum.Parse<GoalType>(currentLevel.goalType);
        currentStage = currentLevel.stage;
        currentScore = 0;

        //UI 초기화
        ActionManager.setCurrentStageUI(currentStage);
        ActionManager.setScoreUI(currentScore, goalScore);
        AppManager.instance.soundManager.PlaySound(Sounds.BGM1, 0.25f, true);
    }


    public void deltaScore(int delta) //점수 증가는 반드시 이것을 사용
    {
        currentScore += delta;
        ActionManager.setScoreUI(currentScore, goalScore);


        //점수 도달 시
        if (currentScore >= goalScore)
        {
            //게임 클리어
            GameClear();
        }

    }

    public int getScore()
    {
        return currentScore;
    }

    private void GameClear()
    {
        Time.timeScale = 0f;
        AppManager.instance.actionManager.showResultPopup();
        AppManager.instance.soundManager.StopSound(Sounds.BGM1);
        AppManager.instance.soundManager.PlaySound(Sounds.Victory, 0.25f, false);
    }


}

