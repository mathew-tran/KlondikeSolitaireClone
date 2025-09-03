using UnityEngine;

public class Player : MonoBehaviour
{
    public LayerMask HitLayerMask;
    public LayerMask BoardMask;
    public LayerMask IgnoreCardLayer;
    public Card CardFocused;
    // Update is called once per frame

    public bool ProcessDeckHit(GameObject obj)
    {
        Deck deck = obj.GetComponent<Deck>();
        if (deck == null)
        {
            return false;
        }
        if (Input.GetMouseButtonDown(0))
        {
            deck.DeckClicked();
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
        
       
        if (CardFocused == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (card.CanDrag())
                {
                    CardFocused = card;
                    CardFocused.SetTargetability(false);
                    return true;
                }
            }
        }
        else
        {
            Debug.Log("Attempt drag" + obj.name);
            if (Input.GetMouseButton(0) == false)
            {
                if (card.CanDrag())
                {
                    if (card.GetCardPile().CanTakeCard(CardFocused))
                    {
                        var oldCardPile = CardFocused.GetCardPile();
                        StartCoroutine(card.GetCardPile().TakeCard(CardFocused));
                        StartCoroutine(oldCardPile.AttemptFlipExposedCard());
                        ReleaseCard();
                        return true;
                    }


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

        if (CardFocused)
        {
            if (Input.GetMouseButton(0) == false)
            {
                if (cardPile.CanTakeCard(CardFocused))
                {
                    var oldCardPile = CardFocused.GetCardPile();
                    StartCoroutine(cardPile.TakeCard(CardFocused));
                    StartCoroutine(oldCardPile.AttemptFlipExposedCard());
                    ReleaseCard();
                    return true;
                }
            }
        }
        return false;
    }
    void Update()
    {
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
            if (CardFocused != null)
            {
                StartCoroutine(CardFocused.RevertToLastPosition());
                ReleaseCard();
            }
        }
        AttemptDragCard();


    }

    private void ReleaseCard()
    {
        CardFocused.SetTargetability(true);
        CardFocused.AdjustRenderLayer(0);
        CardFocused = null;
    }

    public void AttemptDragCard()
    {
        if (CardFocused == null)
        {
            return;
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, BoardMask))
        {
            
            CardFocused.AdjustRenderLayer(1000);
            CardFocused.gameObject.transform.position = hit.point - ray.direction * 2;
        }
    }
}
