using System;
using Assets.Scripts.Helpers;
using UnityEngine;

namespace Assets.Scripts.Arena
{
    [Serializable]
    public class TileMap
    {
        // Don't use this!
        public IntVector2 TileMovementOffset;
        public IntVector2 TileOffset;
        public IntVector2 TileSize;

        public override string ToString()
        {
            return string.Format("Tile Size ({0})", TileSize);
        }
    }

    public static class TileMapUtils
    {
        public static Vector2 GetWorldTransformOffset(this TileMap tile, float worldCellWidth)
        {
            float x = (tile.TileSize.X/2f*worldCellWidth) - (worldCellWidth/2f);
            float y = tile.TileSize.Y/2f*worldCellWidth - worldCellWidth/2f;

            // Calculating Tile Offset
            x += -tile.TileOffset.X*worldCellWidth;
            y += -tile.TileOffset.Y*worldCellWidth;

            return new Vector2(x, -y);
        }
    }
}