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
        public bool AllowToDrawGUI = false;
        public DeploymentMethod DeploymentMethod;
        public IntVector2 GridIndex;
        public AdvanceGridCell ParentAdvanceGridCell;
        public GridCell ParentGridCell;
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

        #region InGameProperties

        [InGameProperty(Name = "Display Name")]
        public string CustomProperty1
        {
            get { return GetDisplayName(); }
        }

        [InGameProperty(Name = "Is It Active")]
        public bool IsItActive { get; set; }


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
        }

        private void _integerPropertiesValues_Changed(int index)
        {
            _integerGameProperties[index].GamePropertyInfo.SetValue(this, _booleanPropertiesValues[index], null);
        }

        private void _floatPropertiesValues_Changed(int index)
        {
            _floatGameProperties[index].GamePropertyInfo.SetValue(this, _booleanPropertiesValues[index], null);
        }

        private void _stringPropertiesValues_Changed(int index)
        {
            _stringGameProperties[index].GamePropertyInfo.SetValue(this, _booleanPropertiesValues[index], null);
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

        #endregion

        public void Start()
        {
            _selfColor = renderer.material.color;
        }

        public abstract void OnTick();
        public abstract string GetDisplayName();

        #region GUI

        public void OnGUI()
        {
            if (AllowToDrawGUI && AdvanceBrain.AllowOthersToDrawOnGUI)
            {
                if (ListOfAllProperties == null)
                    UpdateListOfProperties();


                if (_booleanGameProperties != null)
                {
                    for (int i = 0, n = _booleanGameProperties.Count; i < n; i++)
                    {
                        _booleanPropertiesValues[i] = GUI.Toggle(new Rect(10, 500 + (i*45), 400, 50), _booleanPropertiesValues[i], _booleanGameProperties[i].PropertyName);
                    }
                }

                // TODO: Other Types of Properties
            }
        }

        #endregion
    }
}