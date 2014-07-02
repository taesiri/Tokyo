namespace Assets.Scripts.Arena
{
    public enum CellState
    {
        Full,
        Empty
    }

    public static class CellStateEnumUtils
    {
        public static bool ToBool(this CellState state)
        {
            switch (state)
            {
                case CellState.Full:
                    return false;
                    break;
                case CellState.Empty:
                    return true;
            }

            return false;
        }
    }
}