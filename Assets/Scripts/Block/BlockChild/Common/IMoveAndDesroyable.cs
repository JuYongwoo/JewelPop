using System.Collections;
using UnityEngine;



public interface IMoveAndDesroyable //�����̰� ���� �� �ִ� ������ �ݵ�� �� �������̽��� ���
{

    //TODO JYW IMoveAndDesroyable�� ��ӹ޴� ������ �̵� �������� ����� ����ϴٸ� �߻�Ŭ������ ��ȯ�Ͽ� ���� �Լ��� ����ϵ��� �ؾ��� �� ����

    public void DestroySelf();// ���������� �ı� ����� �ٸ��Ƿ� �ڽĿ��� ���� ����ȭ
    public void Move(Transform targetParent); //�� �̵�


}
