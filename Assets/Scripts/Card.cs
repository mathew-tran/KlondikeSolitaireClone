using NUnit.Framework.Internal;
using System;
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

    public Action OnFinishedMoving;
    private void SetCardImage()
    {
        string cardName = "Cards/";
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
        cardName += "white";
        Debug.Log(cardName);
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

    public void MoveCard(Vector3 target)
    {

    }

    public void Setup(RANK rank, SUIT suit)
    {
        CardRank = rank;
        CardSuit = suit;
        SetCardImage();
    }
}
