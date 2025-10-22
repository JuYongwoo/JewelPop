using System.Collections.Generic;
using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    private enum TitlePanelObjs
    {
        TitleStartBtn
    }

    private Dictionary<TitlePanelObjs, GameObject> titlePanelObjsMap = new Dictionary<TitlePanelObjs, GameObject>();
    private void Awake()
    {
        titlePanelObjsMap = Util.MapEnumChildObjects<TitlePanelObjs, GameObject>(this.gameObject);
        titlePanelObjsMap[TitlePanelObjs.TitleStartBtn].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Stage");
        });
    }

}
