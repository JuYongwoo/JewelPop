namespace JYW.JewelPop.Managers
{
    public class InputManager
    {

        public void OnUpdate()
        {
            GameManager.instance.eventManager.OnStageSceneInputController();

        }

    }
}