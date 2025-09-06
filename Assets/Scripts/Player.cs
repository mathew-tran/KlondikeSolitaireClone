using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public LayerMask HitLayerMask;
    public LayerMask BoardMask;
    public LayerMask IgnoreCardLayer;
    public CardPile LastPile;

    public List<Card> LookedAtCards = new List<Card>();
    // Update is called once per frame

    public bool bCanPlay = true;

    public Action OnPlayerGainedPile;

    private CardPile PlayerPile;

    private void Awake()
    {
        PlayerPile = GetComponent<CardPile>();
    }
    public CardPile GetPlayerCardPile()
    {
        return PlayerPile;
    }
    public void SetPlayerCanPlay(bool bPlayable)
    {
        bCanPlay = bPlayable;
    }
    public bool ProcessDeckHit(GameObject obj)
    {
        Deck deck = obj.GetComponent<Deck>();
        if (deck == null)
        {
            return false;
        }
        if (PlayerPile.HasCards())
        {
            return false;
        }
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(deck.DeckClicked());
        }
        return true;
    }
    public bool ProcessCardHit(GameObject obj)
    {
        Card card = obj.GetComponent<Card>();
        if ( card == null)
        {
            return false;
        }
        
        
        if (PlayerPile.HasCards() == false)
        {
            if (card.CanDrag())
            {
                CardPile pile = card.GetCardPile();
                foreach (Card foundCards in pile.GetCardAndSiblings(card))
                {
                    LookedAtCards.Add(foundCards);
                    foundCards.ShowHighlight(true);
                }

            }
            if (Input.GetMouseButtonDown(0))
            {
                if (card.CanDrag())
                {
                    LastPile = card.GetCardPile();

                    foreach (Card foundCards in LastPile.GetCardAndSiblings(card))
                    {
                        StartCoroutine(PlayerPile.TakeCard(foundCards));
                    }
                    
                    return true;
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0) == false)
            {
                if (card.CanDrag())
                {
                    
                    while(PlayerPile.GetBottomCard())
                    {
                        if (card.GetCardPile().CanTakeCard(PlayerPile.GetBottomCard()))
                        {
                            StartCoroutine(card.GetCardPile().TakeCard(PlayerPile.GetBottomCard()));
                        }
                        else
                        {
                            return false;
                        }                               
                    }
                        
                    StartCoroutine(LastPile.AttemptFlipExposedCard());
                    return true;

                }
            }

        }

        return false;
    }

    public bool ProcessCardPileHit(GameObject obj)
    {
        CardPile cardPile = obj.GetComponent<CardPile>();
        if (cardPile == null)
        {
            return false;
        }

        if (PlayerPile.HasCards())
        {
            if (Input.GetMouseButton(0) == false)
            {
               
                
                while (PlayerPile.GetBottomCard())
                {
                    if (cardPile.CanTakeCard(PlayerPile.GetBottomCard())) { 
                        StartCoroutine(cardPile.TakeCard(PlayerPile.GetBottomCard()));
                    }
                    else
                    {
                        return false;
                    }

                }

                StartCoroutine(LastPile.AttemptFlipExposedCard());
                return true;
                
            }
        }
        return false;
    }
    void Update()
    {
        if (bCanPlay == false)
        {
            return;
        }
        foreach(Card card in LookedAtCards)
        {
            card.ShowHighlight(false);
        }
        LookedAtCards.Clear();

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, HitLayerMask)) {
            GameObject obj = hit.collider.gameObject;
            if (ProcessDeckHit(obj))
            {
                return;    
            }
            if (ProcessCardHit(obj))
            {
                return;
                
            }
            if (ProcessCardPileHit(obj)) 
            {
                return;
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            foreach (Transform child in transform)
            {
                child.GetComponent<Card>().AdjustRenderLayer(0);
                child.gameObject.layer = LayerMask.NameToLayer("Card");
                while (PlayerPile.GetBottomCard())
                {
                    StartCoroutine(LastPile.TakeCard(PlayerPile.GetBottomCard()));
                }
            }
        }
        AttemptDragCard();


    }

    public void AttemptDragCard()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, BoardMask))
        {

            gameObject.transform.position = hit.point - ray.direction * 2;
        }        
        foreach(Transform child in transform)
        {
            child.GetComponent<Card>().AdjustRenderLayer(1000);
            child.gameObject.layer = LayerMask.NameToLayer("IgnoredCard");
        }
    }
}
