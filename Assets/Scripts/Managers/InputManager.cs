using UnityEngine;

public class InputManager
{

    private bool clicking = false;
    private GameObject startBlock = null;

    public void OnUpdate()
    {

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
        if (startBlock == null) //누르고 있는 동안 사라졌다
        {
            UnClick();
            return;
        }
        
        //맞은 객체가 바뀌는 순간 그 블럭과 교체한다.

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask mask = LayerMask.GetMask("Block");

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100, mask);
        if(hit.collider != null){

            if (hit.collider.gameObject != startBlock)
            {
                ManagerObject.instance.actionManager.inputBlockChangeAction(startBlock, hit.collider.gameObject);
                UnClick();
            }
        }


    }
}
