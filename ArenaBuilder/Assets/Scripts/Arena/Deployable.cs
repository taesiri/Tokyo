using UnityEngine;

namespace Assets.Scripts.Arena
{
    public abstract class Deployable : MonoBehaviour
    {
        public DeploymentMethod DeploymentMethod;
        public GridCell ParentGridCell;
        public abstract void OnTick();
        public abstract string GetDisplayName();
    }
}