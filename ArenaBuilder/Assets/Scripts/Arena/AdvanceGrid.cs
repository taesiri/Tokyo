using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class AdvanceGrid : MonoBehaviour
    {
        #region GridProperties

        [SerializeField, HideInInspector] public AdvanceGridCell[] Cells;
        [HideInInspector] public Transform ChildTransform;
        [HideInInspector] public int Columns;
        public float GlobalCellWidth = 1;
        [HideInInspector] public int Rows;

        #endregion

        #region PlaneFields

        public Transform PlaneTransform;
        private float _boundX;
        private float _boundY;
        private float _boundZ;
        private Vector3 _planeBottomLeftPosition;

        public Vector3 GetBottomLeftOfPlane
        {
            get
            {
                UpdatePlaneDetails();
                return _planeBottomLeftPosition;
            }
        }

        #endregion

        #region DebugOnly

        [HideInInspector] public bool DrawDebugCellStatus = false;
        [HideInInspector] public bool DrawDebugLines = false;

        #endregion

        public void Start()
        {
            UpdatePlaneDetails();

            Cells = new AdvanceGridCell[Rows*Columns];
            for (int i = 0; i < Rows*Columns; i++)
            {
                Cells[i] = new AdvanceGridCell {IsEmpty = true};
            }
        }

        private void UpdatePlaneDetails()
        {
            _boundX = PlaneTransform.renderer.bounds.size.x;
            _boundY = PlaneTransform.renderer.bounds.size.y;
            _boundZ = PlaneTransform.renderer.bounds.size.z;

            _planeBottomLeftPosition = PlaneTransform.position - new Vector3(_boundX/2f, _boundY/2f, _boundZ/2f);
        }

        public void Update()
        {
        }

        public bool IsPlaceableWithOffset(TileMap tile, IntVector2 cellIndex)
        {
            for (int i = 0; i < tile.TileSize.X; i++)
            {
                for (int j = 0; j < tile.TileSize.Y; j++)
                {
                    int posX = i + cellIndex.X - tile.TileOffset.X;
                    int posY = -j + cellIndex.Y + tile.TileOffset.Y;
                    if (posX < Rows && 0 <= posX && posY < Columns && 0 <= posY)
                        if (!Cells[CalculateIndex(posX, posY)].IsEmpty)
                            return false;
                        else
                        {
                        }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void UpdateTilesStateWithOffset(Deployable deployableObject, IntVector2 cellIndex, CellState newState)
        {
            for (int i = 0; i < deployableObject.TileMap.TileSize.X; i++)
            {
                for (int j = 0; j < deployableObject.TileMap.TileSize.Y; j++)
                {
                    int posX = i + cellIndex.X - deployableObject.TileMap.TileOffset.X;
                    int posY = -j + cellIndex.Y + deployableObject.TileMap.TileOffset.Y;

                    if (posX < Rows && 0 <= posX && posY < Columns && 0 <= posY)
                    {
                        int index = CalculateIndex(posX, posY);

                        Cells[index].IsEmpty = newState.ToBool();

                        if (newState == CellState.Empty)
                        {
                            Cells[index].InCellObject = null;
                        }
                        else if (newState == CellState.Full)
                        {
                            Cells[index].InCellObject = deployableObject;
                        }
                    }
                }
            }
        }


        public bool DropAtDeployableIfPossible(Ray ray, Deployable deployableObject)
        {
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, 100, 1 << 10);
            if (hitInfo.collider)
            {
                Vector3 loc = hitInfo.point - _planeBottomLeftPosition;
                var index = new IntVector2(loc.x*Columns/_boundX, loc.y*Rows/_boundY);

                if (IsPlaceableWithOffset(deployableObject.TileMap, index))
                {
                    Vector3 pos = IndexToWorldPosition(index);
                    Vector2 wOffset = deployableObject.TileMap.GetWorldTransformOffset(GlobalCellWidth);
                    pos.x += wOffset.x;
                    pos.y += wOffset.y;

                    deployableObject.transform.position = pos;
                    deployableObject.ParentAdvanceGridCell = Cells[CalculateIndex(index)];
                    deployableObject.GridIndex = index;
                    
                    UpdateTilesStateWithOffset(deployableObject, index, CellState.Full);
                    return true;
                }
            }

            return false;
        }

        public void DeployIfPossible(Ray ray, Deployable deployableObject)
        {
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, 100, 1 << 10);
            if (hitInfo.collider)
            {
                Vector3 loc = hitInfo.point - _planeBottomLeftPosition;
                var index = new IntVector2(loc.x*Columns/_boundX, loc.y*Rows/_boundY);

                if (IsPlaceableWithOffset(deployableObject.TileMap, index))
                {
                    Vector3 pos = IndexToWorldPosition(index);
                    Vector2 wOffset = deployableObject.TileMap.GetWorldTransformOffset(GlobalCellWidth);
                    pos.x += wOffset.x;
                    pos.y += wOffset.y;

                    var newCell = (Deployable) Instantiate(deployableObject, pos, Quaternion.identity);
                    newCell.transform.parent = ChildTransform;
                    newCell.gameObject.layer = 9;
                    newCell.ParentAdvanceGridCell = Cells[CalculateIndex(index)];
                    newCell.GridIndex = index;

                    UpdateTilesStateWithOffset(newCell, index, CellState.Full);
                }
            }
        }

        public void EraseTiles(Ray ray)
        {
            RaycastHit hitInfo;
            Physics.Raycast(ray, out hitInfo, 100, 1 << 10);
            if (hitInfo.collider)
            {
                Vector3 loc = hitInfo.point - _planeBottomLeftPosition;
                var index = new IntVector2(loc.x*Columns/_boundX, loc.y*Rows/_boundY);

                Deployable toDeleteTile = Cells[CalculateIndex(index)].InCellObject;
                if (toDeleteTile)
                {
                    IntVector2 gIdx = toDeleteTile.GridIndex;
                    UpdateTilesStateWithOffset(toDeleteTile, gIdx, CellState.Empty);
                    Destroy(toDeleteTile.gameObject);
                }
            }
        }


        public int CalculateIndex(int x, int y)
        {
            if (x < 0 || y < 0)
                return -1;
            return x + (y*Rows);
        }

        public int CalculateIndex(IntVector2 index)
        {
            if (index.X < 0 || index.Y < 0)
                return -1;
            return index.X + (index.Y*Rows);
        }

        public Vector3 IndexToWorldPosition(IntVector2 index)
        {
            var pos = new Vector3(_planeBottomLeftPosition.x + index.X*GlobalCellWidth, _planeBottomLeftPosition.y + index.Y*GlobalCellWidth, 0);
            var offset = new Vector3(GlobalCellWidth/2f, GlobalCellWidth/2f, 0);
            return pos + offset;
        }


        public void OnDrawGizmosSelected()
        {
            if (Application.isPlaying)
            {
                if (DrawDebugCellStatus)
                {
                    for (int i = 0; i < Rows; i++)
                    {
                        for (int j = 0; j < Columns; j++)
                        {
                            Gizmos.color = !Cells[CalculateIndex(i, j)].IsEmpty ? new Color(1, 0, 0, 0.8F) : new Color(0, 1, 0, 0.8F);
                            Gizmos.DrawCube(IndexToWorldPosition(new IntVector2(i, j)), new Vector3(1, 1, 1));
                            Gizmos.color = Color.yellow;

                            if (Cells[CalculateIndex(i, j)].InCellObject)
                                Gizmos.DrawSphere(IndexToWorldPosition(new IntVector2(i, j)), 0.5f);
                        }
                    }
                }
            }
        }
    }
}