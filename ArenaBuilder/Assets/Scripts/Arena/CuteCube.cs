namespace Assets.Scripts.Arena
{
    public class CuteCube : Deployable
    {
        [InGameProperty(Name = "My Property2")]
        public string CustomProperty2 { get; set; }

        public override void OnTick()
        {
        }

        public void Start()
        {
            CustomProperty2 = "Change in Start!";
            IsItActive = true;
        }

        public override string GetDisplayName()
        {
            return "Cute Cube";
        }
    }
}