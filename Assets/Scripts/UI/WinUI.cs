using System;
using Audio.Music;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class WinUI : MonoBehaviour
    {
        
        [Header("Main Menu")]
        [SerializeField]
        private Button playButton;
        [SerializeField]
        private Button quitButton;

        //============================================================================================================//
        
        // Start is called before the first frame update
        private void Start()
        {
            
            MUSIC.WIN.PlayMusic();

            ScreenFader.ForceSetColorBlack();
            playButton.onClick.AddListener(OnPlayButtonPressed);
            

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
            gameObject.SetActive(false);
            SceneManager.LoadScene("Level");
        }
        
        private void OnQuitButtonPressed()
        {
            Application.Quit();
        }
        
        //============================================================================================================//
    }
}
