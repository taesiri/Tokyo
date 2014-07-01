using System.Collections.Generic;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class Brain : MonoBehaviour
    {
        private readonly string[] _menuStrings = {"Creation", "PlayMode"};
        public BrainStates BrainState;
        public List<Deployable> DeployableList;
        public GUILocationHelper Location = new GUILocationHelper();
        private Deployable _currentObject;
        private Matrix4x4 _guiMatrix;
        private GridCellObject _lastVisitedTile;
        private int _menuSelectedIndex;
        private bool isDown;

        public void Start()
        {
            Location.PointLocation = GUILocationHelper.Point.Center;
            Location.UpdateLocation();

            Vector2 ratio = Location.GuiOffset;
            _guiMatrix = Matrix4x4.identity;
            _guiMatrix.SetTRS(new Vector3(1, 1, 1), Quaternion.identity, new Vector3(ratio.x, ratio.y, 1));

            BrainState = BrainStates.EditMode;
        }

        public void OnGUI()
        {
            switch (BrainState)
            {
                case BrainStates.PlayMode:
                    break;
                case BrainStates.EditMode:
                case BrainStates.CreationMode:
                    GUI.matrix = _guiMatrix;


                    GUI.Label(new Rect(0, 0, 100, 50), BrainState.ToString());
                    _menuSelectedIndex = GUI.Toolbar(new Rect(0, 210, 300, 75), _menuSelectedIndex, _menuStrings);


                    for (int i = 0; i < DeployableList.Count; i++)
                    {
                        if (GUI.RepeatButton(new Rect(i*150, 100, 145, 100), DeployableList[i].GetDisplayName()))
                        {
                            if (DeployableList[i].DeploymentMethod == DeploymentMethod.Drag)
                            {
                                isDown = true;
                            }
                            else if (DeployableList[i].DeploymentMethod == DeploymentMethod.Brush)
                            {
                            }

                            Debug.Log(string.Format("Clicked On {0}", DeployableList[i].GetDisplayName()));
                            _currentObject = DeployableList[i];
                        }
                    }


                    GUI.matrix = Matrix4x4.identity;
                    break;
            }
        }

        public void Update()
        {
            switch (BrainState)
            {
                case BrainStates.PlayMode:
                    break;
                case BrainStates.EditMode:
                case BrainStates.CreationMode:
                    HandleMouseEvents();
                    //#if UNITY_IPHONE || UNITY_ANDROID
                    //            HandleTouchEvents();
                    //#elif !UNITY_FLASH
                    //              HandleMouseEvents();
                    //#endif
                    break;
            }
        }


        private void HandleMouseEvents()
        {
            if (_currentObject)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isDown = true;
                    Debug.Log("Mouse Down - HandleMouseEvents");
                }
                if (Input.GetMouseButtonUp(0))
                {
                    isDown = false;
                    Debug.Log("Mouse Up - HandleMouseEvents");


                    // 

                    if (_currentObject && _lastVisitedTile)
                    {
                        if (_currentObject.DeploymentMethod == DeploymentMethod.Drag)
                        {
                            Instantiate(_currentObject, _lastVisitedTile.gameObject.transform.position,
                                Quaternion.identity);
                            _lastVisitedTile.IsEmpty = false;
                        }
                    }
                    _lastVisitedTile = null;
                }

                if (isDown)
                {
                    DragCheck();
                }
            }
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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, 100);
            if (hitInfo.collider)
            {
                var gCell = hitInfo.collider.GetComponent<GridCellObject>();
                if (gCell)
                {
                    if (gCell.IsEmpty)
                    {
                        if (_currentObject)
                        {
                            switch (_currentObject.DeploymentMethod)
                            {
                                case DeploymentMethod.Brush:
                                    Instantiate(_currentObject, gCell.gameObject.transform.position, Quaternion.identity);
                                    gCell.IsEmpty = false;
                                    break;
                                case DeploymentMethod.Drag:
                                    // Wait for End of Drag!
                                    _lastVisitedTile = gCell;
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }
}