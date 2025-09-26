using UnityEngine;
using UnityEngine.UI;

public class StageScene : MonoBehaviour
{
    public static StageScene instance;

    public MapManager mapManager = new MapManager();
    public LevelManager<JSONVars> levelManager = new LevelManager<JSONVars>();




    private bool clicking = false;
    private GameObject startBlock = null;

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

        AppManager.instance.actionManager.StageSceneInputController = () =>
        {
            if (AppManager.instance.actionManager.getIsInMotion() || AppManager.instance.actionManager.getIsBoardChanged()) return; //이동 중에는 입력 무시
            if (Input.GetMouseButtonDown(0))
            {
                Click();

            }
            else if (Input.GetMouseButtonUp(0))
            {
                UnClick();
            }

            if (clicking)
            {
                HandleClicking();

            }
        };

        levelManager.Init(AppManager.instance.resourceManager.levelDatasJSONHandle.Result.text); //현재 스테이지에 따라 맞는 레벨 데이터 로드(지금은 Level_1만)// dict 키값은 서버에서 받아오는 것으로
        mapManager.Init(levelManager.currentLevel);
        
        
        
        Time.timeScale = 1f; //스테이지 시작

    }


    private void Update()
    {
        mapManager.OnUpdate();

    }


    private void Click()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask mask = LayerMask.GetMask("Block");

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100, mask);
        if (hit.collider != null)
        {
            startBlock = hit.collider.gameObject;
            clicking = true;

        }

        if (startBlock == null) UnClick();
    }

    private void UnClick()
    {
        if (!clicking) return; //이미 블럭 전환해서 실행할 필요 X

        startBlock = null;
        clicking = false;

    }

    private void HandleClicking()
    {
        if (startBlock == null)
        {
            UnClick();
            return;
        }


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask mask = LayerMask.GetMask("Block");

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100, mask);
        if (hit.collider != null)
        {

            if (hit.collider.gameObject != startBlock)
            {
                AppManager.instance.actionManager.inputBlockChangeAction(startBlock, hit.collider.gameObject);
                UnClick();
            }
        }


    }
}
