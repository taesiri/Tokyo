using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Arena
{
    public abstract class Deployable : MonoBehaviour
    {
        public DeploymentMethod DeploymentMethod;
        public IntVector2 GridIndex;
        public GUISkin MasterGUISkin;
        public AdvanceGridCell ParentAdvanceGridCell;
        public GridCell ParentGridCell;
        public IntVector2 SelctedTileIndex;
        [SerializeField] public TileMap TileMap;
        private bool _isBlack;
        private Color _selfColor = Color.yellow;

        #region PropertyProxy

        public List<PropertyInfo> ListOfAllProperties;
        private List<GamePropertyWithName> _booleanGameProperties;
        private ObservableList<bool> _booleanPropertiesValues;
        private List<GamePropertyWithName> _floatGameProperties;
        private ObservableList<float> _floatPropertiesValues;
        private List<GamePropertyWithName> _integerGameProperties;
        private ObservableList<int> _integerPropertiesValues;
        private List<GamePropertyWithName> _stringGameProperties;
        private ObservableList<string> _stringPropertiesValues;

        #endregion

        #region GUIFields

        public bool AllowToDrawGUI = false;

        #endregion

        #region InGameProperties

        [InGameProperty(Name = "Display Name")]
        public string DisplayName { get; set; }

        [InGameProperty(Name = "Is It Active")]
        public bool IsItActive { get; set; }

        [InGameProperty(Name = "Custom Int Property")]
        public int MyIntProperty { get; set; }

        [InGameProperty(Name = "Custom Float Property")]
        public float MyFloatProperty { get; set; }


        [InGameProperty(Name = "Toggle Back Color")]
        public bool GoBlack
        {
            get { return _isBlack; }
            set
            {
                renderer.material.color = value ? Color.black : _selfColor;
                _isBlack = value;
            }
        }

        #endregion

        #region PropertySystem

        public bool HasChanged;

        public void UpdateListOfProperties()
        {
            ListOfAllProperties = new List<PropertyInfo>();

            _booleanGameProperties = new List<GamePropertyWithName>();
            _floatGameProperties = new List<GamePropertyWithName>();
            _integerGameProperties = new List<GamePropertyWithName>();
            _stringGameProperties = new List<GamePropertyWithName>();

            _booleanPropertiesValues = new ObservableList<bool>();
            _integerPropertiesValues = new ObservableList<int>();
            _floatPropertiesValues = new ObservableList<float>();
            _stringPropertiesValues = new ObservableList<string>();


            ListOfAllProperties = GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof (InGameProperty))).ToList();


            for (int i = 0, n = ListOfAllProperties.Count; i < n; i++)
            {
                // Retrieving name of Property
                object[] attr = ListOfAllProperties[i].GetCustomAttributes(true);
                InGameProperty[] igpAttr = attr.OfType<InGameProperty>().ToArray();
                string nameOfProperty = igpAttr[0].Name;

                // Type Classification
                if (ListOfAllProperties[i].PropertyType == typeof (bool))
                {
                    _booleanGameProperties.Add(new GamePropertyWithName(ListOfAllProperties[i], nameOfProperty));
                    _booleanPropertiesValues.Add(Convert.ToBoolean(ListOfAllProperties[i].GetValue(this, null)));
                }
                else if (ListOfAllProperties[i].PropertyType == typeof (int))
                {
                    _integerGameProperties.Add(new GamePropertyWithName(ListOfAllProperties[i], nameOfProperty));
                    _integerPropertiesValues.Add(Convert.ToInt32(ListOfAllProperties[i].GetValue(this, null)));
                }
                else if (ListOfAllProperties[i].PropertyType == typeof (float))
                {
                    _floatGameProperties.Add(new GamePropertyWithName(ListOfAllProperties[i], nameOfProperty));
                    _floatPropertiesValues.Add(Convert.ToSingle(ListOfAllProperties[i].GetValue(this, null)));

                }
                else if (ListOfAllProperties[i].PropertyType == typeof (string))
                {
                    _stringGameProperties.Add(new GamePropertyWithName(ListOfAllProperties[i], nameOfProperty));
                    _stringPropertiesValues.Add(Convert.ToString(ListOfAllProperties[i].GetValue(this, null)));
                }
                // ELSE ! Do nothing!
            }

            _booleanPropertiesValues.Changed += _booleanPropertiesValues_Changed;
            _integerPropertiesValues.Changed += _integerPropertiesValues_Changed;
            _floatPropertiesValues.Changed += _floatPropertiesValues_Changed;
            _stringPropertiesValues.Changed += _stringPropertiesValues_Changed;
        }

        private void _booleanPropertiesValues_Changed(int index)
        {
            _booleanGameProperties[index].GamePropertyInfo.SetValue(this, _booleanPropertiesValues[index], null);
            HasChanged = true;
        }

        private void _integerPropertiesValues_Changed(int index)
        {
            _integerGameProperties[index].GamePropertyInfo.SetValue(this, _integerPropertiesValues[index], null);
            HasChanged = true;
        }

        private void _floatPropertiesValues_Changed(int index)
        {
            _floatGameProperties[index].GamePropertyInfo.SetValue(this, _floatPropertiesValues[index], null);
            HasChanged = true;
        }

        private void _stringPropertiesValues_Changed(int index)
        {
            _stringGameProperties[index].GamePropertyInfo.SetValue(this, _stringPropertiesValues[index], null);
            HasChanged = true;
        }

        public List<GamePropertyWithName> GetPropertyOfType(Type t)
        {
            if (ListOfAllProperties == null)
                UpdateListOfProperties();


            if (t == typeof (bool))
            {
                return _booleanGameProperties;
            }
            if (t == typeof (int))
            {
                return _integerGameProperties;
            }
            if (t == typeof (float))
            {
                return _floatGameProperties;
            }
            if (t == typeof (string))
            {
                return _stringGameProperties;
            }

            return null;
        }

        public void SetProperty(Type t, string propertyName, string propertyValue)
        {
            UpdateListOfProperties();

            if (t == typeof (bool))
            {
                GamePropertyWithName propertyInfo = _booleanGameProperties.FirstOrDefault(pInfo => pInfo.PropertyName == propertyName);

                if (propertyInfo != null)
                {
                    // Not working properly!
                    bool value;
                    bool.TryParse(propertyValue, out value);
                    propertyInfo.GamePropertyInfo.SetValue(this, value, null);
                }
            }
            if (t == typeof (int))
            {
                GamePropertyWithName propertyInfo = _integerGameProperties.FirstOrDefault(pInfo => pInfo.PropertyName == propertyName);
                if (propertyInfo != null)
                {
                    int value;
                    int.TryParse(propertyValue, out value);
                    propertyInfo.GamePropertyInfo.SetValue(this, value, null);
                }
            }
            if (t == typeof (float))
            {
                GamePropertyWithName propertyInfo = _floatGameProperties.FirstOrDefault(pInfo => pInfo.PropertyName == propertyName);
                if (propertyInfo != null)
                {
                    float value;
                    float.TryParse(propertyValue, out value);
                    propertyInfo.GamePropertyInfo.SetValue(this, value, null);
                }
            }
            if (t == typeof (string))
            {
                GamePropertyWithName propertyInfo = _stringGameProperties.FirstOrDefault(pInfo => pInfo.PropertyName == propertyName);
                if (propertyInfo != null)
                {
                    propertyInfo.GamePropertyInfo.SetValue(this, propertyValue, null);
                }
            }

            UpdateListOfProperties();
            HasChanged = true;
        }

        #endregion

        public void Start()
        {
            _selfColor = renderer.material.color;

            if (MasterGUISkin == null)
            {
                var gSking = Resources.Load("GUISkin/defaultGUIStyle", typeof (GUISkin)) as GUISkin;
                MasterGUISkin = gSking;
            }
        }

        public abstract void OnTick();
        public abstract string GetDisplayName();

        #region GUI

        public Vector2 ScrollPosition = Vector2.zero;

        public void OnGUI()
        {
            GUI.matrix = AdvanceBrain.GUIMatrix;

            if (AllowToDrawGUI && AdvanceBrain.AllowOthersToDrawOnGUI)
            {
                if (ListOfAllProperties == null)
                    UpdateListOfProperties();


                ScrollPosition = GUI.BeginScrollView(new Rect(25, 320, 500, 300), ScrollPosition, new Rect(0, 0, 400, 600), MasterGUISkin.horizontalScrollbar, MasterGUISkin.verticalScrollbar);

                int offset = 0;
                if (_booleanGameProperties != null)
                {
                    for (int i = 0, n = _booleanGameProperties.Count; i < n; i++)
                    {
                        _booleanPropertiesValues[i] = GUI.Toggle(new Rect(0, (i*30), 400, 25), _booleanPropertiesValues[i], _booleanGameProperties[i].PropertyName, MasterGUISkin.toggle);
                    }

                    offset += _booleanGameProperties.Count;
                }

                if (_integerGameProperties != null)
                {
                    for (int i = 0, n = _integerGameProperties.Count; i < n; i++)
                    {
                        GUI.Label(new Rect(0, ((offset + i)*45), 150, 30), _integerGameProperties[i].PropertyName, MasterGUISkin.label);


                        string str = GUI.TextField(new Rect(160, ((offset + i)*45), 210, 30), _integerPropertiesValues[i] == 0 ? "" : _integerPropertiesValues[i].ToString(), MasterGUISkin.textField);

                        int value;

                        if (int.TryParse(str, out value))
                        {
                            _integerPropertiesValues[i] = value;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(str))
                            {
                                _integerPropertiesValues[i] = 0;
                            }
                        }
                    }
                    offset += _integerGameProperties.Count;
                }


                if (_floatGameProperties != null)
                {
                    for (int i = 0, n = _floatGameProperties.Count; i < n; i++)
                    {
                        GUI.Label(new Rect(0, ((offset + i)*45), 150, 30), _floatGameProperties[i].PropertyName, MasterGUISkin.label);

                        string str = GUI.TextField(new Rect(160, ((offset + i)*45), 210, 30), _floatPropertiesValues[i] == 0.0f ? "" : _floatPropertiesValues[i].ToString(), MasterGUISkin.textField);
                        float value;

                        if (float.TryParse(str, out value))
                        {
                            _floatPropertiesValues[i] = value;
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(str))
                            {
                                _floatPropertiesValues[i] = 0;
                            }
                        }
                    }
                    offset += _floatGameProperties.Count;
                }


                if (_stringGameProperties != null)
                {
                    for (int i = 0, n = _stringGameProperties.Count; i < n; i++)
                    {
                        GUI.Label(new Rect(0, ((offset + i)*45), 150, 30), _stringGameProperties[i].PropertyName, MasterGUISkin.label);
                        _stringPropertiesValues[i] = GUI.TextField(new Rect(160, ((offset + i)*45), 210, 30), _stringPropertiesValues[i], MasterGUISkin.textField);
                    }
                }

                GUI.EndScrollView();


                GUI.matrix = Matrix4x4.identity;
            }
        }

        #endregion
    }
}