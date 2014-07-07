using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class AdvanceGridCell
    {
        public Deployable InCellObject;
        [SerializeField] public bool IsEmpty;
        public AdvanceGrid ParentGrid;
        // Consider adding Index field for Grid Index!
    }
}