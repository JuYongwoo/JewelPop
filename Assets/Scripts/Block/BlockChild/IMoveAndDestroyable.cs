using UnityEngine;
public interface IMoveAndDestroyable //움직이고 터질 수 있는 블럭들은 반드시 이 인터페이스를 상속
{

    public void DestroySelf();// 블럭종류마다 파괴 모션이 다르므로 자식에서 구현 강제화
    public void Move(Transform targetParent);
    public void MoveAndBack(Transform targetParent);
    public void Turnoff();

}
