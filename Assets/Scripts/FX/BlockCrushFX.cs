using System.Collections;
using UnityEngine;



public class BlockCrushFX : MonoBehaviour
{


    public void Start() //�Ϲ� ���� �ı� ���
    {
        StartCoroutine(DestroyAfterBlockCrushMotion());
    }

    private IEnumerator DestroyAfterBlockCrushMotion()
    {
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);

    }

}