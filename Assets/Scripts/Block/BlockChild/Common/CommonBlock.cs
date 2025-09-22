using System.Collections;
using UnityEngine;



public class CommonBlock : BlockChild, IMoveAndDesroyable
{

    [SerializeField]
    private float speed = 2f; //�̵� �ӵ�


    public void DestroySelf() //�Ϲ� ���� �ı� ���
    {
        Instantiate(ManagerObject.instance.resourceManager.blockCrushFxPrefabs["BlockCrush_"+ GetBlockType()], transform.position, Quaternion.identity); //�ı� ����Ʈ ��ȯ �� �ڽ� �ı�
        DestroyImmediate(gameObject);

    }


    public void Move(Transform targetParent)
    {

        var aPos = new Vector3(transform.parent.position.x, transform.transform.parent.position.y, transform.position.z);
        var bPos = new Vector3(targetParent.position.x, targetParent.position.y, transform.position.z);

        StartCoroutine(moveCoroutine(targetParent, aPos, bPos));
    }


    // �ڽ� Transform�� ���� �޾Ƽ� �̵� (�θ𿡼� �ٽ� GetChild(0) ���� ����)
    private IEnumerator moveCoroutine(Transform targetParent, Vector3 startPos, Vector3 endPos)
    {
        transform.SetParent(targetParent, true);
        float t = 0f;
        float endDist = 0.01f * 0.01f;

        ManagerObject.instance.actionManager.setIsInMotion(true);
        ManagerObject.instance.actionManager.setIsBoardChanged(true);

        while (true)
        {
            if (transform == null) yield break; //�ı��� ���
            t += Time.deltaTime * speed;
            if(t > 1f) t = 1f;
            transform.position = Vector3.Lerp(startPos, endPos, t);
//            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            if ((transform.position - endPos).sqrMagnitude <= endDist) break;
            yield return null;
        }

        ManagerObject.instance.actionManager.setIsInMotion(false);
        transform.position = endPos;

    }

    public void moveAndBack(Transform targetParent)
    {
        var aPos = new Vector3(transform.parent.position.x, transform.transform.parent.position.y, transform.position.z);
        var bPos = new Vector3(targetParent.position.x, targetParent.position.y, transform.position.z);
        StartCoroutine(moveAndBackCoroutine(targetParent, aPos, bPos));
    }

    private IEnumerator moveAndBackCoroutine(Transform targetParent, Vector3 startPos, Vector3 endPos)
    {
        float t = 0f;
        float endDist = 0.01f * 0.01f;

        ManagerObject.instance.actionManager.setIsInMotion(true);
        ManagerObject.instance.actionManager.setIsBoardChanged(true);

        while (true)
        {
            if (transform == null) yield break; //�ı��� ���
            t += Time.deltaTime * speed;
            if (t > 1f) t = 1f;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            //            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            if ((transform.position - endPos).sqrMagnitude <= endDist) break;
            yield return null;
        }

        while (true)
        {
            if (transform == null) yield break; //�ı��� ���
            t += Time.deltaTime * speed;
            if (t > 1f) t = 1f;
            transform.position = Vector3.Lerp(endPos, startPos, t);
            //            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            if ((transform.position - startPos).sqrMagnitude <= endDist) break;
            yield return null;
        }



        ManagerObject.instance.actionManager.setIsInMotion(false);
        transform.position = endPos;

    }


}