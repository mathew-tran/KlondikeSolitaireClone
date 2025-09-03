using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static Card;
using static Deck;

public class Deck : MonoBehaviour
{
    [SerializeField]
    private GameObject CardPrefab;

    public Transform SpawnPosition;

    public GameObject CardHolder;
    public CardPile HandPile;

    public Action OnDeckSetupComplete;

    public enum DECK_STATE
    {
        NON_INITIALIZED,
        INITIALIZED
    }

    public DECK_STATE DeckState;
    private IEnumerator SpawnDeck()
    {
        if (DeckState == DECK_STATE.INITIALIZED)
        {
            Debug.Log("Attempted to spawn deck when it's been initialized");
            yield return null;
        }
        else
        {
            DeckState = DECK_STATE.NON_INITIALIZED;
            List<Tuple<Card.SUIT, Card.RANK>> cardData = new List<Tuple<Card.SUIT, Card.RANK>>();
            foreach (Card.RANK rank in Enum.GetValues(typeof(Card.RANK)))
            {
                foreach (Card.SUIT suit in Enum.GetValues(typeof(Card.SUIT)))
                {
                    cardData.Add(new Tuple<Card.SUIT, Card.RANK>(suit, rank));
                }
            }
            cardData = cardData.OrderBy(_ => UnityEngine.Random.value).ToList();


            foreach (Tuple<Card.SUIT, Card.RANK> tuple in cardData)
            {
                GameObject instance = Instantiate(CardPrefab, SpawnPosition.position, transform.rotation, CardHolder.transform);
                Card cardRef = instance.GetComponent<Card>();
                cardRef.Setup(tuple.Item2, tuple.Item1);
                yield return CardHolder.GetComponent<CardPile>().TakeCard(cardRef);
            }
            DeckState = DECK_STATE.INITIALIZED;
        }
    }

    public void DeckClicked()
    {
        if (DeckState == DECK_STATE.INITIALIZED)
        {
            Card TopCard = GetTopCard();
            if (TopCard)
            {
                StartCoroutine(HandPile.TakeCardAndFlip(true, TopCard));

            }
            else
            {
                foreach(Transform child in HandPile.transform)
                {
                    StartCoroutine(CardHolder.GetComponent<CardPile>().TakeCardAndFlip(false, child.GetComponent<Card>(), MOVE_SPEED.INSTANT));
                }
            }
        }
    }

    public Card GetTopCard()
    {
        return CardHolder.GetComponent<CardPile>().GetTopCard();
    }
    private IEnumerator ShuffleDeck()
    {
        yield return null;
        List<Transform> cards = new List<Transform>();
        foreach(Transform child in CardHolder.transform)
        {
            cards.Add(child);
        }

        List<Transform> shuffledCards = cards.OrderBy(_ => UnityEngine.Random.value).ToList();
        cards.Reverse();

        foreach(Transform card in shuffledCards)
        {
            Card cardRef = card.gameObject.GetComponent<Card>();            
            card.SetParent(null);
            yield return StartCoroutine(cardRef.DoMove(SpawnPosition.position, Card.MOVE_SPEED.INSTANT));
            
        }
        

        foreach (Transform card in shuffledCards)
        {            
            Card cardRef = card.gameObject.GetComponent<Card>();
            yield return CardHolder.GetComponent<CardPile>().TakeCard(cardRef);
        }



    }
    void Start()
    {
        StartCoroutine(SetupDeck());
    }
    private IEnumerator SetupDeck()
    {
        yield return StartCoroutine(SpawnDeck());
        OnDeckSetupComplete.Invoke();
    }

}
