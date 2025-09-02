using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

public class Deck : MonoBehaviour
{
    [SerializeField]
    private GameObject CardPrefab;

    public Transform SpawnPosition;

    public GameObject CardHolder;

    public Action OnDeckSetupComplete;
    private IEnumerator SpawnDeck()
    {
        foreach(Card.RANK rank in Enum.GetValues(typeof(Card.RANK))){
            foreach (Card.SUIT suit in Enum.GetValues(typeof(Card.SUIT)))
            {
                GameObject instance = Instantiate(CardPrefab, SpawnPosition.position, transform.rotation, CardHolder.transform);
                Card cardRef = instance.GetComponent<Card>();
                cardRef.Setup(rank, suit);
                yield return new WaitForSeconds(.01f);
                StartCoroutine(cardRef.DoMove(CardHolder.GetComponent<CardPile>().GetLatestPosition(), Card.MOVE_SPEED.SUPERFAST));
                yield return new WaitUntil(() => cardRef.IsMoving() == false);
            }
        }
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
        foreach(Transform card in cards)
        {
            Card cardRef = card.gameObject.GetComponent<Card>();
            card.SetParent(null);
            StartCoroutine(cardRef.DoMove(SpawnPosition.position, Card.MOVE_SPEED.SUPERFAST));
            yield return new WaitUntil(() => cardRef.IsMoving() == false);
        }

        foreach (Transform card in shuffledCards)
        {
            Card cardRef = card.gameObject.GetComponent<Card>();
            card.SetParent(CardHolder.transform);
            StartCoroutine(cardRef.DoMove(CardHolder.GetComponent<CardPile>().GetLatestPosition(), Card.MOVE_SPEED.SUPERFAST));
            yield return new WaitUntil(() => cardRef.IsMoving() == false);
        }



    }
    void Start()
    {
        StartCoroutine(SetupDeck());
    }
    private IEnumerator SetupDeck()
    {
        yield return StartCoroutine(SpawnDeck());
        yield return StartCoroutine(ShuffleDeck());
        CardHolder.GetComponent<CardPile>().FlipExposedCard();
    }

}
