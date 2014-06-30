using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class GridCellObject : MonoBehaviour
    {
        public void GotHit()
        {
            Debug.Log(string.Format("Object hitted at {0}", gameObject.name));
        }
    }
}