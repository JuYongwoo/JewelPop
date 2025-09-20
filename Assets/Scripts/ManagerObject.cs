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


        // ResourceManager ���ҽ� �ε� (�Ϸ�� ������ ���)
        resourceManager.OnAwake();
        // ������� ���� blockPrefabs ������ �ε� �Ϸ��
        mapManager.OnAwake();
    }

    private void Start()
    {
        Screen.SetResolution(900, 1600, false);
        mapManager.OnStart(); // blockPrefabs �ε� �Ϸ� �Ķ� ����
    }

    private void Update()
    {
        inputManager.OnUpdate();
    }
}
