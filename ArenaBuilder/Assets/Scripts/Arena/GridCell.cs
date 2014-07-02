using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class GridCell : MonoBehaviour
    {
        public Transform InCellObject;
        public bool IsEmpty = true;

        public void GotHit()
        {
            Debug.Log(string.Format("Object hitted at {0}", gameObject.name));
        }
    }
}