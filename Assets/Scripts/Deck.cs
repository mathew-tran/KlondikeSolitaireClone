using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static Card;
using static Deck;
using static UnityEngine.Rendering.GPUSort;

public class Deck : MonoBehaviour
{
    [SerializeField]
    private GameObject CardPrefab;

    public Transform SpawnPosition;

    public CardPile CardHolder;
    public CardPile HandPile;
 

    public Action OnDeckSetupComplete;
   

    public enum DECK_STATE
    {
        NON_INITIALIZED,
        DRAWING,
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
            int seed = (int)DateTime.Now.Ticks;
            Debug.Log("Start Game with Seed: " + seed);

            UnityEngine.Random.InitState(seed);

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
                yield return CardHolder.TakeCard(cardRef);
            }
            DeckState = DECK_STATE.INITIALIZED;
        }
    }

    public IEnumerator DeckClicked()
    {
        if (DeckState == DECK_STATE.INITIALIZED)
        {
            DeckState = DECK_STATE.DRAWING;
            Card TopCard = GetTopCard();
            if (TopCard)
            {
                yield return StartCoroutine(HandPile.TakeCardAndFlip(true, TopCard, MOVE_SPEED.FAST));

            }
            else
            {
                List<Transform> cards = new List<Transform>();
                foreach (Transform child in HandPile.transform)
                {
                    cards.Add(child);
                }
                foreach (Transform child in cards)
                {
                    yield return StartCoroutine(child.GetComponent<Card>().DoUnFlip(MOVE_SPEED.INSTANT));
                }

                 

                List<Transform> shuffledCards = cards.OrderBy(_ => UnityEngine.Random.value).ToList();

                foreach (Transform child in shuffledCards)
                {
                    yield return StartCoroutine(CardHolder.TakeCard(child.GetComponent<Card>(), MOVE_SPEED.SUPERFAST));
                }
                yield return new WaitForSeconds(.2f);
            }
            DeckState = DECK_STATE.INITIALIZED;
        }
    }

    public Card GetTopCard()
    {
        return CardHolder.GetTopCard();
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
