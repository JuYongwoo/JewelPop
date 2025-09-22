using System.Collections;
using UnityEngine;



public interface CommonBlockInterface //일반 유형의 블럭들
{
    public void DestroySelf();// 블럭종류마다 파괴 모션이 다르므로 자식에서 구현 강제화



}
