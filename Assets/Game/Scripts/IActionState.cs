
namespace Assets.Game.Scripts
{
    public interface IActionState
    {
        void SetId(int id);
        void SetAction(IAction action);

        void Setup();
        void Update();
        void Cleanup();
    }
}
