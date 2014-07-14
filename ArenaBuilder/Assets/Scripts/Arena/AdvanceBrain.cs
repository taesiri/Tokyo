using System;
using System.Collections.Generic;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class AdvanceBrain : MonoBehaviour
    {
        #region BrainOrgans

        private const string MapName = "SampleMap";
        [HideInInspector] public BrainStates BrainState;
        public List<Deployable> DeployableList;
        public AdvanceGrid GameGrid;
        private bool _allowMove;
        private IntVector2 _deltaIndexOffset;
        private Dictionary<string, Deployable> _deployableDictionary;
        private bool _gridLinesVisibilityStatus = true;
        private bool _isDown;
        private Deployable _objectToDeploy;
        private bool _onGui;
        private Deployable _selectedDeployable;
        private Vector3 _selectedObjectDeltaPosition;

        public bool AllowToMove
        {
            get { return _allowMove; }
            set
            {
                _allowMove = value;
                GameGrid.IsDrawGhostTilesEnable = value;
            }
        }

        public bool ShowGridLines
        {
            get { return _gridLinesVisibilityStatus; }
            set { _gridLinesVisibilityStatus = value; }
        }

        #endregion

        #region GUIHelpers

        public static bool AllowOthersToDrawOnGUI = false;
        public static Matrix4x4 GUIMatrix;
        private readonly GUILocationHelper _location = new GUILocationHelper();
        private readonly string[] _menuStrings = {"Create", "Edit", "Erase", "Move", "Play!"};
        private int _menuSelectedIndex;

        #endregion

        public void Start()
        {
            _location.PointLocation = GUILocationHelper.Point.TopRight;
            _location.UpdateLocation();

            Vector2 ratio = _location.GuiOffset;
            GUIMatrix = Matrix4x4.identity;
            GUIMatrix.SetTRS(new Vector3(1, 1, 1), Quaternion.identity, new Vector3(ratio.x, ratio.y, 1));

            _deployableDictionary = new Dictionary<string, Deployable>(DeployableList.Count);
            for (int i = 0, n = DeployableList.Count; i < n; i++)
            {
                _deployableDictionary.Add(DeployableList[i].GetDisplayName(), DeployableList[i]);
            }

            BrainState = BrainStates.CreationMode;
        }

        public void OnGUI()
        {
            GUI.matrix = GUIMatrix;

            GUI.Label(new Rect(10, 10, 100, 50), BrainState.ToString());
            _menuSelectedIndex = GUI.Toolbar(new Rect(10, 50, 500, 75), _menuSelectedIndex, _menuStrings);
            switch (BrainState)
            {
                case BrainStates.PlayMode:
                    break;
                case BrainStates.EraserMode:
                    break;
                case BrainStates.EditMode:
                    if (_selectedDeployable)
                    {
                        GUI.Label(new Rect(120, 10, 400, 50),
                            String.Format("Selected Object: {0}", _selectedDeployable.name));
                    }
                    break;
                case BrainStates.CreationMode:
                    DrawDeployables();
                    break;
            }

            if (GUI.Button(new Rect(_location.Offset.x - 180, 10, 150, 90), "SAVE"))
            {
                GameGrid.SaveDataToXML(MapName);
            }
            if (GUI.Button(new Rect(_location.Offset.x - 180, 110, 150, 90), "LOAD"))
            {
                GameGrid.LoadDataFromXML(MapName, _deployableDictionary);
            }
            if (GUI.Button(new Rect(_location.Offset.x - 180, 210, 150, 90), "Clear"))
            {
                GameGrid.ClearEntireGrid();
            }

            GUI.matrix = Matrix4x4.identity;
            UpdateBrainState();
        }


        private void DrawDeployables()
        {
            Event e = Event.current;
            for (int i = 0, n = DeployableList.Count; i < n; i++)
            {
                var buttonRect = new Rect(i*160, 150, 150, 50);

                if (e.isMouse && buttonRect.Contains(e.mousePosition))
                {
                    if (e.type == EventType.mouseDown)
                    {
                        _onGui = true;
                        _isDown = true;
                        _objectToDeploy = DeployableList[i];

                        if (_objectToDeploy.DeploymentMethod == DeploymentMethod.Drag)
                        {
                            GameGrid.IsDrawGhostTilesEnable = true;
                        }
                    }
                    else
                    {
                        _onGui = false;
                    }
                }

                GUI.Button(buttonRect, DeployableList[i].GetDisplayName());
            }
        }

        private void UpdateBrainState()
        {
            switch (_menuSelectedIndex)
            {
                case 0:
                    BrainState = BrainStates.CreationMode;
                    AllowOthersToDrawOnGUI = false;
                    break;
                case 1:
                    BrainState = BrainStates.EditMode;
                    AllowOthersToDrawOnGUI = true;
                    break;
                case 2:
                    BrainState = BrainStates.EraserMode;
                    AllowOthersToDrawOnGUI = false;
                    break;
                case 3:
                    BrainState = BrainStates.NavigateMode;
                    AllowOthersToDrawOnGUI = false;
                    break;
                case 4:
                    BrainState = BrainStates.PlayMode;
                    AllowOthersToDrawOnGUI = false;
                    break;
            }
        }


        public void Update()
        {
            if (!_onGui)
            {
                Debug.Log("Updating!");
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
                        break;
                    case BrainStates.NavigateMode:
                        CameraMovementHandler.Handler();
                        break;
                }
            }
        }
        
        private void CreationUpdate()
        {
            if (_objectToDeploy)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (_objectToDeploy.DeploymentMethod == DeploymentMethod.Brush)
                    {
                        _isDown = true;
                    }
                }

                if (_isDown)
                {
                    DragCheck();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (_isDown)
                    {
                        if (_objectToDeploy.DeploymentMethod == DeploymentMethod.Drag)
                        {
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            GameGrid.DeployIfPossible(ray, _objectToDeploy);

                            GameGrid.IsDrawGhostTilesEnable = false;
                        }

                        _isDown = false;
                    }
                }
            }
        }

        private void DragCheck()
        {
            Ray ray;
            switch (_objectToDeploy.DeploymentMethod)
            {
                case DeploymentMethod.Brush:
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    GameGrid.DeployIfPossible(ray, _objectToDeploy);
                    break;
                case DeploymentMethod.Drag:
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    GameGrid.DrawGhostTiles(ray, _objectToDeploy, IntVector2.Zero());
                    break;
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
                    if (_selectedDeployable)
                        _selectedDeployable.AllowToDrawGUI = false;

                    _selectedDeployable = hitInfo.collider.gameObject.GetComponent<Deployable>();

                    AllowToMove = true;

                    _deltaIndexOffset = GameGrid.PointOnPlaneToIndex(hitInfo.point) - _selectedDeployable.GridIndex;

                    GameGrid.UpdateTilesStateWithOffset(_selectedDeployable, _selectedDeployable.GridIndex, CellState.Empty);
                    _selectedObjectDeltaPosition = _selectedDeployable.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    _selectedObjectDeltaPosition.z = 0;

                    _selectedDeployable.AllowToDrawGUI = true;
                }
                else
                {
                    AllowToMove = false;
                }
            }

            if (AllowToMove)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = _selectedDeployable.transform.position.z;
                _selectedDeployable.transform.position = pos + _selectedObjectDeltaPosition;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                GameGrid.DrawGhostTiles(ray, _selectedDeployable, _deltaIndexOffset);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (AllowToMove)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (!GameGrid.DropDeployableIfPossible(ray, _selectedDeployable, _deltaIndexOffset))
                    {
                        Debug.Log("Do Reset!");
                        ResetSelectedObjectPosition();
                    }
                }

                _isDown = false;
                AllowToMove = false;
                _deltaIndexOffset = null;
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
                GameGrid.EraseTiles(ray);
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isDown = false;
            }
        }


        private void ResetSelectedObjectPosition()
        {
            Vector3 pos = GameGrid.IndexToWorldPosition(_selectedDeployable.GridIndex);
            Vector2 wOffset = _selectedDeployable.TileMap.GetWorldTransformOffset(GameGrid.GlobalCellWidth);

            pos.x += wOffset.x;
            pos.y += wOffset.y;

            _selectedDeployable.transform.position = pos;
            GameGrid.UpdateTilesStateWithOffset(_selectedDeployable, _selectedDeployable.GridIndex, CellState.Full);
        }
    }
}