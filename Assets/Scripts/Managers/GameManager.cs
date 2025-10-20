using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public EventManager eventManager = new EventManager();
    public ResourceManager resourceManager = new ResourceManager();
    public SoundManager soundManager = new SoundManager();
    public InputManager inputManager = new InputManager();
    public PoolManager poolManager = new PoolManager();

    private void Awake()
    {
        makeInstanceSelf();

        resourceManager.StartPreload();
        soundManager.OnStart();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void OnDestroy()
    {
        soundManager.OnDestroy();
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
