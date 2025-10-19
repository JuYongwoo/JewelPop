using System.Collections;
using UnityEngine;

public class BlockCrushFX : MonoBehaviour
{


    private void Start() //일반 블럭의 파괴 모션
    {
        GameManager.instance.actionManager.OnPlayAudioClip(GameManager.instance.resourceManager.gameSoundClipsHandles[Sounds.Block3SFX].Result, 0.5f, false);
        StartCoroutine(DestroyAfterBlockCrushMotion());
    }

    private IEnumerator DestroyAfterBlockCrushMotion()
    {
        yield return new WaitForSeconds(0.8f);
        GameManager.instance.poolManager.DestroyPooled(gameObject);

    }

}