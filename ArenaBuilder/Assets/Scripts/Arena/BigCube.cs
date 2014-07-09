namespace Assets.Scripts.Arena
{
    public class BigCube : Deployable
    {
        public override void OnTick()
        {
        }

        public override string GetDisplayName()
        {
            return "BIG CUBE";
        }

        public new void Start()
        {
            base.Start();
            IsItActive = false;
        }
    }
}