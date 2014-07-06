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
        public abstract void OnTick();
        public abstract string GetDisplayName();
    }
}