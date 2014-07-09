using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class AdvanceBrain : MonoBehaviour
    {
        #region BrainOrgans

        [HideInInspector] public BrainStates BrainState;
        public List<Deployable> DeployableList;
        public AdvanceGrid GameGrid;
        private bool _allowToMove;
        private Dictionary<string, Deployable> _deployableDictionary;
        private bool _gridLinesVisibilityStatus = true;
        private bool _isDown;
        private string _mapName = "SampleMap";
        private Deployable _objectToDeploy;
        private Deployable _selectedDeployable;
        private Vector3 _selectedObjectDeltaPosition;

        public bool ShowGridLines
        {
            get { return _gridLinesVisibilityStatus; }
            set { _gridLinesVisibilityStatus = value; }
        }

        #endregion

        #region GUIHelpers

        private readonly GUILocationHelper _location = new GUILocationHelper();
        private readonly string[] _menuStrings = {"Create", "Edit", "Erase", "Play!"};
        private Matrix4x4 _guiMatrix;
        private int _menuSelectedIndex;

        #endregion

        #region PropertySystem

        private List<PropertyInfo> _booleanProperties;
        private ObservableList<bool> _booleanPropertiesValues;
        private List<PropertyInfo> _selectedObjectProperties;

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


                        //if (_selectedObjectProperties != null)
                        //{
                        //    for (int i = 0; i < _selectedObjectProperties.Count; i++)
                        //    {
                        //        GUI.Label(new Rect(10, 600 + (i*45), 400, 50), _selectedObjectProperties[i].CanWrite + " " + _selectedObjectProperties[i].Name);
                        //    }
                        //}

                        if (_booleanProperties != null)
                        {
                            for (int i = 0; i < _booleanProperties.Count; i++)
                            {
                                _booleanPropertiesValues[i] = GUI.Toggle(new Rect(10, 600 + (i*45), 400, 50), _booleanPropertiesValues[i], _booleanProperties[i].Name);

                                // Worst Way possbile to handle changes!
                                //_booleanProperties[i].SetValue(_selectedDeployable, _booleanPropertiesValues[i], null);
                            }
                        }
                    }
                    break;
                case BrainStates.CreationMode:
                    GUI.matrix = _guiMatrix;


                    for (int i = 0; i < DeployableList.Count; i++)
                    {
                        if (GUI.RepeatButton(new Rect(i*155, 150, 150, 50), DeployableList[i].GetDisplayName()))
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


            if (GUI.Button(new Rect(10, 260, 80, 50), "SAVE"))
            {
                SaveDataToXML();
            }
            if (GUI.Button(new Rect(10, 310, 80, 50), "LOAD"))
            {
                LoadDataFromXML();
            }
            if (GUI.Button(new Rect(10, 360, 80, 50), "Clear"))
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
                        //#if UNITY_IPHONE || UNITY_ANDROID
                        //            HandleTouchEvents();
                        //#elif !UNITY_FLASH
                        //              CreationUpdate();
                        //#endif
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

                    UpdatePropertiesList();
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

                    if (!GameGrid.DropDeployableIfPossible(ray, _selectedDeployable))
                    {
                        ResetSelectedObjectPosition();
                    }
                }

                _isDown = false;
                _allowToMove = false;
            }
        }

        private void UpdatePropertiesList()
        {
            _selectedObjectProperties = new List<PropertyInfo>();
            _booleanProperties = new List<PropertyInfo>();
            _booleanPropertiesValues = new ObservableList<bool>();


            _selectedObjectProperties = _selectedDeployable.GetInGameProperties();

            if (_selectedDeployable != null)
            {
                foreach (PropertyInfo prop in _selectedObjectProperties)
                {
                    if (prop.PropertyType == typeof (bool))
                    {
                        _booleanProperties.Add(prop);
                        _booleanPropertiesValues.Add(Convert.ToBoolean(prop.GetValue(_selectedDeployable, null)));
                    }
                }
            }
            _booleanPropertiesValues.Changed += _booleanPropertiesValues_Changed;
        }

        public void _booleanPropertiesValues_Changed(int index)
        {
            _booleanProperties[index].SetValue(_selectedDeployable, _booleanPropertiesValues[index], null);
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


            using (XmlWriter writer = XmlWriter.Create(Application.persistentDataPath + "/" + _mapName + ".dm"))
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
            reader.Load(Application.persistentDataPath + "/" + _mapName + ".dm");


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