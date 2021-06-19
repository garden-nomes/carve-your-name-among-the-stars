using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// given a set of names, can construct weird-sounding but plausible names using Markov chains
// based on https://donjon.bin.sh/code/name/
public class MarkovNameGenerator
{
    private Dictionary<string, FrequencyTable<string>> characterFrequencies = new Dictionary<string, FrequencyTable<string>>();
    private FrequencyTable<int> partCountsFrequencies = new FrequencyTable<int>();
    private FrequencyTable<int> lengthFrequencies = new FrequencyTable<int>();
    private FrequencyTable<string> startFrequencies = new FrequencyTable<string>();

    public MarkovNameGenerator(string[] corpus)
    {
        ProcessCorpus(corpus);
    }

    public string Generate()
    {
        int partCount = partCountsFrequencies.Select();
        string result = "";

        for (int i = 0; i < partCount; i++)
        {
            if (i > 0) result += " ";

            int length = lengthFrequencies.Select();

            string current = startFrequencies.Select();
            result += current;

            for (int j = 1; j < length; j++)
            {
                current = characterFrequencies[current].Select();
                result += current;
            }
        }

        return result;
    }

    private void ProcessCorpus(string[] corpus)
    {
        foreach (var entry in corpus)
        {
            // track frequency of number of parts in name
            var parts = entry.Split(null); // split on whitespace
            partCountsFrequencies.Increment(parts.Length);

            foreach (var name in parts)
            {
                // track frequencies of length of name
                lengthFrequencies.Increment(name.Length);

                // track frequencies of starting character
                startFrequencies.Increment(name.Substring(0, 1));

                // track frequencies of character ordering
                for (int i = 0; i < name.Length - 1; i++)
                {
                    string a = name.Substring(i, 1);
                    string b = name.Substring(i + 1, 1);

                    if (!characterFrequencies.ContainsKey(a))
                    {
                        characterFrequencies[a] = new FrequencyTable<string>();
                    }

                    characterFrequencies[a].Increment(b);
                }
            }
        }
    }

    private class FrequencyTable<T>
    {
        private Dictionary<T, int> frequencies = new Dictionary<T, int>();
        private Dictionary<T, float> weightedFrequencies = new Dictionary<T, float>();
        private float? length = null; // null value indicates table length needs to be updated

        public void Increment(T item)
        {
            if (frequencies.ContainsKey(item))
            {
                frequencies[item]++;
            }
            else
            {
                frequencies[item] = 1;
            }

            weightedFrequencies[item] = Mathf.Pow(frequencies[item], 1.3f);
            length = null;
        }

        public T Select()
        {
            if (length == null)
            {
                length = weightedFrequencies.Values.Sum();
            }

            float selected = Random.value * length.Value;
            float accumulator = 0f;

            foreach (T item in weightedFrequencies.Keys)
            {
                accumulator += weightedFrequencies[item];

                if (accumulator > selected)
                {
                    return item;
                }
            }

            // should be unreachable
            return default(T);
        }
    }
}
