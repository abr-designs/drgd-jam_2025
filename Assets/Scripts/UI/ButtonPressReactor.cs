using System;
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
    
    private Vector3 m_startScale;
    private Quaternion m_startRotation;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        var button = GetComponent<Button>();
        m_transformAnimator = GetComponent<TransformAnimator>();
        
        m_startScale = transform.localScale;
        m_startRotation = transform.localRotation;
        
        button.onClick.AddListener(OnButtonPressed);
    }

    private void OnDisable()
    {
        m_transformAnimator.Stop();
        
        transform.localScale = m_startScale;
        transform.localRotation = m_startRotation;
    }


    private void OnButtonPressed()
    {
        m_transformAnimator.Play();
        soundWhenPressed.PlaySound();
    }
}
