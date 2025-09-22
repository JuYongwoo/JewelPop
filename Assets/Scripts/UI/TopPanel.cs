using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPanel : MonoBehaviour
{
    Dictionary<string, GameObject> topPanelObjs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        topPanelObjs = Util.mapDictionaryInChildrenAll<GameObject>(this.gameObject);
    }

    private void Start()
    {
        ManagerObject.instance.actionManager.getJokerGoalTranform  = () => { return topPanelObjs["TopScoreText"].transform; };
        ManagerObject.instance.actionManager.setScoreUI = setScore;
    }

    private void setScore(int Score)
    {
        topPanelObjs["TopScoreText"].GetComponent<Text>().text = Score.ToString();
    }
}
