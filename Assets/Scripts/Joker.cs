using System.Collections;
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
    }
    private void jokerMotion()
    {
        gameObject.GetComponent<Animator>().SetTrigger("open");
        ismotionactive = false;

    }
}
