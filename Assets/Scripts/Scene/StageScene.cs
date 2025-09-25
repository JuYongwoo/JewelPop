using UnityEngine;

public class StageScene : MonoBehaviour
{
    public static StageScene instance;

    public MapManager mapManager = new MapManager();
    public LevelManager<JSONVars> levelManager = new LevelManager<JSONVars>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }


    }
    private void Start()
    {


        //JYW
        //Ÿ��Ʋ ������ AppManager�� �̹� �ʱ�ȭ �Ǿ��־�� �ϴµ� ���� Title �� ����
        //������ ������ StageManager�� Start���� �ʱ�ȭ


        levelManager.Init(AppManager.instance.resourceManager.LevelDatasJSON[1].text); //���� ���������� ���� �´� ���� ������ �ε�(������ Level_1��)// dict Ű���� �������� �޾ƿ��� ������
        mapManager.Init(levelManager.currentLevel);
        
        
        
        Time.timeScale = 1f; //�������� ����

    }


    private void Update()
    {
        mapManager.OnUpdate();

    }
}
