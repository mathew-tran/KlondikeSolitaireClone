using NUnit.Framework.Internal;
using System;
using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    public enum RANK
    {
        ACE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        TEN,
        JACK,
        QUEEN,
        KING
    }
    public enum SUIT
    {
        DIAMOND,
        CLUB,
        SPADE,
        HEARTS
    }

    public SUIT CardSuit;
    public RANK CardRank;
    public enum MOVE_SPEED
    {
        SLOW,
        MEDIUM,
        FAST,
        SUPERFAST
    }

    public float GetSpeed(MOVE_SPEED speedType)
    {
        switch(speedType)
        {
            case MOVE_SPEED.SLOW:
                return 1.5f;
            case MOVE_SPEED.MEDIUM:
                return .25f;
            case MOVE_SPEED.FAST:
                return .1f;
            case MOVE_SPEED.SUPERFAST:
                return .01f;

        }
        return .01f;
    }
    [SerializeField]
    private bool bIsMoving = false;

    public enum FLIP_STATE
    {
        NOT_FLIPPED,
        FLIPPING,
        FLIPPED
    }

    [SerializeField]
    private FLIP_STATE FlipState = FLIP_STATE.NOT_FLIPPED;

    private void SetCardImage()
    {
        string cardName = "Cards/";
        cardName += GetCardName();

        cardName += "white";
        Texture2D image = Resources.Load<Texture2D>(cardName);

        if (image)
        {
            GetComponent<Renderer>().material.SetTexture("_CardFrontImage", image);
        }
        else
        {
            Debug.LogError("Could not find image: " + cardName);
        }
        

    }

    private string GetCardName()
    {
        string cardName = "";
        switch (CardSuit)
        {
            case SUIT.DIAMOND:
                cardName += "Tiles_";
                break;
            case SUIT.CLUB:
                cardName += "Clovers_";
                break;
            case SUIT.HEARTS:
                cardName += "Hearts_";
                break;
            case SUIT.SPADE:
                cardName += "Pikes_";
                break;
        }

        switch (CardRank)
        {
            case RANK.ACE:
                cardName += "A_";
                break;
            case RANK.TWO:
                cardName += "2_";
                break;
            case RANK.THREE:
                cardName += "3_";
                break;
            case RANK.FOUR:
                cardName += "4_";
                break;
            case RANK.FIVE:
                cardName += "5_";
                break;
            case RANK.SIX:
                cardName += "6_";
                break;
            case RANK.SEVEN:
                cardName += "7_";
                break;
            case RANK.EIGHT:
                cardName += "8_";
                break;
            case RANK.NINE:
                cardName += "9_";
                break;
            case RANK.TEN:
                cardName += "10_";
                break;
            case RANK.JACK:
                cardName += "Jack_";
                break;
            case RANK.QUEEN:
                cardName += "Queen_";
                break;
            case RANK.KING:
                cardName += "King_";
                break;
        }
        return cardName;
    }

    public IEnumerator DoMove(Vector3 target, MOVE_SPEED speedType)
    {
        if (bIsMoving)
        {
            Debug.LogError("Attempted to move card: " + gameObject.name + ", but it was already moving");
        }
        else
        {
            bIsMoving = true;


            float progress = 0.0f;
            float timeToMove = GetSpeed(speedType);
            Vector3 startPosition = transform.position;
            while (timeToMove > progress)
            {
                progress += Time.deltaTime;
                float weight = (progress / timeToMove);
                transform.position = Vector3.Lerp(startPosition, target, weight);
                yield return null;
            }

            bIsMoving = false;
        }

    }

    public IEnumerator DoFlip(MOVE_SPEED speedType)
    {
        if (FlipState != FLIP_STATE.NOT_FLIPPED)
        {
            Debug.LogError("Attempted to flip card: " + gameObject.name + ", but it was already flipped");
        }
        else
        {
            FlipState = FLIP_STATE.FLIPPING;
            yield return StartCoroutine(DoMove(transform.position + new Vector3(0, .5f, 0), MOVE_SPEED.MEDIUM));
            yield return new WaitForSeconds(.1f);


            float progress = 0.0f;
            float timeToMove = GetSpeed(speedType);
            Quaternion startRotation = transform.rotation;
            Quaternion targetRotation = startRotation * Quaternion.Euler(180, 0, 0);
            while (timeToMove > progress)
            {
                progress += Time.deltaTime;
                float weight = (progress / timeToMove);
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, weight);
                yield return null;
            }

            FlipState = FLIP_STATE.FLIPPED;
            yield return StartCoroutine(DoMove(transform.position - new Vector3(0, .5f, 0), MOVE_SPEED.SUPERFAST));

        }

    }

    public bool IsMoving()
    {
        return bIsMoving;
    }

    public void Setup(RANK rank, SUIT suit)
    {
        CardRank = rank;
        CardSuit = suit;
        SetCardImage();
        gameObject.name = GetCardName();
    }
}
