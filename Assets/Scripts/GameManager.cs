using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Deck DeckReference;

    [SerializeField]
    private Transform CardHolderSlots;

    private void Start()
    {
        DeckReference.OnDeckSetupComplete += OnDeckSetupComplete;
    }

    private void OnDeckSetupComplete()
    {

    }
}
