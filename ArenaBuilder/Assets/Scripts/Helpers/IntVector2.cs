using System;
using System.IO;
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

        public IntVector2(float x, float y)
        {
            X = (int) x;
            Y = (int) y;
        }

        public IntVector2(string text)
        {
            string[] parts = text.Split(',');

            if (parts.Length != 2)
            {
                throw new InvalidDataException();
            }

            X = Convert.ToInt32(parts[0]);
            Y = Convert.ToInt32(parts[1]);
        }

        public override string ToString()
        {
            return String.Format("{0},{1}", X, Y);
        }
    }
}