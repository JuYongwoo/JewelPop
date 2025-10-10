using UnityEngine;

public class AppManager : MonoBehaviour
{
    public static AppManager instance;
    public ActionManager actionManager = new ActionManager();
    public ResourceManager resourceManager = new ResourceManager();
    public SoundManager soundManager = new SoundManager();
    public InputManager inputManager = new InputManager();
    public PoolManager poolManager = new PoolManager();

    private void Awake()
    {
        makeInstanceSelf();

        resourceManager.StartPreload();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;


    }

    private void Update()
    {
        inputManager.OnUpdate();
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
