using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TopPanel : MonoBehaviour
{
public enum TopPanelObjects
{
    TopCurrentScoreText,
    TopGoalBodyText,
    TopCurrentStageBodyText,
    TopCurrentScoreFrontImg
}

    Dictionary<TopPanelObjects, GameObject> topPanelObjs = new Dictionary<TopPanelObjects, GameObject>();

    private void Awake()
    {
        topPanelObjs = Util.MapEnumChildObjects<TopPanelObjects, GameObject>(this.gameObject);
        ActionManager.setScoreUI = setScore;
        ActionManager.setCurrentStageUI = setCurrentStage;
    }

    private void Start()
    {
        AppManager.instance.actionManager.getJokerGoalTranform  = () => { return topPanelObjs[TopPanelObjects.TopCurrentScoreText].transform; };

        Image image = topPanelObjs[TopPanelObjects.TopCurrentScoreFrontImg].GetComponent<Image>();
        image.fillMethod = Image.FillMethod.Vertical;
        image.fillOrigin = 0; // 0 = 밑에서 위로, 1 = 위에서 아래로

    }
    private void setCurrentStage(int stage)
    {
        topPanelObjs[TopPanelObjects.TopCurrentStageBodyText].GetComponent<Text>().text = stage.ToString();
    }

    private void setScore(int currentScore, int goalScore)
    {
        topPanelObjs[TopPanelObjects.TopCurrentScoreText].GetComponent<Text>().text = currentScore.ToString();
        topPanelObjs[TopPanelObjects.TopGoalBodyText].GetComponent<Text>().text = (goalScore-currentScore) >= 0 ? (goalScore - currentScore).ToString() : "0";
        topPanelObjs[TopPanelObjects.TopCurrentScoreFrontImg].GetComponent<Image>().fillAmount = Mathf.Clamp01((float)currentScore / (float)goalScore);
    }

}
