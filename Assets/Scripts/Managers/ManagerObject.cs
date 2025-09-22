using UnityEngine;

public class ManagerObject : MonoBehaviour
{
    public static ManagerObject instance;
    public MapManager mapManager = new MapManager();
    public ResourceManager resourceManager = new ResourceManager();
    public ActionManager actionManager = new ActionManager();
    public InputManager inputManager = new InputManager();
    public GameManager gameManager = new GameManager();

    private void Awake()
    {
        makeInstanceSelf();
        resourceManager.OnAwake();
        mapManager.OnAwake();
    }

    private void Start()
    {
        Application.targetFrameRate = 60; //프레임 레이트 설정 (안드로이드 30프레임 저하 차단)
    }

    private void Update()
    {
        inputManager.OnUpdate();
        mapManager.OnUpdate();
    }
    
    private void makeInstanceSelf()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
