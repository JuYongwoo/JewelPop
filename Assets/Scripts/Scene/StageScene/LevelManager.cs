using UnityEngine;
using System;

public class LevelManager<T> where T : JSONVars //������ ���� ���� ������ ����
{
    public enum GoalType
    {
        Joker
    }

    GoalType goalType = GoalType.Joker; //TODO ���� Ȯ�� �� / JYW ��ǥ Ÿ�Ը��� ���� ��ǥ �ٸ��� ����
    
    
    private int currentStage = 0;

    private int currentScore = 0;
    private int goalScore = 0;

    public T currentLevel;

    public void Init(string json)
    {

        //JSON ���� ���� ������ �ʱ�ȭ
        currentLevel = JsonUtility.FromJson<T>(json);
        goalScore = currentLevel.goalScore;
        goalType = Enum.Parse<GoalType>(currentLevel.goalType);
        currentStage = currentLevel.stage;
        currentScore = 0;

        //UI �ʱ�ȭ
        GameManager.instance.actionManager.DeltaScore -= deltaScore;
        GameManager.instance.actionManager.DeltaScore += deltaScore;
        GameManager.instance.actionManager.setCurrentStageUIM(currentStage);
        GameManager.instance.actionManager.setScoreUIM(currentScore, goalScore);
        GameManager.instance.soundManager.PlaySound(Sounds.BGM1, 0.25f, true);
    }

    public void OnDestroy()
    {
        GameManager.instance.actionManager.DeltaScore -= deltaScore;
    }

    public void deltaScore(int delta) //���� ������ �ݵ�� �̰��� ���
    {
        currentScore += delta;
        GameManager.instance.actionManager.setScoreUIM(currentScore, goalScore);


        //���� ���� ��
        if (currentScore >= goalScore)
        {
            //���� Ŭ����
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
        GameManager.instance.actionManager.showResultPopupM();
        GameManager.instance.soundManager.StopSound(Sounds.BGM1);
        GameManager.instance.soundManager.PlaySound(Sounds.Victory, 0.25f, false);
    }


}

