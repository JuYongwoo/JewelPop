using JYW.JewelPop.StageScene;
using UnityEngine;

namespace JYW.JewelPop.Block
{

    public class BlockParent : MonoBehaviour
    {

        private YX yx;

        public void SetGridPositionYX(YX yx) //유니티 상의 위치가 아닌 그리드 상의 x,y 좌표
        {
            this.yx = yx;

        }

        public YX GetGridPositionYX() // //
        {
            return yx;
        }


        public void SetUnityPositionYX((float y, float x) yx)
        {
            gameObject.transform.localPosition = new Vector2(yx.x, yx.y);
        }
    }
}