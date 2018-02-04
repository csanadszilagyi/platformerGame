using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace platformerGame.Utilities
{ 
    /// <summary>
    /// Weight - value table. Seed the table with weight - value pairs, 
    /// then by using roll() function, itt will pick randomly a weight(proportion),
    /// and then returns the specified type of value.
    /// Bigger weight, bigger chance...
    /// </summary>
    /// <typeparam name="T">T is the type of the value</typeparam>
    class ProbabilityRoll<T>
    {
        /// <summary>
        /// the first type of the tuple is weight, second is value
        /// </summary>
        List< Tuple<int, T> > probabilityTable;

        public ProbabilityRoll()
        {
            probabilityTable = new List<Tuple<int, T>>();
        }

        /// <summary>
        /// Uploads the probability table by adding weights - values pairs.
        /// </summary>
        /// <param name="table">array of the weight - value pairs</param>
        /// <returns></returns>
        public void seed(Tuple<int, T>[] wv_pairs)
        {
            probabilityTable.Clear();
            
            foreach (var p in wv_pairs)
            {
                probabilityTable.Add(p);
            }
        }

        /// <summary>
        /// Uploads the probability table with weights and values.
        /// Arrays must have same sizes.
        /// </summary>
        /// <param name="weights">array of weights</param>
        /// <param name="values">array of values</param>
        /// <returns>false, if the length of weights and values not equals, else true.</returns>
        public bool seed(int[] weights, T[] values)
        {
            if(weights.Length != values.Length)
            {
                return false;
            }

            int len = weights.Length;
            probabilityTable.Clear();

            for(int i = 0; i < len; i++)
            {
                probabilityTable.Add(new Tuple<int, T>(weights[i], values[i]));
            }

            return true;
        }

        public void add(int weight, T value)
        {
            probabilityTable.Add(new Tuple<int, T>(weight, value));
        }

        public void clear()
        {
            probabilityTable.Clear();
        }

        private T lookupValue(int x)
        {
            // assume 0 ≤ x < sum_of_weights
            int cumulative_weight = 0;

            for (var row = 0; row < probabilityTable.Count; row++)
            {
                cumulative_weight += probabilityTable[row].Item1;
                if (x < cumulative_weight)
                {
                    return probabilityTable[row].Item2;
                }
            }

            // should never run, because the condition inside for loop should evaulate true sometime,
            // so we always return from there. 
            return Activator.CreateInstance<T>();
        }

        public T roll(bool use_empty = false)
        {
            int sum_of_weights = 0;
            for (var row = 0; row < probabilityTable.Count; row++)
            {

                sum_of_weights += probabilityTable[row].Item1;
            }

            int x = cAppRandom.GetRandomNumber(0, use_empty ? 100 : sum_of_weights);
            return lookupValue(x);
        }

    }
}
