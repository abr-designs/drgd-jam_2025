using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using GameInput;
using UI;
using UnityEngine;
using UnityEngine.Assertions;
using Utilities;
using Utilities.Animations;
using Utilities.WaitForAnimations;
using Utilities.WaitForAnimations.Base;

[RequireComponent(typeof(DayController))]
public class GameController : MonoBehaviour
{
    private enum FINISH_TYPE
    {
        NONE = 0,
        DEATH,
        DAY_END,
    }

    private enum GAME_STATE
    {
        NONE = 0,
        MAIN_MENU,
        GAME,
        STORE
    }

    //[SerializeField, Obsolete]
    //private DynamiteManager DynamiteManager;
    
    //[SerializeField]
    //private GameObject mainMenuGameObject;

    [SerializeField, Min(0f), Header("Intro Cinematic")]
    private float introAnimationTime = 2f;
    [SerializeField]
    private WaitForAnimationBase introCinematicAnimator;
    
    [SerializeField, Min(0f), Header("Game Day Start Cinematic")]
    private float dayStartAnimationTime = 2f;
    [SerializeField]
    private WaitForAnimationBase dayStartCinematicAnimator;

    private DynamiteManager m_dynamiteManager;
    
    //private MainMenuUI m_mainMenuUI;
    //private GameCanvasController m_gameCanvasController;
    //private UpgradeStoreController m_upgradeStoreController;

    private GameObject[] m_canvases;
    
    //============================================================================================================//
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //------------------------------------------------//
        m_dynamiteManager = FindAnyObjectByType<DynamiteManager>();
        
        var mainMenuUI = FindAnyObjectByType<MainMenuUI>();
        var gameCanvasController = FindAnyObjectByType<GameCanvasController>();
        var upgradeStoreController = FindAnyObjectByType<UpgradeStoreController>();

        m_canvases = new[]
        {
            null, //NONE
            mainMenuUI.gameObject, //MAIN_MENU
            gameCanvasController.gameObject, //GAME
            upgradeStoreController.gameObject //STORE
        };
        
        Assert.IsNotNull(m_dynamiteManager, "m_dynamiteManager is null!");
        
        Assert.IsNotNull(mainMenuUI, "mainMenuUI is null!");
        Assert.IsNotNull(gameCanvasController, "gameCanvasController is null!");
        Assert.IsNotNull(upgradeStoreController, "upgradeStoreController is null!");
        
        //------------------------------------------------//
        
        ShowCanvas(GAME_STATE.MAIN_MENU);
        
        GameInputDelegator.SetInputLock(true);
        m_dynamiteManager.enabled = false;

        var coroutines = new []
        {
            MainMenuCoroutine(),
            GameStartingCoroutine(),
            GameLoopCoroutine()
        };

        StartCoroutine(SessionCoroutine(0, coroutines));
    }

    //Show Canvas
    //============================================================================================================//

    private void ShowCanvas(GAME_STATE gameState)
    {
        var gameStateIndex = (int)gameState;

        for (int i = 1; i < m_canvases.Length; i++)
        {
            m_canvases[i].SetActive(i == gameStateIndex);
        }
    }

    //Coroutines
    //============================================================================================================//

    private IEnumerator SessionCoroutine(int startIndex, IEnumerator[] coroutines)
    {
        startIndex = Math.Clamp(startIndex, 0, coroutines.Length - 1);
        
        for (int i = startIndex; i < coroutines.Length; i++)
        {
            yield return StartCoroutine(coroutines[i]);
        }
    }
    
    //============================================================================================================//

    private IEnumerator MainMenuCoroutine()
    {
        bool startPressed = false;
        void OnGameStarted()
        {
            startPressed = true;
        }

        //------------------------------------------------//
        
        MainMenuUI.OnGameStarted += OnGameStarted;

        yield return new WaitUntil(() => startPressed);
        
        MainMenuUI.OnGameStarted -= OnGameStarted;
    }

    private IEnumerator GameStartingCoroutine()
    {
        if (introCinematicAnimator)
            yield return introCinematicAnimator.DoAnimation(introAnimationTime, ANIM_DIR.START_TO_END);
        else
            yield return new WaitForSeconds(introAnimationTime);
    }

    private IEnumerator GameLoopCoroutine()
    {
        while (true)
        {
            ShowCanvas(GAME_STATE.GAME);
            DayController.IncrementDay();
            //Start a Game Day
            if (dayStartCinematicAnimator)
                yield return dayStartCinematicAnimator.DoAnimation(dayStartAnimationTime, ANIM_DIR.START_TO_END);
            else
                yield return new WaitForSeconds(dayStartAnimationTime);
            
            //Start day timer
            DayController.StartGameDay();
            GameInputDelegator.SetInputLock(false);
            
            //Start the drop rate for explosives
            m_dynamiteManager.enabled = true;

            //Wait for the player death announcement
            yield return StartCoroutine(WaitForDeathCoroutine(
                type =>
                {
                    //TODO Maybe do something specific depending on how the day ended

                    //Lock the player inputs
                    GameInputDelegator.SetInputLock(true);
            
                    //Stop the drop rate for explosives
                    m_dynamiteManager.enabled = false;
                }));
            
            //Display the store
            ShowCanvas(GAME_STATE.STORE);
            //Wait for the store to finish
            yield return StartCoroutine(WaitForStoreCloseCoroutine());
        }
    }


    private IEnumerator WaitForDeathCoroutine(Action<FINISH_TYPE> onDayCompleted)
    {
        var playerDied = false;
        void OnPlayerDied() => playerDied = true;
        
        var dayFinished = false;
        void OnDayFinished() => dayFinished = true;
        
        PlayerHealth.OnPlayerDied += OnPlayerDied;
        DayController.OnDayFinished += OnDayFinished;

        yield return new WaitUntil(() => playerDied || dayFinished);
        
        PlayerHealth.OnPlayerDied -= OnPlayerDied;
        DayController.OnDayFinished -= OnDayFinished;
        
        onDayCompleted?.Invoke(playerDied ? FINISH_TYPE.DEATH : FINISH_TYPE.DAY_END);
    }

    private IEnumerator WaitForStoreCloseCoroutine()
    {
        var storeFinished = false;
        void OnStoreClosed() => storeFinished = true;
        
        UpgradeStoreController.OnStoreClosed += OnStoreClosed;
        yield return new WaitUntil(() => storeFinished);
        
        UpgradeStoreController.OnStoreClosed -= OnStoreClosed;
    }
    
}
