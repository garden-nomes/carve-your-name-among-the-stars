using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Markov Name Generator")]
public class MarkovNameGeneratorScriptableObject : ScriptableObject
{
    [TextArea, Tooltip("List of newline-seperated names to seed Markov chain with")]
    public string corpus = "";

    private MarkovNameGenerator generator = null;

    public string Generate()
    {
        if (corpus.Length == 0) return "";

        if (generator == null)
        {
            string[] names = corpus.Split(
                new [] { "\n", "\r\n", "\r" },
                StringSplitOptions.RemoveEmptyEntries
            );

            generator = new MarkovNameGenerator(names);
        }

        return generator.Generate();
    }

#if UNITY_EDITOR
    [ContextMenu("Test")]
    public void Test()
    {
        for (int i = 0; i < 10; i++)
        {
            Debug.Log(Generate());
        }
    }

    private void OnValidate()
    {
        generator = null;
    }
#endif
}
