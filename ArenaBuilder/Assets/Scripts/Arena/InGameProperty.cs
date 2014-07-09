using System;

namespace Assets.Scripts.Arena
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InGameProperty : Attribute
    {
        public string Name { get; set; }
    }
}