using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultPopupPanel : BasePopupEffect
{
    private enum ResultPopupPanelObjects
    {
        ResultRetryBtn
    }

    Dictionary<ResultPopupPanelObjects, GameObject> resultPopupPanelObjsMap = new Dictionary<ResultPopupPanelObjects, GameObject>();

    private void Awake()
    {
        resultPopupPanelObjsMap = Util.MapEnumChildObjects<ResultPopupPanelObjects, GameObject>(this.gameObject);
        resultPopupPanelObjsMap[ResultPopupPanelObjects.ResultRetryBtn].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
        GameManager.instance.eventManager.ShowResultPopupEvent -= SetActive;
        GameManager.instance.eventManager.ShowResultPopupEvent += SetActive;
    }

    private void Start()
    {
        gameObject.SetActive(false);

    }
    private void OnDestroy()
    {
        GameManager.instance.eventManager.ShowResultPopupEvent -= SetActive;

    }

    private void SetActive()
    {
        gameObject.SetActive(true);
    }


}

