﻿using System;
using System.Collections.Generic;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class AdvanceBrain : MonoBehaviour
    {
        #region Player

        public GameObject PlayerPrefab;
        private GameObject _currentPlayer;

        private GameObject _pDest;
        private GameObject _pStart;

        #endregion

        #region BrainOrgans

        private const string MapName = "SampleMap";
        public GUISkin DefaultGUISkin;
        public List<Deployable> DeployableList;
        public AdvanceGrid GameGrid;
        private bool _allowMove;
        private BrainStates _brainState;
        private IntVector2 _deltaIndexOffset;
        private Dictionary<string, Deployable> _deployableDictionary;
        private bool _gridLinesVisibilityStatus = true;
        private bool _isDown;
        private Camera _levelEditorCamera;
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
            set
            {
                if (value != _gridLinesVisibilityStatus)
                {
                    GameGrid.GridLinesTransform.GetComponent<Renderer>().enabled = value;
                    _gridLinesVisibilityStatus = value;
                }
            }
        }


        public BrainStates BrainState
        {
            get { return _brainState; }
            set
            {
                _brainState = value;
                AllowOthersToDrawOnGUI = value == BrainStates.EditMode;
            }
        }

        #endregion

        #region GUIHelpers

        public static bool AllowOthersToDrawOnGUI = false;
        public static Matrix4x4 GUIMatrix;
        private readonly GUILocationHelper _location = new GUILocationHelper();
        private readonly string[] _menuStrings = {"Create", "Edit", "Erase", "Move"};
        private readonly string[] _otherMenuStrings = {"Save", "Load", "Clear"};
        private bool _guiMenuToggle;
        private bool _inPlayMode;

        #endregion

        #region Helpers

        public static AdvanceBrain Instance;

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

            if (DefaultGUISkin == null)
            {
                DefaultGUISkin = Resources.Load("GUISkin/defaultGUIStyle", typeof (GUISkin)) as GUISkin;
            }


            Instance = this;
            _levelEditorCamera = Camera.main;
        }

        public void OnGUI()
        {
            GUI.matrix = GUIMatrix;

            GUI.Label(new Rect(190, 10, 180, 50), BrainState.ToString(), DefaultGUISkin.label);
            
            _guiMenuToggle = GUI.Toggle(new Rect(0, 0, 80, 80), _guiMenuToggle, "+", "Button");
            ShowGridLines = GUI.Toggle(new Rect(_location.Offset.x - 100, 0, 100, 60), ShowGridLines, "#", "Button");

            if (_guiMenuToggle)
            {
                DrawMenu();
                DrawSaveLoadButtons();
            }
            DrawPlayControl();

            switch (BrainState)
            {
                case BrainStates.EditMode:
                    if (_selectedDeployable)
                    {
                        GUI.Label(new Rect(350, 10, 400, 50), String.Format("Selected Object: {0}", _selectedDeployable.name), DefaultGUISkin.label);
                    }
                    break;
                case BrainStates.CreationMode:
                    if (_guiMenuToggle)
                        DrawDeployables();
                    break;
            }

            GUI.matrix = Matrix4x4.identity;
        }


        private void DrawMenu()
        {
            Event e = Event.current;
            for (int i = 0, n = _menuStrings.Length; i < n; i++)
            {
                var buttonRect = new Rect(10, 100 + i*80, 150, 75);

                if (e.isMouse && buttonRect.Contains(e.mousePosition))
                {
                    if (e.type == EventType.MouseDown)
                    {
                        _onGui = true;
                        BrainState = (BrainStates) i;
                    }
                    else
                    {
                        _onGui = false;
                    }
                }

                GUI.Button(buttonRect, _menuStrings[i]);
            }
        }

        private void DrawDeployables()
        {
            Event e = Event.current;
            for (int i = 0, n = DeployableList.Count; i < n; i++)
            {
                var buttonRect = new Rect(180, 110 + i*60, 150, 50);

                if (e.isMouse && buttonRect.Contains(e.mousePosition))
                {
                    if (e.type == EventType.MouseDown)
                    {
                        _onGui = true;
                        _objectToDeploy = DeployableList[i];

                        if (_objectToDeploy.DeploymentMethod == DeploymentMethod.Drag)
                        {
                            GameGrid.IsDrawGhostTilesEnable = true;
                            _isDown = true;
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

        private void DrawPlayControl()
        {
            Event e = Event.current;
            var buttonRect = new Rect(81, 0, 80, 80);
            if (e.isMouse && buttonRect.Contains(e.mousePosition))
            {
                if (e.type == EventType.MouseDown)
                {
                    _inPlayMode = !_inPlayMode;
                    _onGui = true;

                    if (_inPlayMode)
                    {
                        PreparePlayer();
                    }
                    else
                    {
                        QuitPlayMode();
                        BrainState = BrainStates.NavigateMode;
                    }
                }
            }

            GUI.Button(buttonRect, _inPlayMode ? "STOP" : "PLAY");
        }

        private void DrawSaveLoadButtons()
        {
            Event e = Event.current;
            for (int i = 0, n = _otherMenuStrings.Length; i < n; i++)
            {
                var buttonRect = new Rect(_location.Offset.x - 180, 70 + i*90, 150, 75);

                if (e.isMouse && buttonRect.Contains(e.mousePosition))
                {
                    if (e.type == EventType.MouseDown)
                    {
                        _onGui = true;

                        switch (i)
                        {
                            case 0:
                                GameGrid.SaveDataToXML(MapName);
                                break;
                            case 1:
                                GameGrid.LoadDataFromXML(MapName, _deployableDictionary);
                                break;
                            case 2:
                                GameGrid.ClearEntireGrid();
                                break;
                        }
                    }
                    else
                    {
                        _onGui = false;
                    }
                }

                GUI.Button(buttonRect, _otherMenuStrings[i]);
            }
        }

        public void Update()
        {
            if (!_onGui)
            {
                switch (BrainState)
                {
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

        #region PlayMode

        private void QuitPlayMode()
        {
            if (_currentPlayer)
                Destroy(_currentPlayer);

            _levelEditorCamera.gameObject.SetActive(true);
            ShowGridLines = true;

            GameGrid.ClearEntireGrid();
            GameGrid.LoadDataFromXML(MapName, _deployableDictionary);
        }

        private void PreparePlayer()
        {
            _pStart = GameObject.FindWithTag("PlayerStart");
            _pDest = GameObject.FindWithTag("PlayerDestination");

            if (!_pStart || !_pDest)
            {
                Debug.LogWarning("Start and/or Destination not found");
            }
            else
            {
                GameGrid.SaveDataToXML(MapName);

                _pStart.GetComponent<Renderer>().enabled = false;
                //_pDest.renderer.enabled = false;
                _currentPlayer = (GameObject) Instantiate(PlayerPrefab, _pStart.transform.position, Quaternion.identity);

                GameObject[] allMainCams = GameObject.FindGameObjectsWithTag("MainCamera");
                if (allMainCams.Length > 1)
                {
                    // Player object has built in MainCamera component
                    _levelEditorCamera.gameObject.SetActive(false);
                }

                _guiMenuToggle = false;
                ShowGridLines = false;
            }
        }

        public void PlayerReachedDestination()
        {
            QuitPlayMode();
        }

        #endregion
    }
}