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
        public AdvanceGridCell ParentAdvanceGridCell;
        public GridCell ParentGridCell;
        [SerializeField] public TileMap TileMap;

        [InGameProperty(Name = "Display Name")]
        public string CustomProperty1
        {
            get { return GetDisplayName(); }
        }

        [InGameProperty(Name = "IsItActive")]
        public bool IsItActive { get; set; }


        public abstract void OnTick();
        public abstract string GetDisplayName();

        public List<PropertyInfo> GetInGameProperties()
        {
            return GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof (InGameProperty))).ToList();
        }

    }
}