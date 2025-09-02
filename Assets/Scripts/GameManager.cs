using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Deck DeckReference;

    [SerializeField]
    private Transform CardHolderSlots;

    private void Start()
    {
        DeckReference.OnDeckSetupComplete += OnDeckSetupComplete;
    }

    private void OnDeckSetupComplete()
    {
        StartCoroutine(FillSlots());
        
    }
    private IEnumerator FillSlots()
    {
        List<CardPile> slots = new List<CardPile>();

        foreach (CardPile pile in CardHolderSlots.GetComponentsInChildren<CardPile>())
        {
            slots.Add(pile);
        }

        List<CardPile> allSlots = new List<CardPile>(slots);

        for (int i = 0; i < 7; ++i)
        {
            foreach (CardPile pile in slots)
            {
                yield return StartCoroutine(pile.TakeCard(DeckReference.GetTopCard()));
            }
            slots.RemoveAt(0);
        }

        foreach(CardPile pile in allSlots)
        {
            yield return StartCoroutine(pile.FlipExposedCard());
        }
    }
       
}
