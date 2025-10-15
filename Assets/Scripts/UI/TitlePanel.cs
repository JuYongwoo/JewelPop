using System.Collections.Generic;
using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    private enum TitleUI
    {
        TitleStartBtn
    }

    private Dictionary<TitleUI, GameObject> uiElements = new Dictionary<TitleUI, GameObject>();
    private void Awake()
    {
        uiElements = Util.MapEnumChildObjects<TitleUI, GameObject>(this.gameObject);
    }

    void Start()
    {
        uiElements[TitleUI.TitleStartBtn].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Stage");
        });
    }

}
