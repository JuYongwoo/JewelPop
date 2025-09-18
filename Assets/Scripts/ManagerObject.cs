using UnityEngine;

public class ManagerObject : MonoBehaviour
{
    public static ManagerObject instance;
    public MapManager mapManager = new MapManager();
    public ResourceManager resourceManager = new ResourceManager();
    public ActionManager actionManager = new ActionManager();
    public InputManager inputManager = new InputManager();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);



        resourceManager.OnAwake();
        mapManager.OnAwake();

    }



        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
    {
        Screen.SetResolution(900, 1600, false);

    }

    // Update is called once per frame
    void Update()
    {
        inputManager.OnUpdate();
    }
}
