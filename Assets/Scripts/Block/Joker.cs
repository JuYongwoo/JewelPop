using UnityEngine;



public class Joker : BlockChild
{
    private bool ismotionactive = false;

    private void Update()
    {

        if (ismotionactive)
        {
            jokerMotion();
        }
    }

    public void motionStart()
    {
        ismotionactive = true;
        ManagerObject.instance.gameManager.deltaScore(1);
    }
    private void jokerMotion()
    {
        gameObject.GetComponent<Animator>().SetTrigger("open");
        ismotionactive = false;

    }

    public override void DestroySelf()
    {
        //조커는 사라지지 않음
        return;
    }
}
