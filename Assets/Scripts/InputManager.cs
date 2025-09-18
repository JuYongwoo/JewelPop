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
        Debug.Log("0");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask mask = LayerMask.GetMask("Block");

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100, mask);
        if (hit.collider != null)
        {
            startBlock = hit.collider.gameObject;

        }
        clicking = true;
    }

    private void UnClick()
    {
        if (!clicking) return; //이미 블럭 전환해서 실행할 필요 X

        Debug.Log("2");
        startBlock = null;
        clicking = false;

    }

    private void HandleClicking()
    {

        //맞은 객체가 바뀌는 순간 그 블럭과 교체한다.

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask mask = LayerMask.GetMask("Block");

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, 100, mask);
        if(hit.collider != null){

            if (hit.collider.gameObject != startBlock)
            {
                //여기서 두 오브젝트가 바꿨을때 터지는지 확인, ActionManager를 통해서 Util과 연락
                //바꿔서 터지면 바꾸고 터트린다, 안터지는애면 잠깐갔다온다(연출만)
                Debug.Log("전환");
                ManagerObject.instance.actionManager.blockChangeAction(hit.collider.gameObject, startBlock);
                UnClick();
            }
        }


    }
}
