using UnityEngine;



public class JokerBlock : BlockChild, ISpecial
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



}
