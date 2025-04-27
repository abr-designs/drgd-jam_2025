using Controllers;
using TMPro;
using UnityEngine;
using Utilities;

public class GameCanvasController : HiddenSingleton<GameCanvasController>
{
    [SerializeField]
    private TMP_Text dayText;
    [SerializeField]
    private TMP_Text timeText;
    [SerializeField]
    private TMP_Text alertText;
    
    //Unity Functions
    //============================================================================================================//

    private void OnEnable()
    {
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
    
}
