using System;

namespace Expedition0.Util
{
    public static class BitUtils
    {
        public static int CountOnes(int number)
        {
            uint uNumber = (uint) number;
            int count = 0;
            for (; uNumber != 0; ++count)
            {
                uNumber &= (uNumber - 1);  // Clears the lowest set bit
            }
            return count;
        }
    }
}