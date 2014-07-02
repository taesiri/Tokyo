using System;
using System.Collections.Generic;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class Brain : MonoBehaviour
    {
        private readonly string[] _menuStrings = {"Creation", "Edit", "Erase", "PlayMode"};
        public BrainStates BrainState;
        public List<Deployable> DeployableList;
        public Grid GameGrid;
        public Transform GridTransform;
        public GUILocationHelper Location = new GUILocationHelper();
        private bool _allowToMove;
        private Deployable _currentObject;
        private Matrix4x4 _guiMatrix;
        private bool _isDown;
        private GridCell _lastVisitedTile;
        private int _menuSelectedIndex;
        private GridCell _originCell;
        private Deployable _selectedObject;

        public void Start()
        {
            Location.PointLocation = GUILocationHelper.Point.BottomLeft;
            Location.UpdateLocation();


            Vector2 ratio = Location.GuiOffset;
            _guiMatrix = Matrix4x4.identity;
            _guiMatrix.SetTRS(new Vector3(1, 1, 1), Quaternion.identity, new Vector3(ratio.x, ratio.y, 1));

            BrainState = BrainStates.EditMode;

            if (!GridTransform)
            {
                Debug.LogWarning("Grid Transform is Missing!");
                GridTransform = GameObject.FindWithTag("Grid").transform;
            }
            if (!GameGrid)
            {
                Debug.LogWarning("Game Grid is Missing!");
                GameGrid = GameObject.FindWithTag("Grid").GetComponent<Grid>();
            }
        }

        public void OnGUI()
        {
            GUI.Label(new Rect(0, 0, 100, 50), BrainState.ToString());
            _menuSelectedIndex = GUI.Toolbar(new Rect(0, Location.Offset.y - 100, 300, 75), _menuSelectedIndex,
                _menuStrings);

            switch (BrainState)
            {
                case BrainStates.PlayMode:
                    break;
                case BrainStates.EraserMode:
                    break;
                case BrainStates.EditMode:
                    if (_selectedObject)
                    {
                        GUI.Label(new Rect(0, 50, 400, 50),
                            String.Format("Selected Object: {0}", _selectedObject.name));
                    }
                    break;
                case BrainStates.CreationMode:
                    GUI.matrix = _guiMatrix;


                    for (int i = 0; i < DeployableList.Count; i++)
                    {
                        if (GUI.RepeatButton(new Rect(i*150, 100, 145, 100), DeployableList[i].GetDisplayName()))
                        {
                            if (DeployableList[i].DeploymentMethod == DeploymentMethod.Drag)
                            {
                                _isDown = true;
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

            UpdateBrainState();
        }

        private void UpdateBrainState()
        {
            switch (_menuSelectedIndex)
            {
                case 0:
                    BrainState = BrainStates.CreationMode;
                    break;
                case 1:
                    BrainState = BrainStates.EditMode;
                    break;
                case 2:
                    BrainState = BrainStates.EraserMode;
                    break;
                case 3:
                    BrainState = BrainStates.PlayMode;
                    break;
            }
        }

        public void Update()
        {
            switch (BrainState)
            {
                case BrainStates.PlayMode:
                    break;
                case BrainStates.EraserMode:
                    EraserUpdate();
                    break;
                case BrainStates.EditMode:
                    EditUpdate();
                    break;
                case BrainStates.CreationMode:
                    CreationUpdate();
                    //#if UNITY_IPHONE || UNITY_ANDROID
                    //            HandleTouchEvents();
                    //#elif !UNITY_FLASH
                    //              CreationUpdate();
                    //#endif
                    break;
            }
        }


        private void EraserUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isDown = true;
            }
            if (_isDown)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                Physics.Raycast(ray, out hitInfo, 100, 1 << 8);
                if (hitInfo.collider)
                {
                    var gCell = hitInfo.collider.GetComponent<GridCell>();
                    if (gCell)
                    {
                        if (!gCell.IsEmpty)
                        {
                            Destroy(gCell.InCellObject.gameObject);
                            gCell.IsEmpty = true;
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isDown = false;
            }
        }

        private void CreationUpdate()
        {
            if (_currentObject)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (_currentObject.DeploymentMethod == DeploymentMethod.Brush)
                    {
                        _isDown = true;
                    }
                    Debug.Log("Mouse Down - CreationUpdate");
                }
                if (Input.GetMouseButtonUp(0))
                {
                    _isDown = false;
                    Debug.Log("Mouse Up - CreationUpdate");

                    // Not the Base Way for handling new Cell Instatiation!
                    if (_currentObject && _lastVisitedTile)
                    {
                        if (_currentObject.DeploymentMethod == DeploymentMethod.Drag)
                        {
                            var newCell =
                                (Deployable) Instantiate(_currentObject, _lastVisitedTile.gameObject.transform.position,
                                    Quaternion.identity);
                            newCell.transform.parent = GridTransform;
                            newCell.gameObject.layer = 9;
                            newCell.ParentGridCell = _lastVisitedTile;

                            _lastVisitedTile.InCellObject = newCell.transform;
                            _lastVisitedTile.IsEmpty = false;
                        }
                    }
                    _lastVisitedTile = null;
                }

                if (_isDown)
                {
                    DragCheck();
                }
            }
        }


        private void EditUpdate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isDown = true;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                Physics.Raycast(ray, out hitInfo, 100, 1 << 9);
                if (hitInfo.collider)
                {
                    _selectedObject = hitInfo.collider.gameObject.GetComponent<Deployable>();
                    _originCell = _selectedObject.ParentGridCell;
                    _allowToMove = true;
                }
                else
                {
                    _allowToMove = false;
                }
            }

            if (_allowToMove)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = _selectedObject.transform.position.z;
                _selectedObject.transform.position = pos;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_allowToMove)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitInfo;
                    Physics.Raycast(ray, out hitInfo, 100, 1 << 8);
                    if (hitInfo.collider)
                    {
                        var gCell = hitInfo.collider.GetComponent<GridCell>();
                        if (gCell)
                        {
                            if (gCell.IsEmpty)
                            {
                                GameGrid.UpdateTilesState(_selectedObject.TileMap, gCell, CellState.Full);
                                gCell.InCellObject = _selectedObject.transform;
                                // Caution : Cell in the vicinity of gCell dosent store reference to TILE ELEMENT[_selectedObject]
                                Vector3 pos = gCell.gameObject.transform.position;

                                pos.x += _currentObject.TileMap.TileSize.X/2f*GameGrid.CellWidth -
                                         GameGrid.CellWidth/2f;
                                pos.y -= _currentObject.TileMap.TileSize.Y/2f*GameGrid.CellWidth -
                                         GameGrid.CellWidth/2f;
                                _selectedObject.transform.position = pos;
                                _selectedObject.ParentGridCell = gCell;


                                GameGrid.UpdateTilesState(_selectedObject.TileMap, _originCell, CellState.Empty);
                                _originCell = null;
                            }
                            else
                            {
                                ResetSelectedObjectPosition();
                            }
                        }
                        else
                        {
                            ResetSelectedObjectPosition();
                        }
                    }
                    else
                    {
                        ResetSelectedObjectPosition();
                    }
                }

                _isDown = false;
                _allowToMove = false;
            }
        }

        private void ResetSelectedObjectPosition()
        {
            Vector3 pos = _originCell.transform.position;

            pos.x += _currentObject.TileMap.TileSize.X / 2f * GameGrid.CellWidth -
                     GameGrid.CellWidth / 2f;
            pos.y -= _currentObject.TileMap.TileSize.Y / 2f * GameGrid.CellWidth -
                     GameGrid.CellWidth / 2f;

            _selectedObject.transform.position = pos;
        }

        //private void HandleTouchEvents()
        //{
        //    if (Input.touchCount == 1)
        //    {
        //        switch (Input.touches[0].phase)
        //        {
        //            case TouchPhase.Began:
        //                break;
        //            case TouchPhase.Moved:
        //                if (_isDown)
        //                {
        //                    DragCheck();
        //                }
        //                break;
        //            case TouchPhase.Stationary:
        //                break;
        //            case TouchPhase.Canceled:
        //                _isDown = false;
        //                break;
        //            case TouchPhase.Ended:
        //                _isDown = false;
        //                break;
        //        }
        //    }
        //    else if (Input.touchCount == 0)
        //    {
        //        _isDown = false;
        //    }
        //}

        private void DragCheck()
        {
            Debug.Log("Dragging");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, 100);
            if (hitInfo.collider)
            {
                var gCell = hitInfo.collider.GetComponent<GridCell>();
                if (gCell)
                {
                    if (gCell.IsEmpty)
                    {
                        if (GameGrid.IsPlaceable(_currentObject.TileMap, gCell))
                        {
                            if (_currentObject)
                            {
                                switch (_currentObject.DeploymentMethod)
                                {
                                    case DeploymentMethod.Brush:

                                        Vector3 pos = gCell.gameObject.transform.position;

                                        pos.x += _currentObject.TileMap.TileSize.X/2f*GameGrid.CellWidth -
                                                 GameGrid.CellWidth/2f;
                                        pos.y -= _currentObject.TileMap.TileSize.Y/2f*GameGrid.CellWidth -
                                                 GameGrid.CellWidth/2f;

                                        var newCell =
                                            (Deployable)
                                                Instantiate(_currentObject, pos, Quaternion.identity);

                                        newCell.transform.parent = GridTransform;
                                        newCell.gameObject.layer = 9;
                                        newCell.ParentGridCell = gCell;
                                        GameGrid.UpdateTilesState(_currentObject.TileMap, gCell, CellState.Full);
                                        gCell.InCellObject = newCell.transform;

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
}