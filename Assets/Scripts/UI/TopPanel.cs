using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPanel : MonoBehaviour
{
    Dictionary<string, GameObject> topPanelObjs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        topPanelObjs = Util.mapDictionaryInChildrenAll<GameObject>(this.gameObject);
        ActionManager.setScoreUI = setScore;
        ActionManager.setCurrentStageUI = setCurrentStage;
    }

    private void Start()
    {
        ManagerObject.instance.actionManager.getJokerGoalTranform  = () => { return topPanelObjs["TopCurrentScoreText"].transform; };

        Image image = topPanelObjs["TopCurrentScoreFrontImg"].GetComponent<Image>();
        image.fillMethod = Image.FillMethod.Vertical;
        image.fillOrigin = 0; // 0 = 밑에서 위로, 1 = 위에서 아래로

    }
    private void setCurrentStage(int stage)
    {
        topPanelObjs["TopCurrentStageBodyText"].GetComponent<Text>().text = stage.ToString();
    }

    private void setScore(int currentScore, int goalScore)
    {
        topPanelObjs["TopCurrentScoreText"].GetComponent<Text>().text = currentScore.ToString();
        topPanelObjs["TopGoalBodyText"].GetComponent<Text>().text = (goalScore-currentScore) >= 0 ? (goalScore - currentScore).ToString() : "0";
        topPanelObjs["TopCurrentScoreFrontImg"].GetComponent<Image>().fillAmount = Mathf.Clamp01((float)currentScore / (float)goalScore);
    }

}
