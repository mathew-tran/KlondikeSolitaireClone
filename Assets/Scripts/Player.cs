using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.Rendering;

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
                if (CardFocused == null)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (card.CanDrag())
                        {
                            CardFocused = card;
                            CardFocused.SetTargetability(false);
                            Debug.Log(obj.name);

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
                                Debug.Log("attempt card take");
                            }
                            

                        }
                    }
                   
                        
                }
                
               
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
