using System.Collections;
using UnityEngine;



public class BlockCrushFX : MonoBehaviour
{


    private void Start() //일반 블럭의 파괴 모션
    {
        AppManager.instance.soundManager.PlaySound(Sounds.Block3SFX, 0.25f, false);
        StartCoroutine(DestroyAfterBlockCrushMotion());
    }

    private IEnumerator DestroyAfterBlockCrushMotion()
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);

    }

}