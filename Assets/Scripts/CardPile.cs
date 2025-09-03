using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using static Card;

public class CardPile : MonoBehaviour
{
    public enum PILE_TYPE
    {
        DECK, // This is the deck where it should build up
        HOLDER, // This is the bottom piles
        FOUNDATION, // This is the foundation where the top card should be seen only
        HAND, //
        PLAYER
    }

    public PILE_TYPE PileType;

    public Action OnCardPlaced;
    public IEnumerator TakeCard(Card card, Card.MOVE_SPEED moveSpeed = Card.MOVE_SPEED.SUPERFAST)
    {
        card.gameObject.transform.SetParent(transform);
        StartCoroutine(card.DoMove(GetComponent<CardPile>().GetLatestPosition(), moveSpeed));
        card.RenderQueueLayer = 3000 + transform.childCount;
        card.AdjustRenderLayer(0);
        card.LastPosition = card.GetCardPile().GetLatestPosition();
        card.gameObject.layer = LayerMask.NameToLayer("Card");
        yield return new WaitUntil(() => card.IsMoving() == false);
        OnCardPlaced?.Invoke();
    }

    public List<Card> GetCardAndSiblings(Card card)
    {
        List<Card> pile = new List<Card>();

        for (int i =  card.transform.GetSiblingIndex(); i < transform.childCount; ++i)
        {
            pile.Add(transform.GetChild(i).GetComponent<Card>());
        }

        return pile;
    }

    public IEnumerator TakeCardAndFlip(bool bFlipped, Card card, Card.MOVE_SPEED moveSpeed = Card.MOVE_SPEED.SUPERFAST)
    {
        yield return StartCoroutine(TakeCard(card, moveSpeed));
        if (bFlipped)
        {
            yield return StartCoroutine(card.DoFlip(MOVE_SPEED.SUPERFAST));
            
        }
        else
        {
            yield return StartCoroutine(card.DoUnFlip(MOVE_SPEED.SUPERFAST));
        }

       
    }

    public bool CanTakeCard(Card card)
    {
        switch(PileType)
        {
            case PILE_TYPE.HOLDER:
                if (GetTopCard() == null)
                {
                    if (card.CardRank == Card.RANK.KING)
                    {
                        return true;
                    }
                    return false;
                }
                return GetTopCard().CanStackCard(card, STACK_TYPE.HOLDER);
            case PILE_TYPE.FOUNDATION:
                if(GetTopCard() == null)
                {
                    if (card.CardRank == Card.RANK.ACE)
                    {
                        return true;
                    }
                    return false;
                }
                return GetTopCard().CanStackCard(card, STACK_TYPE.FOUNDATION);
        }
        return false;
      
    }
    public Vector3 GetLatestPosition()
    {
        var offset = new Vector3(0, .05f, 0f);
        switch(PileType)
        {
            case PILE_TYPE.DECK:
                Vector3 offsetAmount = new Vector3(0f, .01f, .01f);
                return ((transform.childCount - 1) * offsetAmount) + transform.position + offset;
            case PILE_TYPE.HOLDER:
            case PILE_TYPE.PLAYER:
                Vector3 holderOffsetAmount = new Vector3(0f, .2f, -.55f);
                return ((transform.childCount - 1) * holderOffsetAmount) + transform.position + offset;
            case PILE_TYPE.FOUNDATION:
                return transform.position + offset;
            case PILE_TYPE.HAND:
                Vector3 handOffset = new Vector3(0f, .01f, .0f);
                return ((transform.childCount - 1) * handOffset) + transform.position + offset;

        }
        return transform.position + offset;
       
    }

    public bool HasCards()
    {
        return transform.childCount > 0;
    }
    public Card GetTopCard()
    {
        if (transform.childCount <= 0)
        {
            return null;
        }
        return transform.GetChild(transform.childCount - 1).GetComponent<Card>();
    }
    public Card GetBottomCard()
    {
        if (transform.childCount <= 0)
        {
            return null;
        }
        return transform.GetChild(0).GetComponent<Card>();
    }
    public IEnumerator AttemptFlipExposedCard()
    {
        Debug.Log("Attempt to flip exposed card" + gameObject.name);
        if (transform.childCount == 0)
        {
            yield return null;
        }
        else if (GetTopCard().CanFlip()) {
            yield return StartCoroutine(GetTopCard().DoFlip(Card.MOVE_SPEED.FAST));
        }
        
    }
}
