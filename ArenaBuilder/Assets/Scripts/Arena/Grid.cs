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
        [HideInInspector] public bool DrawGizmo = true;

        public bool IsPlaceableWithOffset(TileMap tile, GridCell originCell)
        {
            for (int i = 0; i < tile.TileSize.X; i++)
            {
                for (int j = 0; j < tile.TileSize.Y; j++)
                {
                    int posX = i + originCell.GridPostion.X - tile.TileOffset.X;
                    int posY = j + originCell.GridPostion.Y - tile.TileOffset.Y;
                    if (posX < Row && posY < Column)
                        if (!Cells[GetIndex(posX, posY)].IsEmpty)
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


        public void UpdateTilesStateWithOffset(Deployable deployableObject, GridCell originCell, CellState newState)
        {
            for (int i = 0; i < deployableObject.TileMap.TileSize.X; i++)
            {
                for (int j = 0; j < deployableObject.TileMap.TileSize.Y; j++)
                {
                    int posX = i + originCell.GridPostion.X - deployableObject.TileMap.TileOffset.X;
                    int posY = j + originCell.GridPostion.Y - deployableObject.TileMap.TileOffset.Y;

                    if (posX < Row && posY < Column)
                    {
                        int index = GetIndex(posX, posY);

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


        public bool IsPlaceable(TileMap tile, GridCell originCell)
        {
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
                        return false;
                    }
                }
            }

            return true;
        }

        public void UpdateTilesState(Deployable deployableObject, GridCell originCell, CellState newState)
        {
            for (int i = 0; i < deployableObject.TileMap.TileSize.X; i++)
            {
                for (int j = 0; j < deployableObject.TileMap.TileSize.Y; j++)
                {
                    if (i + originCell.GridPostion.X < Row && j + originCell.GridPostion.Y < Column)
                    {
                        int index = GetIndex(i + originCell.GridPostion.X, j + originCell.GridPostion.Y);

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

        public int GetIndex(int x, int y)
        {
            return x + (y*Row);
        }
    }
}