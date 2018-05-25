using System;
using System.Collections.Generic;
using System.Linq;
using RandomPairsApp.Models;

namespace RandomPairsApp.Services
{

    // The service provider to facilitate thread-safe manipulation of the Random Pairs list
    // and related functionalities.
    public class RandomPairsServiceProvider
    {
        public List<PairItem> RandomPairsList { get; private set; }

        public List<int> SumList { get; private set; }

        private const int MAX_PAIRS = 10000;
        private const int SUM_AMOUNT = 10;
        private const int MIN_TTL = 1;
        private const int MAX_TTL = 100;
        private const int MIN_VALUE = 1000;
        private const int MAX_VALUE = 10000;

        // Lock objects for thread usage
        private Object _pairsListLock = new Object();
        private Object _sumListLock = new Object();

        public RandomPairsServiceProvider()
        {
            RandomPairsList = new List<PairItem>();
            SumList = new List<int>();
        }

        // This is called every 1 second by the Generator service
        public void UpdatePairsList()
        {
            lock (_pairsListLock)
            {
                RemoveExpiredPairs();
                RemoveExceedPairs();
                GenerateNewPair();
            }
        }

        /*
         * PUBLIC METHODS
         */

        // Calculate the sum of the 10 oldest numbers that haven't summed yet.
        // If not enough 10 numbers, calculate whatever available and return.
        public SumResult GetSum()
        {
            int sum = 0;
            SumResult sumResult = null;

            try
            {
                lock (_pairsListLock)
                {
                    if (RandomPairsList != null)
                    {
                        int count = 0;
                        for (int i = 0; i < RandomPairsList.Count && count < SUM_AMOUNT; i++)
                        {
                            if (RandomPairsList[i].TTL > 0 && !RandomPairsList[i].IsSummed)
                            {
                                RandomPairsList[i].IsSummed = true;
                                sum += RandomPairsList[i].Value;
                                count++;
                            }
                        }
                    }
                }

                // If the sum is actually calculated
                if (sum > 0)
                {
                    int sumCount = InsertSumValueAscending(sum);
                    sumResult = new SumResult()
                    {
                        NewSumValue = sum,
                        SumCount = sumCount
                    };
                }
            }
            catch (Exception ex)
            {
                // TODO: log error
            }

            return sumResult;
        }

        // Get median value from the sorted, duplication-removed Sum list
        public float GetMedian()
        {
            float median = 0;

            lock (_sumListLock)
            {
                try
                {
                    if (SumList != null && SumList.Count > 0)
                    {
                        var distinctList = SumList.Distinct().ToList();
                        int midIndex = (distinctList.Count - 1) / 2;
                        median = distinctList[midIndex];

                        // If there are 2 middle items, calculate the avg. value
                        if (distinctList.Count % 2 == 0)
                        {
                            median = (median + distinctList[midIndex + 1]) / 2;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO: log error
                }
            }

            return median;
        }

        // Get the size of the Sum list
        public int? GetSumCount()
        {
            int? sumCount = null;

            lock (_sumListLock)
            {
                try
                {
                    if (SumList == null)
                        return 0;
                    else
                        return SumList.Count;
                }
                catch (Exception ex)
                {
                    // TODO: log error
                }
            }

            return sumCount;
        }

        // Lock access to the pairs list and make a snapshot copy of it
        public List<PairItem> GetPairsListSnapshot()
        {
            List<PairItem> result = new List<PairItem>();

            lock (_pairsListLock)
            {
                result.AddRange(RandomPairsList);
            }

            return result;
        }

        /*
         * PRIVATE METHODS
         */

        // Randomize values for a new pair
        private void GenerateNewPair()
        {
            if (RandomPairsList == null)
                RandomPairsList = new List<PairItem>();

            Random rand = new Random(DateTime.UtcNow.Millisecond);
            int second = rand.Next(MIN_TTL, MAX_TTL);
            int value = rand.Next(MIN_VALUE, MAX_VALUE);

            RandomPairsList.Add(new PairItem(second, value));
        }

        private void RemoveExpiredPairs()
        {
            if (RandomPairsList == null || RandomPairsList.Count == 0)
                return;

            try
            {
                // reduce TTL count for all pairs, assuming this method is called once per second
                RandomPairsList.ForEach(p => p.TTL--); 
                // then remove the expired ones
                RandomPairsList.RemoveAll(p => p.TTL <= 0);
            }
            catch (Exception ex)
            {
                // TODO: log error
            }
        }

        private void RemoveExceedPairs()
        {
            try
            {
                int exceedNumber = RandomPairsList.Count - MAX_PAIRS + 1;

                if (exceedNumber > 0)
                {
                    RandomPairsList.RemoveRange(0, exceedNumber);
                }
            }
            catch (Exception ex)
            {
                // TODO: log error
            }
        }

        // Insert sum value into the List in ascending order
        private int InsertSumValueAscending(int value)
        {
            lock (_sumListLock)
            {
                try
                {
                    if (SumList == null)
                        SumList = new List<int>();

                    int insertIndex = 0;

                    for (insertIndex = 0; insertIndex < SumList.Count; insertIndex++)
                    {
                        if (SumList[insertIndex] >= value)
                            break;
                    }

                    SumList.Insert(insertIndex, value);
                    return SumList.Count;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}