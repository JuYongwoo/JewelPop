
public class GameManager
{

    private int score = 0;

    public void deltaScore(int delta) //점수 증가는 반드시 이것을 사용
    {
        score += delta;
        ManagerObject.instance.actionManager.setScoreUI(score);
    }

    public int getScore()
    {
        return score;
    }


}

