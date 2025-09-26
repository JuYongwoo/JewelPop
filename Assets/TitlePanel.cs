using System.Collections.Generic;
using UnityEngine;

public class TitlePanel : MonoBehaviour
{

    private Dictionary<string, GameObject> uiElements = new Dictionary<string, GameObject>();
    private void Awake()
    {
        uiElements = Util.MapAllChildObjects<GameObject>(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiElements["StartBtn"].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Stage");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
