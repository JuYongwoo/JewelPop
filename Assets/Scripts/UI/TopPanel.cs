using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TopPanel : MonoBehaviour
{
    private enum TopPanelObjects
    {
        TopCurrentScoreText,
        TopGoalBodyText,
        TopCurrentStageBodyText,
        TopCurrentScoreFrontImg
    }

    Dictionary<TopPanelObjects, GameObject> topPanelObjsMap = new Dictionary<TopPanelObjects, GameObject>();

    private void Awake()
    {
        topPanelObjsMap = Util.MapEnumChildObjects<TopPanelObjects, GameObject>(this.gameObject);
        GameManager.instance.eventManager.SetScoreUIEvent -= SetScore;
        GameManager.instance.eventManager.SetScoreUIEvent += SetScore;
        GameManager.instance.eventManager.SetCurrentStageUIEvent -= SetCurrentStage;
        GameManager.instance.eventManager.SetCurrentStageUIEvent += SetCurrentStage;
        GameManager.instance.eventManager.GetJokerGoalTranformEvent -= GetJokerGoalTranform;
        GameManager.instance.eventManager.GetJokerGoalTranformEvent += GetJokerGoalTranform;
    }

    private void Start()
    {

        Image image = topPanelObjsMap[TopPanelObjects.TopCurrentScoreFrontImg].GetComponent<Image>();
        image.fillMethod = Image.FillMethod.Vertical;
        image.fillOrigin = 0; // 0 = 밑에서 위로, 1 = 위에서 아래로

    }

    private void OnDestroy()
    {
        GameManager.instance.eventManager.SetScoreUIEvent -= SetScore;
        GameManager.instance.eventManager.SetCurrentStageUIEvent -= SetCurrentStage;
        GameManager.instance.eventManager.GetJokerGoalTranformEvent -= GetJokerGoalTranform;
    }

    private Transform GetJokerGoalTranform()
    {
        topPanelObjsMap.TryGetValue(TopPanelObjects.TopCurrentScoreText, out var scoreText);
        return scoreText.transform;
    }

    private void SetCurrentStage(int stage)
    {
        topPanelObjsMap.TryGetValue(TopPanelObjects.TopCurrentStageBodyText, out var bodyText);
        bodyText.GetComponent<Text>().text = stage.ToString();
    }

    private void SetScore(int currentScore, int goalScore)
    {

        topPanelObjsMap.TryGetValue(TopPanelObjects.TopCurrentScoreText, out var scoreText);
        topPanelObjsMap.TryGetValue(TopPanelObjects.TopGoalBodyText, out var bodyText);
        topPanelObjsMap.TryGetValue(TopPanelObjects.TopCurrentScoreFrontImg, out var frontImg);

        scoreText.GetComponent<Text>().text = currentScore.ToString();
        bodyText.GetComponent<Text>().text = (goalScore - currentScore) >= 0 ? (goalScore - currentScore).ToString() : "0";
        frontImg.GetComponent<Image>().fillAmount = Mathf.Clamp01((float)currentScore / (float)goalScore);
    }

}
