using System;
using System.Collections;
using System.Collections.Generic;
using GameInput;
using UI;
using UnityEngine;
using UnityEngine.Assertions;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenuGameObject;
    
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

        var coroutines = new IEnumerator[]
        {
            MainMenuCoroutine(),
            GameStartCoroutine(),
            GameLoopCoroutine()
        };

        StartCoroutine(SessionCoroutine(0, coroutines));
    }

    private void OnDisable()
    {
        MainMenuUI.OnGameStarted -= OnGameStarted;
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

    private IEnumerator GameStartCoroutine()
    {
        //TODO Do intro animation for game start
        //TODO Start GameLoop Coroutine
        //throw new NotImplementedException();

        yield return new WaitForSeconds(2f);
    }

    private IEnumerator GameLoopCoroutine()
    {
        GameInputDelegator.SetInputLock(false);
        var playerDied = false;
        var storeFinished = false;

        void OnPlayerDied() => playerDied = true;
        void OnStoreClosed() => storeFinished = true;

        //------------------------------------------------//

        while (true)
        {
            //TODO Start a Game Day
            //TODO Start the drop rate for the explosives
            //TODO Wait for the player death announcement
            //TODO Display the store
            //TODO Wait for the store to finish
            
            yield return null;
        }
    }

    private IEnumerator WaitForDeathCoroutine()
    {
        var playerDied = false;
        void OnPlayerDied() => playerDied = true;

        yield return new WaitUntil(() => playerDied);
    }

    private IEnumerator WaitForStoreCloseCoroutine()
    {
        var storeFinished = false;
        void OnStoreClosed() => storeFinished = true;
        
        yield return new WaitUntil(() => storeFinished);
    }
    
    
    //Callbacks
    //============================================================================================================//
    
    private void OnGameStarted()
    {
        
    }

    
}
