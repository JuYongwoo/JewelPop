using UnityEngine;



public class JokerBlock : BlockChild, ISpecial
{


    public void SpecialMotion()
    {
        //gameObject.GetComponent<Animator>().SetTrigger("open");


        Instantiate(AppManager.instance.resourceManager.jokerScoreFxHandle.Result, transform.position, Quaternion.identity);
        AppManager.instance.actionManager.DeltaScoreM(1);
    }



}
