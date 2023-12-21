namespace Utility
{
    public static class MathUtility
    {
        public static int ClampListIndex(int index, int listSize) => 
            (index % listSize + listSize) % listSize;
    }
}