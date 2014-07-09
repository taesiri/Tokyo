using System;
using System.Collections.Generic;
using System.Reflection;

namespace Assets.Scripts.Arena
{
    public class GamePropertyList
    {
        public List<PropertyInfo> PropertyInfos;
        public List<string> PropertyName;
        public Type Type;

        public void Add(PropertyInfo prop, string displayName)
        {
            PropertyInfos.Add(prop);
            PropertyName.Add(displayName);
        }
    }
}