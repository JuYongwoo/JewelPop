using UnityEngine;
using System;
using JYW.JewelPop.JSON;
using JYW.JewelPop.Managers;

namespace JYW.JewelPop.StageScene
{
    public class LevelManager<T> where T : JSONVars //점수와 같은 게임 정보를 관리
    {


        public GoalType goalType = GoalType.Joker; //TODO 게임 확장 시 / JYW 목표 타입마다 게임 목표 다르게 설정


        private int currentStage = 0;
        private int currentScore = 0;
        private int goalScore = 0;

        public T currentLevel;

        public void OnAwake()
        {
            GameManager.instance.eventManager.DeltaScoreEvent -= DeltaScore;
            GameManager.instance.eventManager.DeltaScoreEvent += DeltaScore;
        }

        public void OnStart(string json)
        {

            //JSON 토대로 레벨 데이터 초기화
            currentLevel = JsonUtility.FromJson<T>(json);
            goalScore = currentLevel.GoalScore;
            goalType = Enum.Parse<GoalType>(currentLevel.GoalType);
            currentStage = currentLevel.Stage;
            currentScore = 0;

            //UI 초기화
            GameManager.instance.eventManager.OnSetCurrentStageUI(currentStage);
            GameManager.instance.eventManager.OnSetScoreUI(currentScore, goalScore);
            GameManager.instance.eventManager.OnPlayAudioClip(GameManager.instance.resourceManager.gameSoundClipsHandles[Sounds.BGM1].Result, 0.25f, true);
        }

        public void OnDestroy()
        {
            GameManager.instance.eventManager.DeltaScoreEvent -= DeltaScore;
        }

        private void DeltaScore(int delta) //점수 증가는 반드시 이것을 사용
        {
            currentScore += delta;
            GameManager.instance.eventManager.OnSetScoreUI(currentScore, goalScore);


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
            GameManager.instance.eventManager.OnShowResultPopup();
            GameManager.instance.eventManager.OnStopAudioClip(GameManager.instance.resourceManager.gameSoundClipsHandles[Sounds.BGM1].Result);
            GameManager.instance.eventManager.OnPlayAudioClip(GameManager.instance.resourceManager.gameSoundClipsHandles[Sounds.Victory].Result, 0.25f, false);
        }


    }

}