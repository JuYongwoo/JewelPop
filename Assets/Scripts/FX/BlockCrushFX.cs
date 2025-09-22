using System.Collections;
using UnityEngine;



public class BlockCrushFX : MonoBehaviour
{


    public void Start() //일반 블럭의 파괴 모션
    {
        StartCoroutine(DestroyAfterBlockCrushMotion());
    }

    private IEnumerator DestroyAfterBlockCrushMotion()
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);

    }

}