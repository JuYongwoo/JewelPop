using UnityEngine;



public class Joker : BlockChild, SpecialBlock
{

    //deprecated - �̺�Ʈ ��Ŀ��� ���� ȣ�� ������� ����
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
        //motionEvent -= MotionEvent; //�ѹ��� ����ǵ��� �̺�Ʈ���� ����

    }



}
