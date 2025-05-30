using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public static event Action OnGameStarted;
        
        [Header("Main Menu")]
        [SerializeField]
        private Button playButton;
        [SerializeField]
        private Button settingsButton;
        [SerializeField]
        private Button quitButton;

        [SerializeField, Header("Windows")] 
        private BaseUIWindow settingsWindow;
        //============================================================================================================//
        
        // Start is called before the first frame update
        private void Start()
        {
            Assert.IsNotNull(settingsWindow);
            
            ScreenFader.ForceSetColorBlack();
            playButton.onClick.AddListener(OnPlayButtonPressed);
            
            settingsButton.onClick.AddListener(OnSettingButtonPressed);

#if UNITY_WEBGL
            quitButton.gameObject.SetActive(false);
#else
            quitButton.onClick.AddListener(OnQuitButtonPressed);
#endif

            ScreenFader.FadeIn(1f, null);
        }

        //============================================================================================================//
        
        private void OnPlayButtonPressed()
        {
            OnGameStarted?.Invoke();
            gameObject.SetActive(false);
        }
        
        private void OnSettingButtonPressed()
        {
            settingsWindow.OpenWindow();
        }

        private void OnQuitButtonPressed()
        {
            Application.Quit();
        }
        
        //============================================================================================================//
    }
}
