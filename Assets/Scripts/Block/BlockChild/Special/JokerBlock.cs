using UnityEngine;



public class JokerBlock : BlockChild, ISpecial
{


    public void SpecialMotion()
    {
        //gameObject.GetComponent<Animator>().SetTrigger("open");
        Instantiate(AppManager.instance.resourceManager.jokerScoreFxPrefab, transform.position, Quaternion.identity);
        StageScene.instance.levelManager.deltaScore(1);
    }



}
