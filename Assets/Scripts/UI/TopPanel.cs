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
        AppManager.instance.actionManager.setScoreUI -= setScore;
        AppManager.instance.actionManager.setScoreUI += setScore;
        AppManager.instance.actionManager.setCurrentStageUI -= setCurrentStage;
        AppManager.instance.actionManager.setCurrentStageUI += setCurrentStage;
    }

    private void Start()
    {
        AppManager.instance.actionManager.getJokerGoalTranform -= getJokerGoalTranform;
        AppManager.instance.actionManager.getJokerGoalTranform += getJokerGoalTranform;

        Image image = topPanelObjs[TopPanelObjects.TopCurrentScoreFrontImg].GetComponent<Image>();
        image.fillMethod = Image.FillMethod.Vertical;
        image.fillOrigin = 0; // 0 = 밑에서 위로, 1 = 위에서 아래로

    }

    private void OnDestroy()
    {
        AppManager.instance.actionManager.setScoreUI -= setScore;
        AppManager.instance.actionManager.setCurrentStageUI -= setCurrentStage;
        AppManager.instance.actionManager.getJokerGoalTranform -= getJokerGoalTranform;
    }

    private Transform getJokerGoalTranform()
    {
        topPanelObjs.TryGetValue(TopPanelObjects.TopCurrentScoreText, out var scoreText);
        return scoreText.transform;
    }

    private void setCurrentStage(int stage)
    {
        topPanelObjs.TryGetValue(TopPanelObjects.TopCurrentStageBodyText, out var bodyText);
        bodyText.GetComponent<Text>().text = stage.ToString();
    }

    private void setScore(int currentScore, int goalScore)
    {

        topPanelObjs.TryGetValue(TopPanelObjects.TopCurrentScoreText, out var scoreText);
        topPanelObjs.TryGetValue(TopPanelObjects.TopGoalBodyText, out var bodyText);
        topPanelObjs.TryGetValue(TopPanelObjects.TopCurrentScoreFrontImg, out var frontImg);

        scoreText.GetComponent<Text>().text = currentScore.ToString();
        bodyText.GetComponent<Text>().text = (goalScore-currentScore) >= 0 ? (goalScore - currentScore).ToString() : "0";
        frontImg.GetComponent<Image>().fillAmount = Mathf.Clamp01((float)currentScore / (float)goalScore);
    }

}
