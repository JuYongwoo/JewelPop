using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultPopupPanel : BasePopupEffect
{
    private enum ResultPopupObjects
    {
        ResultRetryBtn
    }

    Dictionary<ResultPopupObjects, GameObject> resultPopupPanelObjs = new Dictionary<ResultPopupObjects, GameObject>();

    private void Start()
    {
        resultPopupPanelObjs = Util.MapEnumChildObjects<ResultPopupObjects, GameObject>(this.gameObject);

        GameManager.instance.actionManager.ShowResultPopupEvent -= SetActive;
        GameManager.instance.actionManager.ShowResultPopupEvent += SetActive;



        resultPopupPanelObjs[ResultPopupObjects.ResultRetryBtn].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        gameObject.SetActive(false);

    }
    private void OnDestroy()
    {
        GameManager.instance.actionManager.ShowResultPopupEvent -= SetActive;

    }

    private void SetActive()
    {
        gameObject.SetActive(true);
    }


}

