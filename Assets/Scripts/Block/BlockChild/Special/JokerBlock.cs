using UnityEngine;
public class JokerBlock : BlockChild, ISpecial
{
    public void SpecialMotion()
    {
        //gameObject.GetComponent<Animator>().SetTrigger("open");

        GameManager.instance.poolManager.Spawn(GameManager.instance.resourceManager.jokerScoreFxHandle.Result, transform.position, Quaternion.identity);
        GameManager.instance.eventManager.OnDeltaScore(1);
    }

}
