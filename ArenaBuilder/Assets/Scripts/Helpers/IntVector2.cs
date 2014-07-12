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
                throw new Exception();
            }

            X = Convert.ToInt32(parts[0]);
            Y = Convert.ToInt32(parts[1]);
        }

        public override string ToString()
        {
            return String.Format("{0},{1}", X, Y);
        }


        public static IntVector2 operator +(IntVector2 iv1, IntVector2 iv2)
        {
            return new IntVector2(iv1.X + iv2.X, iv1.Y + iv2.Y);
        }

        public static IntVector2 operator -(IntVector2 iv1, IntVector2 iv2)
        {
            return new IntVector2(iv1.X - iv2.X, iv1.Y - iv2.Y);
        }
    }
}