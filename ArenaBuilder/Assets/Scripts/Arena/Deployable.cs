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
        public List<PropertyInfo> ListOfAllProperties;
        public List<string> NamesOfAllProperties;
        public AdvanceGridCell ParentAdvanceGridCell;
        public GridCell ParentGridCell;
        [SerializeField] public TileMap TileMap;
        private bool _isBlack;
        private Color _selfColor = Color.yellow;


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


        public void Start()
        {
            _selfColor = renderer.material.color;
        }

        public abstract void OnTick();
        public abstract string GetDisplayName();

        public List<PropertyInfo> GetInGameProperties()
        {
            if (ListOfAllProperties == null)
            {
                ListOfAllProperties = new List<PropertyInfo>();
                UpdateListOfInGameProperties();
            }
            return ListOfAllProperties;
        }

        public void UpdateListOfInGameProperties()
        {
            ListOfAllProperties = GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof (InGameProperty))).ToList();
        }

        public List<string> GetInGamePropertiesNames()
        {
            if (NamesOfAllProperties == null)
            {
                NamesOfAllProperties = new List<string>();

                if (ListOfAllProperties == null)
                {
                    ListOfAllProperties = new List<PropertyInfo>();
                    UpdateListOfInGameProperties();
                }

                foreach (PropertyInfo prop in ListOfAllProperties)
                {
                    object[] attr = prop.GetCustomAttributes(true);

                    foreach (object atr in attr)
                    {
                        var igpAttr = atr as InGameProperty;
                        if (igpAttr != null)
                        {
                            NamesOfAllProperties.Add(igpAttr.Name);
                        }
                    }
                }
            }
            return NamesOfAllProperties;
        }
    }
}