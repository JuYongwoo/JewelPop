
public class GameManager
{

    private int score = 0;

    public void deltaScore(int delta) //���� ������ �ݵ�� �̰��� ���
    {
        score += delta;
        ManagerObject.instance.actionManager.setScoreUI(score);
    }

    public int getScore()
    {
        return score;
    }


}

