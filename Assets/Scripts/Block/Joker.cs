using UnityEngine;



public class Joker : BlockChild, SpecialBlock
{

    //deprecated - 이벤트 방식에서 직접 호출 방식으로 변경
    //public event Action motionEvent;


    public void specialMotion()
    {
        //motionEvent += MotionEvent;
        gameObject.GetComponent<Animator>().SetTrigger("open");
        Instantiate(ManagerObject.instance.resourceManager.jokerScoreFxPrefab, transform.position, Quaternion.identity);
        ManagerObject.instance.gameManager.deltaScore(1);
    }
    private void MotionEvent()
    {
        gameObject.GetComponent<Animator>().SetTrigger("open");
        Instantiate(ManagerObject.instance.resourceManager.jokerScoreFxPrefab, transform.position, Quaternion.identity);
        //motionEvent -= MotionEvent; //한번만 실행되도록 이벤트에서 제거

    }



}
