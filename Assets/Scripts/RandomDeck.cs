using System.Collections.Generic;
using UnityEngine;

// like a deck of cards, spits out items in a non-repeating random order and
// reshuffles after all items has occurred
public class RandomDeck<T>
{
    private readonly T[] items;
    private List<T> deck;

    public RandomDeck(T[] items)
    {
        this.items = items;
        this.deck = CreateShuffledList(items);
    }

    public T Draw()
    {
        var item = deck[deck.Count - 1];

        if (deck.Count == 1)
            deck = CreateShuffledList(items);
        else
            deck.RemoveAt(deck.Count - 1);

        return item;
    }

    private List<T> CreateShuffledList(T[] array)
    {
        var copy = new T[array.Length];
        array.CopyTo(copy, 0);

        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T tmp = copy[i];
            copy[i] = copy[j];
            copy[j] = tmp;
        }

        return new List<T>(copy);
    }
}
