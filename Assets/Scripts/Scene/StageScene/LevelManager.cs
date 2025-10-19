using UnityEngine;
using System;

public class LevelManager<T> where T : JSONVars //점수와 같은 게임 정보를 관리
{


    public GoalType goalType = GoalType.Joker; //TODO 게임 확장 시 / JYW 목표 타입마다 게임 목표 다르게 설정
    
    
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
        GameManager.instance.actionManager.DeltaScoreEvent -= DeltaScore;
        GameManager.instance.actionManager.DeltaScoreEvent += DeltaScore;
        GameManager.instance.actionManager.OnSetCurrentStageUI(currentStage);
        GameManager.instance.actionManager.OnSetScoreUI(currentScore, goalScore);
        GameManager.instance.actionManager.OnPlayAudioClip(GameManager.instance.resourceManager.gameSoundClipsHandles[Sounds.BGM1].Result, 0.25f, true);
    }

    public void OnDestroy()
    {
        GameManager.instance.actionManager.DeltaScoreEvent -= DeltaScore;
    }

    private void DeltaScore(int delta) //점수 증가는 반드시 이것을 사용
    {
        currentScore += delta;
        GameManager.instance.actionManager.OnSetScoreUI(currentScore, goalScore);


        //점수 도달 시
        if (currentScore >= goalScore)
        {
            //게임 클리어
            GameClear();
        }

    }


    private void GameClear()
    {
        Time.timeScale = 0f;
        GameManager.instance.actionManager.OnShowResultPopup();
        GameManager.instance.actionManager.OnStopAudioClip(GameManager.instance.resourceManager.gameSoundClipsHandles[Sounds.BGM1].Result);
        GameManager.instance.actionManager.OnPlayAudioClip(GameManager.instance.resourceManager.gameSoundClipsHandles[Sounds.Victory].Result, 0.25f, false);
    }


}

