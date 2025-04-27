using Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

public class GameCanvasController : HiddenSingleton<GameCanvasController>
{
    [SerializeField]
    private TMP_Text dayText;
    [SerializeField]
    private TMP_Text timeText;
    [SerializeField]
    private TMP_Text alertText;

    [SerializeField, Header("Health")]
    private Image[] healthStack;
    [SerializeField]
    private Sprite hasHealthSprite;
    [SerializeField]
    private Sprite noHealthSprite;
    
    //Unity Functions
    //============================================================================================================//

    private void OnEnable()
    {
        PlayerHealth.OnPlayerHealthChange += OnPlayerHealthChange;
        DayController.OnTimeLeftChanged += OnTimeLeftChanged;
        DayController.OnDayChanged += OnDayChanged;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        dayText.text = "";
        timeText.text = "";
        alertText.text = "";
    }
    
    private void OnDisable()
    {
        PlayerHealth.OnPlayerHealthChange -= OnPlayerHealthChange;
        DayController.OnTimeLeftChanged -= OnTimeLeftChanged;
        DayController.OnDayChanged -= OnDayChanged;
    }
    
    //Callbacks
    //============================================================================================================//

    private void OnTimeLeftChanged(float seconds)
    {
        timeText.text = $"{seconds:#0.0}s";
    }
    
    private void OnDayChanged(int dayValue)
    {
        dayText.text = $"Day {dayValue}";
    }
    
    private void OnPlayerHealthChange(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < healthStack.Length; i++)
        {
            healthStack[i].gameObject.SetActive(maxHealth >= i);
            healthStack[i].sprite = currentHealth >= i ? hasHealthSprite : noHealthSprite;
        }
    }
    
}
