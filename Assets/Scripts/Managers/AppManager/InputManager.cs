using UnityEngine;

public class InputManager
{

    private bool clicking = false;
    private GameObject startBlock = null;

    public void OnUpdate()
    {
        if(AppManager.instance.actionManager.getIsInMotion() || AppManager.instance.actionManager.getIsBoardChanged()) return; //이동 중에는 입력 무시
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

        if(startBlock == null) UnClick();
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
        if(hit.collider != null){

            if (hit.collider.gameObject != startBlock)
            {
                AppManager.instance.actionManager.inputBlockChangeAction(startBlock, hit.collider.gameObject);
                UnClick();
            }
        }


    }
}
