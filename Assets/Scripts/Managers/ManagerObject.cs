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
        gameManager.setLevel(resourceManager.LevelDatasJSON[1].text); //���� ���������� ���� �´� ���� ������ �ε�(������ Level_1��)// �������� �޾ƿ��� �ɷ� �ٲ����
        mapManager.OnAwake(gameManager.currentLevel); //
    }

    private void Start()
    {;
        Application.targetFrameRate = 60; //������ ����Ʈ ���� (�ȵ���̵� 30������ ���� ����)
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
