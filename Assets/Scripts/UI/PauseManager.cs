using Cyberultimate.Unity;
using System.Collections;
using System.Collections.Generic;
using LetterBattle.Utility;
using UnityEngine;
using Game;

public class PauseManager : MonoSingleton<PauseManager>
{
    [SerializeField]
    private GameObject pauseObject = null;
    [SerializeField]
    private GameObject gameOverObject = null;

    [SerializeField]
    private StarterAssets.StarterAssetsInputs input = null;

    public bool IsPaused { get; private set; } = false;
    public bool IsDead { get; private set; } = false;

    public GameObject LastObject { get; private set; } = null;

    [SerializeField]
    private AudioSource source = null;

    [SerializeField]
    private AudioClip gameOver;

    public void SwitchPause()
    {
        if (IsDead)
        {
            return;
        }

        IsPaused = !IsPaused;
        Pause(pauseObject, IsPaused);
    }

    public void SwitchDeath()
    {
        IsDead = !IsDead;
        if (IsDead)
        {
            GameMusic.Current.FadeOut(0.5f);
            source.PlayOneShot(gameOver);

        }
        IsPaused = true;
        Pause(gameOverObject, IsDead);
    }

    public void Pause(GameObject obj, bool isTrue)
    {
        input.SetCursorState(isTrue);
        if (isTrue)
        {
            Time.timeScale = 0;
            // TimeScaling.Status.Register(obj, 0);
        }

        else
        {
            Time.timeScale = 1;
            // screw u biegus, these scripts of yours are shady
            // TimeScaling.Status.Unregister(obj);
        }
        obj.SetActive(isTrue);
        LastObject = obj;
        
        PauseObject.Current.OnOpen();
    }
}
