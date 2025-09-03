using UnityEngine;

public class Player : MonoBehaviour
{
    public LayerMask HitLayerMask;
    public LayerMask BoardMask;
    public LayerMask IgnoreCardLayer;
    public Card CardFocused;
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100, HitLayerMask)) {
            GameObject obj = hit.collider.gameObject;
            Card card = obj.GetComponent<Card>();
            
            if (card != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (card.CanDrag())
                    {
                        CardFocused = card;

                    }                    

                }
                Debug.Log(obj.name);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (CardFocused != null)
            {
                CardFocused.AdjustRenderLayer(0);
                StartCoroutine(CardFocused.GetCardPile().TakeCard(CardFocused));
            }
            CardFocused = null;
        }

        AttemptDragCard();
        
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
