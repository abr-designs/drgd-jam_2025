using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using GameInput;
using UI;
using UnityEngine;
using UnityEngine.Assertions;
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
    
    [SerializeField]
    private GameObject mainMenuGameObject;

    [SerializeField, Min(0f), Header("Intro Cinematic")]
    private float introAnimationTime = 2f;
    [SerializeField]
    private WaitForAnimationBase introCinematicAnimator;
    
    [SerializeField, Min(0f), Header("Game Day Start Cinematic")]
    private float dayStartAnimationTime = 2f;
    [SerializeField]
    private WaitForAnimationBase dayStartCinematicAnimator;
    
    //============================================================================================================//
    private void OnEnable()
    {
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        Assert.IsNotNull(mainMenuGameObject);
        mainMenuGameObject.SetActive(true);
        
        GameInputDelegator.SetInputLock(true);

        var coroutines = new []
        {
            MainMenuCoroutine(),
            GameStartingCoroutine(),
            GameLoopCoroutine()
        };

        StartCoroutine(SessionCoroutine(0, coroutines));
    }

    private void OnDisable()
    {
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
        
        //------------------------------------------------//

        while (true)
        {
            //Start a Game Day
            if (dayStartCinematicAnimator)
                yield return dayStartCinematicAnimator.DoAnimation(dayStartAnimationTime, ANIM_DIR.START_TO_END);
            else
                yield return new WaitForSeconds(dayStartAnimationTime);
            
            //Start day timer
            DayController.StartGameDay();
            GameInputDelegator.SetInputLock(false);
            
            //TODO Start the drop rate for the explosives
            
            
            //Wait for the player death announcement
            yield return StartCoroutine(WaitForDeathCoroutine(
                type =>
                {
                    //TODO Maybe do something specific depending on how the day ended
                }));
            
            //TODO Display the store
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
        
        UpgradeStore.OnStoreClosed += OnStoreClosed;
        yield return new WaitUntil(() => storeFinished);
        
        UpgradeStore.OnStoreClosed -= OnStoreClosed;
    }
    
}

public class PlayerHealth
{
    public static event Action OnPlayerDied;
}
