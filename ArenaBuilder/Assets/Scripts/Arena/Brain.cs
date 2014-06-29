using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class Brain : MonoBehaviour
    {
        private readonly string[] _menuStrings = {"Navigation", "Creation"};
        public GUILocationHelper Location = new GUILocationHelper();
        private Matrix4x4 _guiMatrix;
        private int _menuSelectedIndex;

        public void Start()
        {

            Location.PointLocation = GUILocationHelper.Point.Center;
            Location.UpdateLocation();

            Vector2 ratio = Location.GuiOffset;
            _guiMatrix = Matrix4x4.identity;
            _guiMatrix.SetTRS(new Vector3(1, 1, 1), Quaternion.identity, new Vector3(ratio.x, ratio.y, 1));
        }


        public void OnGUI()
        {
            GUI.matrix = _guiMatrix;
            _menuSelectedIndex = GUI.Toolbar(new Rect(0, 0, 300, 75), _menuSelectedIndex, _menuStrings);
            GUI.matrix = Matrix4x4.identity;
        }
    }
}