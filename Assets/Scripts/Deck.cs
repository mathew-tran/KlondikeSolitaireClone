using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    private GameObject CardPrefab;

    private List<GameObject> Cards = new List<GameObject>();
    private void SpawnDeck()
    {
        Vector3 offsetAmount = new Vector3(.01f, .01f, 0);
        int cardNumber = 0;
        foreach(Card.RANK rank in Enum.GetValues(typeof(Card.RANK))){
            foreach (Card.SUIT suit in Enum.GetValues(typeof(Card.SUIT)))
            {
                GameObject instance = Instantiate(CardPrefab, transform.position + offsetAmount * cardNumber, transform.rotation);
                instance.GetComponent<Card>().Setup(rank, suit);
                Cards.Add(instance);
                cardNumber += 1;
            }
        }
    }
    void Start()
    {
        SpawnDeck();
    }

}
