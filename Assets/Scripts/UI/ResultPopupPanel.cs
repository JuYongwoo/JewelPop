using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultPopupPanel : BasePopupEffect
{
    public enum ResultPopupObjects
    {
        ResultRetryBtn
    }

    Dictionary<ResultPopupObjects, GameObject> resultPopupPanelObjs = new Dictionary<ResultPopupObjects, GameObject>();


    protected override void OnEnable()
    {
        base.OnEnable();
    }


    private void Start()
    {
        resultPopupPanelObjs = Util.MapEnumChildObjects<ResultPopupObjects, GameObject>(this.gameObject);

        AppManager.instance.actionManager.showResultPopup -= setActive;
        AppManager.instance.actionManager.showResultPopup += setActive;



        resultPopupPanelObjs[ResultPopupObjects.ResultRetryBtn].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        this.gameObject.SetActive(false);

    }
    private void OnDestroy()
    {
        AppManager.instance.actionManager.showResultPopup -= setActive;

    }

    private void setActive()
    {
        this.gameObject.SetActive(true);
    }


}

