using UnityEngine;

public class CardPile : MonoBehaviour
{
    public Vector3 GetLatestPosition()
    {
        Vector3 offsetAmount = new Vector3(.001f, .01f, 0);
        return (transform.childCount * offsetAmount) + transform.position;
    }

    public void FlipExposedCard()
    {
        if (transform.childCount <= 0)
        {
            Debug.LogError("Attempted to flip a non existant card for pile: " + gameObject.name);
            return;
        }

        StartCoroutine(transform.GetChild(transform.childCount -1).GetComponent<Card>().DoFlip(Card.MOVE_SPEED.MEDIUM));
    }
}
