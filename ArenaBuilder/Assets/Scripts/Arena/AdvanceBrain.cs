using System;
using System.Collections.Generic;
using System.Xml;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class AdvanceBrain : MonoBehaviour
    {
        #region FieldsForSceneSetup

        private readonly GUILocationHelper _location = new GUILocationHelper();
        private Matrix4x4 _guiMatrix;

        #endregion

        #region BrainOrgans

        [HideInInspector] public BrainStates BrainState;
        public List<Deployable> DeployableList;
        public AdvanceGrid GameGrid;
        private bool _allowToMove;
        private bool _isDown;
        private Deployable _objectToDeploy;
        private Deployable _selectedDeployable;
        private Vector3 _selectedObjectDeltaPosition;

        #endregion

        #region GUIHelpers

        private readonly string[] _menuStrings = {"Create", "Edit", "Erase", "Play!"};
        private int _menuSelectedIndex;

        #endregion

        public void Start()
        {
            _location.PointLocation = GUILocationHelper.Point.TopLeft;
            _location.UpdateLocation();


            Vector2 ratio = _location.GuiOffset;
            _guiMatrix = Matrix4x4.identity;
            _guiMatrix.SetTRS(new Vector3(1, 1, 1), Quaternion.identity, new Vector3(ratio.x, ratio.y, 1));

            BrainState = BrainStates.CreationMode;
        }

        public void OnGUI()
        {
            GUI.Label(new Rect(10, 10, 100, 50), BrainState.ToString());
            _menuSelectedIndex = GUI.Toolbar(new Rect(10, 50, 300, 75), _menuSelectedIndex, _menuStrings);
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
                    GUI.matrix = _guiMatrix;


                    for (int i = 0; i < DeployableList.Count; i++)
                    {
                        if (GUI.RepeatButton(new Rect(i*150, 150, 150, 50), DeployableList[i].GetDisplayName()))
                        {
                            if (DeployableList[i].DeploymentMethod == DeploymentMethod.Drag)
                            {
                                _isDown = true;
                            }
                            else if (DeployableList[i].DeploymentMethod == DeploymentMethod.Brush)
                            {
                            }
                            _objectToDeploy = DeployableList[i];
                        }
                    }

                    GUI.matrix = Matrix4x4.identity;
                    break;
            }


            if (GUI.Button(new Rect(400, 10, 100, 50), "SAVE"))
            {
                SaveDataToXML();
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
                // this code doesn't check tile status (empty or full)ness
                // WTF
                if (Input.GetMouseButtonUp(0))
                {
                    _isDown = false;
                    //if (_objectToDeploy && _lastVisitedTile)
                    //{
                    //    if (_objectToDeploy.DeploymentMethod == DeploymentMethod.Drag)
                    //    {
                    //        var newCell =
                    //            (Deployable) Instantiate(_objectToDeploy, _lastVisitedTile.gameObject.transform.position,
                    //                Quaternion.identity);
                    //        newCell.transform.parent = GameGrid.transform;
                    //        newCell.gameObject.layer = 9;
                    //        newCell.ParentGridCell = _lastVisitedTile;

                    //        _lastVisitedTile.InCellObject = newCell;
                    //        _lastVisitedTile.IsEmpty = false;
                    //    }
                    //}
                    //_lastVisitedTile = null;
                }

                if (_isDown)
                {
                    DragCheck();
                }
            }
        }

        private void DragCheck()
        {
            switch (_objectToDeploy.DeploymentMethod)
            {
                case DeploymentMethod.Brush:
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    GameGrid.DeployIfPossible(ray, _objectToDeploy);
                    break;
                case DeploymentMethod.Drag:
                    // Wait for End of Drag!
                    //_lastVisitedTile = gCell;
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
                    _selectedDeployable = hitInfo.collider.gameObject.GetComponent<Deployable>();

                    _allowToMove = true;
                    GameGrid.UpdateTilesStateWithOffset(_selectedDeployable, _selectedDeployable.GridIndex, CellState.Empty);
                    _selectedObjectDeltaPosition = _selectedDeployable.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    _selectedObjectDeltaPosition.z = 0;
                }
                else
                {
                    _allowToMove = false;
                }
            }

            if (_allowToMove)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = _selectedDeployable.transform.position.z;
                _selectedDeployable.transform.position = pos + _selectedObjectDeltaPosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_allowToMove)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (!GameGrid.DropAtDeployableIfPossible(ray, _selectedDeployable))
                    {
                        ResetSelectedObjectPosition();
                    }
                }

                _isDown = false;
                _allowToMove = false;
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

        public void SaveDataToXML()
        {
            Deployable[] childs = GameGrid.GetAllChildren();

            using (XmlWriter writer = XmlWriter.Create("D:\\data.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Tiles");

                foreach (Deployable child in childs)
                {
                    writer.WriteStartElement("Tile");

                    writer.WriteElementString("Name", child.GetDisplayName());
                    writer.WriteElementString("GridIndex", child.GridIndex.ToString());
                   
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }
}