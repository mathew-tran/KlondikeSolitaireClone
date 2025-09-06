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


    private int Seed;


    public int GetSeed()
    {
        return Seed;
    }

    public void SetSeed(int newSeed)
    {
        Seed = newSeed;
    }

    public enum GAME_STATE
    {
        IN_GAME,
        WIN
    }

    public enum SEED_TYPE
    {
        REUSE,
        NEW
    }

    public GAME_STATE CurrentState = GAME_STATE.IN_GAME;

    public Action OnGameOver;

    private static GameManager Instance;
    public static GameManager GetInstance()
    {
        return Instance;
    }
    private void Awake()
    {
        Instance = this;
        DeckReference.OnDeckSetupComplete += OnDeckSetupComplete;

        OnGameOver += DoGameOver;
        OnGameOver += PauseMenuReference.OnWin;
        CardPiles = GameObject.FindObjectsByType(typeof(CardPile), FindObjectsSortMode.None) as CardPile[];
        foreach (CardPile pile in CardPiles)
        {
            pile.OnCardPlaced += CheckGameOver;
        }

        var newSeed = PlayerPrefs.GetInt("Seed", -1);
        if (newSeed == -1)
        {
            Seed = (int)DateTime.Now.Ticks;
        }
        else
        {
            Seed = newSeed;
        }


        UnityEngine.Random.InitState(Seed);
    }



    public void Restart(SEED_TYPE seed = SEED_TYPE.NEW)
    {

        Time.timeScale = 1.0f;
        if (seed == SEED_TYPE.NEW)
        {
            PlayerPrefs.SetInt("Seed", -1);
        }
        else
        {
            PlayerPrefs.SetInt("Seed", Seed);
        }

        PlayerPrefs.Save();

         SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        
    }

    public void RestartWithNewSeed()
    {
        Restart(SEED_TYPE.NEW);
    }

    public void RestartWithExistingSeed()
    {
        Restart(SEED_TYPE.REUSE);
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

    public void CheckForPlayerHints()
    {
        if (Settings.GetInstance().GetShowHints() == false)
        {
            return;
        }
        CardPile cardPile = PlayerReference.GetPlayerCardPile();
        Card card = null;
        if (cardPile.HasCards())
        {
             card = cardPile.GetBottomCard();
        }

        foreach (CardPile pile in CardPiles)
        {            
            pile.AttemptToShowHint(card);            
        }
    }
    public void CheckGameOver()
    {
        CheckForPlayerHints();
        if (PlayerReference.GetPlayerCardPile().HasCards() == false)
        {
            DeckReference.DeckClickIfEmpty();
        }
        
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
        PlayerPrefs.SetInt("GamesWon", PlayerPrefs.GetInt("GamesWon", 0) + 1);
        PlayerPrefs.Save();
        Debug.Log("Game Over");
        OnGameOver?.Invoke();
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.P) || Input.GetKeyUp(KeyCode.Tab))
        {
            PauseMenuReference.Toggle();
        }
        if (PauseMenuReference.gameObject.activeSelf)
        {
            if (Input.GetKeyUp(KeyCode.R) && CurrentState != GAME_STATE.WIN)
            {
                RestartWithExistingSeed();
            }
            if (Input.GetKeyUp(KeyCode.N))
            {
                RestartWithNewSeed();
            }
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
