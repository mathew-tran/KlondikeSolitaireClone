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

    public Transform mSpawnPosition;

    public GameObject mCardHolder;
    private IEnumerator SpawnDeck()
    {
        foreach(Card.RANK rank in Enum.GetValues(typeof(Card.RANK))){
            foreach (Card.SUIT suit in Enum.GetValues(typeof(Card.SUIT)))
            {
                GameObject instance = Instantiate(CardPrefab, mSpawnPosition.position, transform.rotation, mCardHolder.transform);
                Card cardRef = instance.GetComponent<Card>();
                cardRef.Setup(rank, suit);
                yield return new WaitForSeconds(.01f);
                StartCoroutine(cardRef.DoMove(mCardHolder.GetComponent<CardPile>().GetLatestPosition(), Card.MOVE_SPEED.FAST));
                yield return new WaitUntil(() => cardRef.IsMoving() == false);
            }
        }
    }

    private IEnumerator ShuffleDeck()
    {
        yield return null;

    }
    void Start()
    {
        StartCoroutine(SpawnDeck());
    }

}
