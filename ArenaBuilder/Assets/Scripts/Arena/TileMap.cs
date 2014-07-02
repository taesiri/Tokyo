using System;
using Assets.Scripts.Helpers;

namespace Assets.Scripts.Arena
{
    [Serializable]
    public class TileMap
    {
        public IntVector2 TileMovementOffset;
        public IntVector2 TileSize;
    }
}