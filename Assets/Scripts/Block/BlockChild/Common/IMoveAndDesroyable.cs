using System.Collections;
using UnityEngine;



public interface IMoveAndDesroyable //움직이고 터질 수 있는 블럭들은 반드시 이 인터페이스를 상속
{

    //TODO JYW IMoveAndDesroyable을 상속받는 블럭들의 이동 로직들이 상당히 비슷하다면 추상클래스로 전환하여 공통 함수를 사용하도록 해야할 수 있음

    public void DestroySelf();// 블럭종류마다 파괴 모션이 다르므로 자식에서 구현 강제화
    public void Move(Transform targetParent); //블럭 이동


}
