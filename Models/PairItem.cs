using System;

namespace RandomPairsApp.Models
{
    public class PairItem
    {
        // Time To Live, decrease every second
        public int TTL;

        // Initial TTL
        public readonly int Second;
        public int Value;

        // Flag indicates whether the value of this item was used for a Sum request or not.
        // False by default.
        public bool IsSummed;

        public PairItem()
        {
            TTL = Second = 0;
            Value = 0;
            IsSummed = false;
        }

        public PairItem(int ttl, int value, bool isSummed = false)
        {
            TTL = Second = ttl;
            Value = value;
            IsSummed = isSummed;
        }
    }
}