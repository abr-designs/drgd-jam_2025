using System;
using UnityEngine;
using Utilities;

namespace Controllers
{
    public class DayController : HiddenSingleton<DayController>
    {
        public static event Action OnDayFinished;
        public static event Action<float> OnTimeLeftChanged; 

        [SerializeField, Min(0f)]
        private float timeForDay;

        private bool m_dayActive;
        private float m_timeLeft;

        //Unity Functions
        //============================================================================================================//
        
        private void Update()
        {
            if (!m_dayActive)
                return;

            UpdateTimerValue();

            if (m_timeLeft <= 0f)
                EndDay();
        }
        
        //============================================================================================================//
        public static void StartGameDay()
        {
            Instance?.StartDay();
        }

        private void UpdateTimerValue()
        {
            m_timeLeft -=Time.deltaTime;
            
            OnTimeLeftChanged?.Invoke(m_timeLeft);
        }

        private void StartDay()
        {
            m_dayActive = true;
            m_timeLeft = timeForDay;
        }

        private void EndDay()
        {
            m_dayActive = false;
            m_timeLeft = 0f;
            OnDayFinished?.Invoke();
        }
    }
}