using UnityEngine;

namespace Assets.Scripts.Arena
{
    public abstract class Deployable : MonoBehaviour
    {
        public DeploymentMethod DeploymentMethod;
        public GridCellObject ParentGridCellObject;
        public abstract void OnTick();
        public abstract string GetDisplayName();
    }
}