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

    public GameObject Hint;
    public Transform HeldCards;

    public string GetName()
    {
        return transform.parent.name  + "->" + name + PileType.ToString();
    }
    public IEnumerator TakeCard(Card card, Card.MOVE_SPEED moveSpeed = Card.MOVE_SPEED.SUPERFAST)
    {
        
        card.gameObject.transform.SetParent(HeldCards);
        card.CardPileRef = this;
        StartCoroutine(card.DoMove(GetLatestPosition(), moveSpeed));
        card.RenderQueueLayer = 3000 + HeldCards.childCount;
        card.AdjustRenderLayer(0);
        card.LastPosition = card.GetCardPile().GetLatestPosition();
        card.gameObject.layer = LayerMask.NameToLayer("Card");
        yield return new WaitUntil(() => card.IsMoving() == false);
        OnCardPlaced?.Invoke();
        if (CanShowHints())
        {
            Debug.Log(GetName());
            for (int i = 0; i < HeldCards.childCount; ++i)
            {
                HeldCards.GetChild(i).GetComponent<Card>().ShowHint(false);
            }

            if (Hint != null)
            {
                Hint.gameObject.SetActive(false);
            }
        }
        



    }

    public List<Card> GetCardAndSiblings(Card card)
    {
        List<Card> pile = new List<Card>();

        if (card == null)
        {
            return pile;
        }
        if (PileType == PILE_TYPE.FOUNDATION)
        {
            pile.Add(GetTopCard());
        }
        else if (PileType == PILE_TYPE.HOLDER)
        {
            for (int i = card.transform.GetSiblingIndex(); i < HeldCards.childCount; ++i)
            {
                pile.Add(HeldCards.GetChild(i).GetComponent<Card>());
            }

        }
        else
        {
            pile.Add(card);
        }


        return pile;
    }

    public IEnumerator TakeCardAndFlip(bool bFlipped, Card card, Card.MOVE_SPEED moveSpeed = Card.MOVE_SPEED.SUPERFAST)
    {


        yield return StartCoroutine(TakeCard(card, moveSpeed));

        if (bFlipped)
        {
            yield return StartCoroutine(card.DoFlip(moveSpeed));

        }
        else
        {
            yield return StartCoroutine(card.DoUnFlip(moveSpeed));
        }



    }

    public bool CanShowHints()
    {
        return PileType == PILE_TYPE.HOLDER || PileType == PILE_TYPE.FOUNDATION;
    }
    public void AttemptToShowHint(Card card)
    {
        if (CanShowHints() == false)
        {
            return;
        }
        if (card == null)
        {
            if (GetTopCard() == null)
            {
                Hint.gameObject.SetActive(false);
            }
            else
            {
                GetTopCard().ShowHint(false);
            }
               
            return;
        }

        if (GetTopCard() == null)
        {
            Hint.gameObject.SetActive(CanTakeCard(card));
        }
        else
        {
            GetTopCard().ShowHint(CanTakeCard(card));
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
            case PILE_TYPE.FOUNDATION:
            case PILE_TYPE.DECK:
                Vector3 offsetAmount = new Vector3(0f, .01f, .01f);
                return ((HeldCards.childCount - 1) * offsetAmount) + transform.position + offset;
            case PILE_TYPE.HOLDER:
            case PILE_TYPE.PLAYER:
                Vector3 holderOffsetAmount = new Vector3(0f, .2f, -.55f);
                return ((HeldCards.childCount - 1) * holderOffsetAmount) + transform.position + offset;
           
            case PILE_TYPE.HAND:
                Vector3 handOffset = new Vector3(0f, .01f, .0f);
                return ((HeldCards.childCount - 1) * handOffset) + transform.position + offset;

        }
        return transform.position + offset;
       
    }

    public bool HasCards()
    {
        return HeldCards.childCount > 0;
    }
    public Card GetTopCard()
    {
        if (HeldCards.childCount <= 0)
        {
            return null;
        }
        return HeldCards.GetChild(HeldCards.childCount - 1).GetComponent<Card>();
    }
    public Card GetBottomCard()
    {
        if (HeldCards.childCount <= 0)
        {
            return null;
        }
        return HeldCards.GetChild(0).GetComponent<Card>();
    }
    public IEnumerator AttemptFlipExposedCard()
    {
        //Debug.Log("Attempt to flip exposed card" + gameObject.name);
        if (HeldCards.childCount == 0)
        {
            yield return null;
        }
        else if (GetTopCard().CanFlip()) {
            yield return StartCoroutine(GetTopCard().DoFlip(Card.MOVE_SPEED.FAST));
        }
        
    }
}
