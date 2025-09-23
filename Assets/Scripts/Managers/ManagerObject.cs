using UnityEngine;

public class ManagerObject : MonoBehaviour
{
    public static ManagerObject instance;
    public MapManager mapManager = new MapManager();
    public ResourceManager resourceManager = new ResourceManager();
    public ActionManager actionManager = new ActionManager();
    public InputManager inputManager = new InputManager();
    public LevelManager<JSONVars> gameManager = new LevelManager<JSONVars>();

    private void Awake()
    {
        makeInstanceSelf();
        resourceManager.OnAwake();
        gameManager.setLevel(resourceManager.LevelDatasJSON[1].text); //현재 스테이지에 따라 맞는 레벨 데이터 로드(지금은 Level_1만)// 서버에서 받아오는 걸로 바꿔야함
        mapManager.OnAwake(gameManager.currentLevel); //
    }

    private void Start()
    {;
        Application.targetFrameRate = 60; //프레임 레이트 설정 (안드로이드 30프레임 저하 차단)
        gameManager.OnStart();
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
