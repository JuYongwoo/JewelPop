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
        //타이틀 씬에서 AppManager가 이미 초기화 되어있어야 하는데 현재 Title 씬 부재
        //때문에 지금은 StageManager는 Start에서 초기화


        levelManager.Init(AppManager.instance.resourceManager.LevelDatasJSON[1].text); //현재 스테이지에 따라 맞는 레벨 데이터 로드(지금은 Level_1만)// dict 키값은 서버에서 받아오는 것으로
        mapManager.Init(levelManager.currentLevel);
        
        
        
        Time.timeScale = 1f; //스테이지 시작

    }


    private void Update()
    {
        mapManager.OnUpdate();

    }
}
