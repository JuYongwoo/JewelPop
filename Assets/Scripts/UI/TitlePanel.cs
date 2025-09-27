using System.Collections.Generic;
using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    public enum TitleUI
    {
        TitleStartBtn
    }

    private Dictionary<TitleUI, GameObject> uiElements = new Dictionary<TitleUI, GameObject>();
    private void Awake()
    {
        uiElements = Util.MapEnumChildObjects<TitleUI, GameObject>(this.gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiElements[TitleUI.TitleStartBtn].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Stage");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
