using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class PointOnPlane : MonoBehaviour
    {
        public bool[] Cells;
        public int Cols = 8;
        public int Rows = 8;
        private Vector3 _actualPosition;
        private float _boundX;
        private float _boundY;
        private float _boundZ;


        public void Start()
        {
            _boundX = renderer.bounds.size.x;
            _boundY = renderer.bounds.size.y;
            _boundZ = renderer.bounds.size.z;

            _actualPosition = transform.position - new Vector3(_boundX/2f, _boundY/2f, _boundZ/2f);
            Cells = new bool[Rows*Cols];

        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                Physics.Raycast(ray, out hitInfo, 100);
                if (hitInfo.collider)
                {
                    Debug.Log(ReuturnCellId(hitInfo.point - _actualPosition, false));
                }
            }
        }

        public IntVector2 ReuturnCellId(Vector3 loc, bool invertY)
        {
            if (invertY)
                return new IntVector2(Rows - (loc.y*Rows/_boundY), loc.x*Cols/_boundX);
            return new IntVector2(loc.y*Rows/_boundY, loc.x*Cols/_boundX);
        }
    }
}