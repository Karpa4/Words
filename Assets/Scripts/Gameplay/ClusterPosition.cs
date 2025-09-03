namespace Gameplay
{
    public struct ClusterPosition
    {
        public int WordIndex { get; }
        public int StartCellIndex { get; }
        public bool IsDefault => StartCellIndex == -1 && WordIndex == -1;
        
        public ClusterPosition(int wordIndex, int startCellIndex)
        {
            WordIndex = wordIndex;
            StartCellIndex = startCellIndex;
        }

        public static ClusterPosition Default()
        {
            return new ClusterPosition(-1, -1);
        }
    }
}