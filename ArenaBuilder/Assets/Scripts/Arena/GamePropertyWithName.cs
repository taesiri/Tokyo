using System;
using System.Reflection;

namespace Assets.Scripts.Arena
{
    public class GamePropertyWithName
    {
        public PropertyInfo GamePropertyInfo;
        public string PropertyName;
        public Type Type;

        public GamePropertyWithName(PropertyInfo prop, string displayName)
        {
            GamePropertyInfo = prop;
            PropertyName = displayName;
        }
    }
}