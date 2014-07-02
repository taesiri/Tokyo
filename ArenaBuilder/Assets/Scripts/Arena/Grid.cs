using UnityEngine;

namespace Assets.Scripts.Arena
{
    public class Grid : MonoBehaviour
    {
        [HideInInspector] public int CellWidth = 1;
        [SerializeField] public GridCell[] Cells;
        public int Column;
        public GameObject GridTileObject;
        public int Row;

        public bool IsPlaceable(TileMap tile, GridCell originCell)
        {
            Debug.Log(string.Format("TileMap Size:{0}", tile.TileSize));
            Debug.Log(string.Format("Original Cell: {0}", originCell.GridPostion));

            for (int i = 0; i < tile.TileSize.X; i++)
            {
                for (int j = 0; j < tile.TileSize.Y; j++)
                {
                    if (i + originCell.GridPostion.X < Row && j + originCell.GridPostion.Y < Column)
                        if (!Cells[GetIndex(i + originCell.GridPostion.X, j + originCell.GridPostion.Y)].IsEmpty)
                            return false;
                        else
                        {
                        }
                    else
                    {
                        Debug.Log(i + originCell.GridPostion.X);
                        Debug.Log(j + originCell.GridPostion.Y);
                        return false;
                    }
                }
            }

            return true;
        }

        public void UpdateTilesState(TileMap tile, GridCell originCell, CellState newState)
        {
            for (int i = 0; i < tile.TileSize.X; i++)
            {
                for (int j = 0; j < tile.TileSize.Y; j++)
                {
                    if (i + originCell.GridPostion.X < Row && j + originCell.GridPostion.Y < Column)
                    {
                        Cells[GetIndex(i + originCell.GridPostion.X, j + originCell.GridPostion.Y)].IsEmpty =
                            newState.ToBool();


                        //if (newState == CellState.Empty)
                        //{
                        //    Cells[GetIndex(i + originCell.GridPostion.X, j + originCell.GridPostion.Y)].InCellObject =
                        //        null;
                        //}
                    }
                }
            }
        }

        public int GetIndex(int x, int y)
        {
            return x + (y*Row);
        }
    }
}