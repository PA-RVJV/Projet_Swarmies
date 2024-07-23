using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("--------- Audio Source ---------")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("--------- Audio Clip ---------")]
    public AudioClip background;
    public AudioClip GameOverBackGround;
    public AudioClip buttonClick;
    public AudioClip backButtonClick;
    public AudioClip destroyUnit;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void ChangeBackgroundClip()
    {
        musicSource.Stop();
        musicSource.clip = GameOverBackGround;
        musicSource.Play();
    }
    
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);    
    }
    
    public void PlayUIButtonClickSFX()
    {
        SFXSource.PlayOneShot(buttonClick);
    }
    
    public void PlaUIBackButtonClickSFX()
    {
        SFXSource.PlayOneShot(backButtonClick);
    }
}
