namespace Assets.Scripts.Arena
{
    public class IgnoreMask
    {

    }

    public static class IngoreMaskUtils
    {
        public static IgnoreMask GenerateMask(this Deployable deployable)
        {
            return new IgnoreMask();
        }
    }
}