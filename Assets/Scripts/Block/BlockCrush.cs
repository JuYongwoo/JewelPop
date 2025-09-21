using System.Collections;
using UnityEngine;



public class BlockCrush : MonoBehaviour
{


    public void Start() //�Ϲ� ���� �ı� ���
    {
        StartCoroutine(DestroyAfterBlockCrushMotion());
    }

    private IEnumerator DestroyAfterBlockCrushMotion()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);

    }

}