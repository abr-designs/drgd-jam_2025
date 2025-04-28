using System;
using System.Collections;
using Controllers;
using GameInput;
using UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
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

    private enum DEBT_TYPE
    {
        NONE = 0,
        PAID,
        BANKRUPT
    }

    private enum GAME_STATE
    {
        NONE = 0,
        MAIN_MENU,
        GAME,
        STORE,
        DEBT
    }

    [SerializeField, Min(0f), Header("Intro Cinematic")]
    private float introAnimationTime = 2f;
    [SerializeField]
    private WaitForAnimationBase introCinematicAnimator;

    [SerializeField, Min(0f), Header("Game Day Start Cinematic")]
    private float dayStartAnimationTime = 2f;
    [SerializeField]
    private WaitForAnimationBase dayStartCinematicAnimator;

    private DynamiteManager m_dynamiteManager;
    private GameObject[] m_canvases;


    [SerializeField, Min(1), Header("Game Debt Settings")]
    private int startDebt = 100;
    [SerializeField] private int debtInterestStart = 0; // The base interest level per day
    [SerializeField] private int debtInterestIncrement = 1; // The amount the interest will increase
    [SerializeField] private int debtInterestDays = 3; // Amount of time until debt interest increases
    [SerializeField] private int debtRepairCostStart = 1; // Amount of cost per death
    [SerializeField] private int debtRepairCostIncrease = 1; // Amount of increase cost per death


    private int _debtLeft = 0;
    private int _debtInterest = 0;
    private int _currentRepairCost = 0;
    private int _debtDayCounter = 0;

    //============================================================================================================//

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //------------------------------------------------//
        m_dynamiteManager = FindAnyObjectByType<DynamiteManager>();

        var mainMenuUI = FindAnyObjectByType<MainMenuUI>();
        var gameCanvasController = FindAnyObjectByType<GameCanvasController>();
        var upgradeStoreController = FindAnyObjectByType<UpgradeStoreController>();
        var debtUIController = FindAnyObjectByType<DebtCanvasController>();

        m_canvases = new[]
        {
            null, //NONE
            mainMenuUI.gameObject, //MAIN_MENU
            gameCanvasController.gameObject, //GAME
            upgradeStoreController.gameObject, //STORE
            debtUIController.gameObject // DEBT
        };

        Assert.IsNotNull(m_dynamiteManager, "m_dynamiteManager is null!");

        Assert.IsNotNull(mainMenuUI, "mainMenuUI is null!");
        Assert.IsNotNull(gameCanvasController, "gameCanvasController is null!");
        Assert.IsNotNull(upgradeStoreController, "upgradeStoreController is null!");
        Assert.IsNotNull(debtUIController, "debtUIController is null!");

        //------------------------------------------------//

        ShowCanvas(GAME_STATE.MAIN_MENU);

        GameInputDelegator.SetInputLock(true);
        DynamiteManager.StopSpawning();

        var coroutines = new[]
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
        // Reset debt
        _debtLeft = startDebt;
        _debtInterest = debtInterestStart;
        _currentRepairCost = debtRepairCostStart;
        _debtDayCounter = 0;

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
            DynamiteManager.StartSpawning();


            var playerDied = false;
            //Wait for the player death announcement
            yield return StartCoroutine(WaitForDeathCoroutine(
                type =>
                {
                    //TODO Maybe do something specific depending on how the day ended
                    playerDied = type == FINISH_TYPE.DEATH;

                    //Lock the player inputs
                    GameInputDelegator.SetInputLock(true);
                }));

            // Display debt
            _debtDayCounter++;
            _debtInterest = debtInterestStart +  debtInterestIncrement * (_debtDayCounter/debtInterestDays);
            ShowCanvas(GAME_STATE.DEBT);
            DebtCanvasController.ShowDebt(true, _debtLeft, Math.Min(_debtInterest,_debtLeft), playerDied ? _currentRepairCost : 0);

            var gameOver = false;
            yield return StartCoroutine(WaitForDebtCloseCoroutine(
                type => 
                {
                    gameOver = type == DEBT_TYPE.BANKRUPT;
                }
            ));

            if(gameOver) {
                // TODO -- game over - do fade coroutine?
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
            }

            // load the win scene
            if(_debtLeft <= 0) {
                SceneManager.LoadScene("Win");
                break;
            }

            // Increase repair cost (if dead)
            if (playerDied)
            {
                _currentRepairCost += debtRepairCostIncrease;
            }

            //Display the store
            ShowCanvas(GAME_STATE.STORE);
            UpgradeStoreController.ShowStore(true);
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

        while (true)
        {
            if (playerDied)
                break;

            //Wait for all of the dynamite to finish before wrapping
            if (dayFinished && Dynamite.ActiveCount == 0)
            {
                yield return new WaitForSeconds(0.5f);
                break;
            }

            if (dayFinished)
            {
                //Stop the drop rate for explosives
                DynamiteManager.StopSpawning();
            }

            yield return null;
        }

        //Stop the drop rate for explosives
        DynamiteManager.StopSpawning();

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

    private IEnumerator WaitForDebtCloseCoroutine(Action<DEBT_TYPE> onDebtCompleted)
    {
        var debtFinished = false;
        var debtPaid = false;
        void OnDebtClosed(bool paid, int amount) {
            _debtLeft -= amount;
            debtFinished = true;
            debtPaid = paid;
        }

        DebtCanvasController.OnDebtWindowClose += OnDebtClosed;
        yield return new WaitUntil(() => debtFinished);
        DebtCanvasController.OnDebtWindowClose -= OnDebtClosed;

        onDebtCompleted?.Invoke(debtPaid ? DEBT_TYPE.PAID : DEBT_TYPE.BANKRUPT);
    }




}
