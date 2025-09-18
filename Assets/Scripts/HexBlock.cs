using UnityEngine;
using UnityEngine.UI;

    public class HexBlock : MonoBehaviour
    {

        public Vector2Int girdPos = new Vector2Int();

        private int colorIndex;

        public void setPosition(Vector2Int pos) //����Ƽ ���� ��ġ�� �ƴ� �׸��� ���� x,y ��ǥ
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
