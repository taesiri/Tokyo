using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
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
        private Deployable _selectedDeployable;
        private Vector3 _selectedObjectDeltaPosition;

        public bool AllowToMove
        {
            get { return _allowMove; }
            set
            {
                _allowMove = value;
                GameGrid.GridLinesMaterial.SetFloat("_IsEnable", value ? 1.0f : 0.0f);
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
        private readonly GUILocationHelper _location = new GUILocationHelper();
        private readonly string[] _menuStrings = {"Create", "Edit", "Erase", "Move", "Play!"};
        private Matrix4x4 _guiMatrix;
        private int _menuSelectedIndex;

        #endregion

        #region CameraMovementZoom

        public const float MaximumOrthographicSize = 40.0f;
        public float MinPinchSpeed = 1.0F;
        public float MovementSpeed = .01f;
        public float VarianceInDistances = 5.0F;
        public float ZoomSpeedMouse = 5.0f;
        public float ZoomSpeedTocuh = 1.0f;

        #endregion

        public void Start()
        {
            _location.PointLocation = GUILocationHelper.Point.TopLeft;
            _location.UpdateLocation();

            Vector2 ratio = _location.GuiOffset;
            _guiMatrix = Matrix4x4.identity;
            _guiMatrix.SetTRS(new Vector3(1, 1, 1), Quaternion.identity, new Vector3(ratio.x, ratio.y, 1));

            _deployableDictionary = new Dictionary<string, Deployable>(DeployableList.Count);
            for (int i = 0, n = DeployableList.Count; i < n; i++)
            {
                _deployableDictionary.Add(DeployableList[i].GetDisplayName(), DeployableList[i]);
            }

            BrainState = BrainStates.CreationMode;
        }

        public void OnGUI()
        {
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
                    GUI.matrix = _guiMatrix;


                    for (int i = 0, n = DeployableList.Count; i < n; i++)
                    {
                        if (GUI.RepeatButton(new Rect(i*160, 150, 150, 50), DeployableList[i].GetDisplayName()))
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


            if (GUI.Button(new Rect(10, 250, 120, 90), "SAVE"))
            {
                SaveDataToXML();
            }
            if (GUI.Button(new Rect(10, 350, 120, 90), "LOAD"))
            {
                LoadDataFromXML();
            }
            if (GUI.Button(new Rect(10, 450, 120, 90), "Clear"))
            {
                ClearGrid();
            }
            UpdateBrainState();
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
            if (GUIUtility.hotControl == 0)
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
                        break;
                    case BrainStates.NavigateMode:
                        NavigateUpdate();
                        break;
                }
            }
        }

        private void NavigateUpdate()
        {
            if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved &&
                Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                Vector2 curDist = Input.GetTouch(0).position - Input.GetTouch(1).position;
                //current distance between finger touches
                Vector2 prevDist = ((Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition) -
                                    (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition));

                float touchDelta = curDist.magnitude - prevDist.magnitude;

                float speedTouch0 = Input.GetTouch(0).deltaPosition.magnitude/Input.GetTouch(0).deltaTime;
                float speedTouch1 = Input.GetTouch(1).deltaPosition.magnitude/Input.GetTouch(1).deltaTime;


                if ((speedTouch0 > MinPinchSpeed) && (speedTouch1 > MinPinchSpeed))
                {
                    if (touchDelta + VarianceInDistances <= 1)
                    {
                        Camera.main.orthographicSize =
                            Mathf.Clamp(Camera.main.orthographicSize + (1*ZoomSpeedTocuh),
                                MaximumOrthographicSize/10,
                                MaximumOrthographicSize);
                    }
                    else if (touchDelta + VarianceInDistances > 1)
                    {
                        Camera.main.orthographicSize =
                            Mathf.Clamp(Camera.main.orthographicSize - (1*ZoomSpeedTocuh),
                                MaximumOrthographicSize/10, MaximumOrthographicSize);
                    }
                    MoveCamera(Vector3.zero);
                }
            }
            else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                MoveCamera(Input.GetTouch(0).deltaPosition);
            }
        }

        public void MoveCamera(Vector3 movement)
        {
            //float boundaryX = (GameGrid.Columns * 6) - (Camera.main.aspect * Camera.main.orthographicSize);
            //float boundaryY = (GameGrid.Rows * 6) - Camera.main.orthographicSize;

            movement.z = 0;
            movement *= -MovementSpeed;
            movement *= Camera.main.orthographicSize/10;

            Vector3 pos = Camera.main.transform.position;
            pos += movement;

            //pos.x = Mathf.Clamp(pos.x, -boundaryX, boundaryX);
            //pos.y = Mathf.Clamp(pos.y, -boundaryY, boundaryY);

            Camera.main.transform.position = pos;
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

        public void ClearGrid()
        {
            GameGrid.ClearEntireGrid();
        }

        public void SaveDataToXML()
        {
            Deployable[] childs = GameGrid.GetAllChildren();


            using (XmlWriter writer = XmlWriter.Create(Application.persistentDataPath + "/" + MapName + ".dm"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Tiles");
                writer.WriteAttributeString("Row", GameGrid.Rows.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Column", GameGrid.Columns.ToString(CultureInfo.InvariantCulture));

                foreach (Deployable child in childs)
                {
                    writer.WriteStartElement("Tile");

                    writer.WriteAttributeString("Name", child.GetDisplayName());
                    writer.WriteAttributeString("GridIndex", child.GridIndex.ToString());

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public void LoadDataFromXML()
        {
            var reader = new XmlDocument();
            reader.Load(Application.persistentDataPath + "/" + MapName + ".dm");


            if (reader.DocumentElement != null)
            {
                int row = Convert.ToInt32(reader.DocumentElement.Attributes["Row"].InnerText);
                int column = Convert.ToInt32(reader.DocumentElement.Attributes["Column"].InnerText);


                if (GameGrid.Rows == row && GameGrid.Columns == column)
                {
                    foreach (XmlNode node in reader.DocumentElement.ChildNodes)
                    {
                        if (node.Attributes != null)
                        {
                            var index = new IntVector2(node.Attributes["GridIndex"].InnerText);
                            string objectName = node.Attributes["Name"].InnerText;

                            GameGrid.DeployIfPossible(index, _deployableDictionary[objectName]);
                        }
                    }
                }
                else
                {
                    // Grid Size is Different!   
                    //TODO : Change Grid Size
                    UpdateGridSize();
                }
            }
        }

        public void UpdateGridSize()
        {
        }
    }
}