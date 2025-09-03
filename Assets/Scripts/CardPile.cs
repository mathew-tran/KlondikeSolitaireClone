using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
using static Card;

public class CardPile : MonoBehaviour
{
    public enum PILE_TYPE
    {
        DECK, // This is the deck where it should build up
        HOLDER, // This is the bottom piles
        FOUNDATION, // This is the foundation where the top card should be seen only
        HAND //
    }

    public PILE_TYPE PileType;
    public IEnumerator TakeCard(Card card, Card.MOVE_SPEED moveSpeed = Card.MOVE_SPEED.SUPERFAST)
    {
        card.gameObject.transform.SetParent(transform);
        StartCoroutine(card.DoMove(GetComponent<CardPile>().GetLatestPosition(), moveSpeed));
        card.RenderQueueLayer = 3000 + transform.childCount;
        card.gameObject.GetComponent<Renderer>().material.renderQueue = card.RenderQueueLayer;
        card.LastPosition = card.GetCardPile().GetLatestPosition();
        yield return new WaitUntil(() => card.IsMoving() == false);
    }

    public IEnumerator TakeCardAndFlip(bool bFlipped, Card card, Card.MOVE_SPEED moveSpeed = Card.MOVE_SPEED.SUPERFAST)
    {
        if (bFlipped)
        {
            yield return StartCoroutine(card.DoFlip(MOVE_SPEED.SUPERFAST));
            
        }
        else
        {
            yield return StartCoroutine(card.DoUnFlip(MOVE_SPEED.SUPERFAST));
        }

        yield return StartCoroutine(TakeCard(card, moveSpeed));
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
                Vector3 offsetAmount = new Vector3(.01f, .01f, .01f);
                return ((transform.childCount - 1) * offsetAmount) + transform.position + offset;
            case PILE_TYPE.HOLDER:
                Vector3 holderOffsetAmount = new Vector3(0f, .2f, -.55f);
                return ((transform.childCount - 1) * holderOffsetAmount) + transform.position + offset;
            case PILE_TYPE.FOUNDATION:
                return transform.position + offset;
            case PILE_TYPE.HAND:
                return transform.position + offset;

        }
        return transform.position + offset;
       
    }


    public Card GetTopCard()
    {
        if (transform.childCount <= 0)
        {
            Debug.LogError("Attempted to flip a non existant card for pile: " + gameObject.name);
            return null;
        }
        return transform.GetChild(transform.childCount - 1).GetComponent<Card>();
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
