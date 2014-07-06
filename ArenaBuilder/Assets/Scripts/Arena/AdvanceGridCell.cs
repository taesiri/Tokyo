using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class AdvanceGridCell
    {
        public Deployable InCellObject;
        [SerializeField] public IntVector2 Index;
        [SerializeField] public bool IsEmpty;
        public GameObject ParentGrid;
    }
}