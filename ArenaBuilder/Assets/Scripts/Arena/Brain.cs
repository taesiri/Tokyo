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

        private bool isDown;


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

            if (GUI.RepeatButton(new Rect(0, 100, 150, 150), "Element"))
            {
                isDown = true;
                Debug.Log("Clicked!");
               
            }

            GUI.matrix = Matrix4x4.identity;
        }

        public void Update()
        {
#if UNITY_IPHONE || UNITY_ANDROID
            HandleTouchEvents();
#elif !UNITY_FLASH
              HandleMouseEvents();
#endif
        }

        private void HandleMouseEvents()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //    Debug.Log("Mouse Down - HandleMouseEvents");
            //    isDown = true;
            //}
            //if (Input.GetMouseButtonUp(0))
            //{
            //    isDown = false;
            //    Debug.Log("Mouse Up - HandleMouseEvents");
            //}

            //Debug.Log(isDown);
            //if (isDown)
            //{
            //    DragCheck();
            //}
        }

        private void HandleTouchEvents()
        {
            if (Input.touchCount == 1)
            {
                switch (Input.touches[0].phase)
                {
                    case TouchPhase.Began:
                        break;
                    case TouchPhase.Moved:
                        if (isDown)
                        {
                            DragCheck();
                        }
                        break;
                    case TouchPhase.Stationary:
                        break;
                    case TouchPhase.Canceled:
                        isDown = false;
                        break;
                    case TouchPhase.Ended:
                        isDown = false;
                        break;
                }
            }
            else if (Input.touchCount == 0)
            {
                isDown = false;
            }
        }

        private void DragCheck()
        {
            Debug.Log("Dragging");
            Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, 100);
            if (hitInfo.collider)
            {
                var gCell = hitInfo.collider.GetComponent<GridCellObject>();
                if (gCell)
                {
                    gCell.gameObject.renderer.material.color = Color.green;
                }
            }
        }
    }
}