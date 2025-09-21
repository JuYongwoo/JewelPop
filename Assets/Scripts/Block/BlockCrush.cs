using System.Collections;
using UnityEngine;



public class BlockCrush : MonoBehaviour
{


    public void Start() //일반 블럭의 파괴 모션
    {
        StartCoroutine(DestroyAfterBlockCrushMotion());
    }

    private IEnumerator DestroyAfterBlockCrushMotion()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);

    }

}