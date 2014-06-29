using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class GUILocationHelper
    {
        public enum Point
        {
            TopLeft,
            TopRigjt,
            BottomLeft,
            BottomRight,
            Center
        }

        public Vector2 GuiOffset;
        public Vector2 Offset;
        public float OriginalHeigth = 800f;
        public float OriginalWidth = 1280f;
        public Point PointLocation = Point.TopLeft;

        public void UpdateLocation()
        {
            switch (PointLocation)
            {
                case Point.TopLeft:
                    Offset = new Vector2(0, 0);
                    break;
                case Point.TopRigjt:
                    Offset = new Vector2(OriginalWidth, 0);
                    break;
                case Point.BottomLeft:
                    Offset = new Vector2(0, OriginalHeigth);
                    break;
                case Point.BottomRight:
                    Offset = new Vector2(OriginalWidth, OriginalHeigth);
                    break;
                case Point.Center:
                    Offset = new Vector2(OriginalWidth/2f, OriginalHeigth/2f);
                    break;
            }

            GuiOffset.x = Screen.width/OriginalWidth;
            GuiOffset.y = Screen.height/OriginalHeigth;
        }
    }
}