using System.Collections;
using UnityEngine;



public interface IMoveAndDesroyable //�����̰� ���� �� �ִ� ������ �ݵ�� �� �������̽��� ���
{
    public void DestroySelf();// ���������� �ı� ����� �ٸ��Ƿ� �ڽĿ��� ���� ����ȭ
    public void move(Transform targetParent); //�� �̵�


}
