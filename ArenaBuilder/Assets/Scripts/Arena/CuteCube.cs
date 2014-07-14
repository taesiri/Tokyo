namespace Assets.Scripts.Arena
{
    public class CuteCube : Deployable
    {
        [InGameProperty(Name = "Custom Propery")]
        public bool CustomBoolProperty { get; set; }


        public override void OnTick()
        {
        }

        public new void Start()
        {
            base.Start();
        }

        public override string GetDisplayName()
        {
            return "Cute Cube";
        }
    }
}