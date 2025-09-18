using UnityEngine;
using UnityEngine.UI;

    public class HexBlock : MonoBehaviour
    {

        public Vector2Int girdPos = new Vector2Int();

        private int colorIndex;

        public void setPosition(Vector2Int pos) //유니티 상의 위치가 아닌 그리드 상의 x,y 좌표
        {
            girdPos= pos;

        }

        public void SetColor(int index)
        {
            colorIndex = index;
            if (ManagerObject.instance.resourceManager.blockTextures != null && index >= 0 && index < ManagerObject.instance.resourceManager.blockTextures.objects.Length)
                this.gameObject.GetComponent<SpriteRenderer>().sprite = ManagerObject.instance.resourceManager.blockTextures.objects[index];
        }

        public bool IsSameColor(HexBlock other)
        {
            return other != null && other.colorIndex == colorIndex;
        }
}
