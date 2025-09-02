using UnityEngine;

public class CardPile : MonoBehaviour
{
    public Vector3 GetLatestPosition()
    {
        Vector3 offsetAmount = new Vector3(.001f, .01f, 0);
        return (transform.childCount * offsetAmount) + transform.position;
    }
}
