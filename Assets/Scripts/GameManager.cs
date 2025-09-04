using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Deck DeckReference;

    [SerializeField]
    private Transform CardHolderSlots;

    public CardPile[] CardPiles;

    public Player PlayerReference;

    public PauseMenu PauseMenuReference;

    public enum GAME_STATE
    {
        IN_GAME,
        WIN
    }

    public GAME_STATE CurrentState = GAME_STATE.IN_GAME;

    public Action OnGameOver;

    public void Restart()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void Awake()
    {
        DeckReference.OnDeckSetupComplete += OnDeckSetupComplete;

        OnGameOver += DoGameOver;
        OnGameOver += PauseMenuReference.OnWin;
        CardPiles = GameObject.FindObjectsByType(typeof(CardPile), FindObjectsSortMode.None) as CardPile[];
        foreach (CardPile pile in CardPiles)
        {
            pile.OnCardPlaced += CheckGameOver;
        }
    }

    private void OnDestroy()
    {
        DeckReference.OnDeckSetupComplete -= OnDeckSetupComplete;
        OnGameOver -= DoGameOver;
        OnGameOver -= PauseMenuReference.OnWin;
        foreach (CardPile pile in CardPiles)
        {
            pile.OnCardPlaced -= CheckGameOver;    
        }
    }

    public void DoGameOver()
    {
        PlayerReference.SetPlayerCanPlay(false);
    }
    public void CheckGameOver()
    {
        if (DeckReference.DeckState == Deck.DECK_STATE.NON_INITIALIZED)
        {
            return;
        }
        if (CurrentState != GAME_STATE.IN_GAME)
        {
            return;
        }
        foreach (CardPile pile in CardPiles)
        {
            if (pile.PileType != CardPile.PILE_TYPE.FOUNDATION)
            {
                {
                    if (pile.HasCards())
                    {
                        return;
                    }
                }
            }
        }
        CurrentState = GameManager.GAME_STATE.WIN;
        Debug.Log("Game Over");
        OnGameOver?.Invoke();
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.P))
        {
            PauseMenuReference.Toggle();
        }
    }
    private void OnDeckSetupComplete()
    {
        StartCoroutine(FillSlots());
        
    }
    private IEnumerator FillSlots()
    {
        List<CardPile> slots = new List<CardPile>();

        foreach (CardPile pile in CardHolderSlots.GetComponentsInChildren<CardPile>())
        {
            slots.Add(pile);
        }

        List<CardPile> allSlots = new List<CardPile>(slots);

        for (int i = 0; i < 7; ++i)
        {
            foreach (CardPile pile in slots)
            {
                yield return StartCoroutine(pile.TakeCard(DeckReference.GetTopCard()));
            }
            slots.RemoveAt(0);
        }

        foreach(CardPile pile in allSlots)
        {
            yield return StartCoroutine(pile.AttemptFlipExposedCard());
        }
    }
       
}
