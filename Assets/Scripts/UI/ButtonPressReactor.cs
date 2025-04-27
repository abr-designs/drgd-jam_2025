using Audio;
using Audio.SoundFX;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Animations;

[RequireComponent(typeof(Button), typeof(TransformAnimator))]
public class ButtonPressReactor : MonoBehaviour
{
    [SerializeField]
    private SFX soundWhenPressed;
    private TransformAnimator m_transformAnimator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        var button = GetComponent<Button>();
        m_transformAnimator = GetComponent<TransformAnimator>();
        
        button.onClick.AddListener(OnButtonPressed);
    }


    private void OnButtonPressed()
    {
        m_transformAnimator.Play();
        soundWhenPressed.PlaySound();
    }
}
