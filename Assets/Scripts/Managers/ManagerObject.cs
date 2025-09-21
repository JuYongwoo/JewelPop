using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

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
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);


        // ResourceManager 리소스 로딩 (완료될 때까지 대기)
        resourceManager.OnAwake();
        // 여기까지 오면 blockPrefabs 무조건 로드 완료됨
        mapManager.OnAwake();
    }

    private void Start()
    {
        Application.targetFrameRate = 60; // 프레임 레이트 설정 (안드로이드 30프레임 저하 차단)
        mapManager.OnStart(); // blockPrefabs 로딩 완료 후라 안전
    }

    private void Update()
    {
        inputManager.OnUpdate();
        mapManager.OnUpdate();
    }
}
