using System;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    [Serializable]
    public class IntVector2
    {
        [SerializeField] public int X;

        [SerializeField] public int Y;

        public IntVector2()
        {
            X = 0;
            Y = 0;
        }

        public IntVector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return String.Format("X: {0}, Y: {1}", X, Y);
        }
    }
}