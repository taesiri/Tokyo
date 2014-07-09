namespace Assets.Scripts.Arena
{
    public class CuteCube : Deployable
    {
        [InGameProperty(Name = "My Property2")]
        public string CustomProperty2 { get; set; }

        [InGameProperty(Name = "Custom Propery")]
        public bool CustomBoolProperty { get; set; }


        public override void OnTick()
        {
        }

        public new void Start()
        {
            base.Start();
            CustomProperty2 = "Change in Start!";
            IsItActive = true;
        }

        public override string GetDisplayName()
        {
            return "Cute Cube";
        }
    }
}