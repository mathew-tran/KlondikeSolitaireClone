using System.Collections;
using UnityEngine;
using static Card;

public class CardPile : MonoBehaviour
{
    public enum PILE_TYPE
    {
        DECK, // This is the deck where it should build up
        HOLDER, // This is the bottom piles
        FOUNDATION // This is the foundation where the top card should be seen only
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

    public bool CanTakeCard(Card card)
    {
        if (GetTopCard() == null)
        {
            if (card.CardRank == Card.RANK.KING)
            {
                return true;
            }
            return false;
        }
        return GetTopCard().CanStackCard(card);
    }
    public Vector3 GetLatestPosition()
    {
        switch(PileType)
        {
            case PILE_TYPE.DECK:
                Vector3 offsetAmount = new Vector3(.001f, .01f, 0);
                return ((transform.childCount - 1) * offsetAmount) + transform.position;
            case PILE_TYPE.HOLDER:
                Vector3 holderOffsetAmount = new Vector3(0f, .2f, -.55f);
                return ((transform.childCount - 1) * holderOffsetAmount) + transform.position;
            case PILE_TYPE.FOUNDATION:
                return transform.position;

        }
        return transform.position;
       
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
