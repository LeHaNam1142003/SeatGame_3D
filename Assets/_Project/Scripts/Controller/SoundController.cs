using UnityEngine;

public class SoundController : SingletonDontDestroy<SoundController>
{
    public AudioSource backgroundAudio;
    public AudioSource fxAudio;
    public SoundConfig SoundConfig => ConfigController.Sound;

    public void Start()
    {
        Setup();

        Observer.MusicChanged += OnMusicChanged;
        Observer.SoundChanged += OnSoundChanged;
        Observer.PlayGamePlayMusic += GamePlaySound;

        Observer.PlayWinSound += WinLevel;
        Observer.LoseLevel += LoseLevel;
        Observer.StartLevel += StartLevel;
        Observer.ClickButton += ClickButton;
        Observer.CoinMove += CoinMove;
        Observer.PlayRunMusic += PlayerRun;
        Observer.PlaySwapSound += SwapTool;
        Observer.PlayFlySound += FlyTool;
        Observer.PlaySpinSound += SpinWhee;
        Observer.MissionSound += MissionComplete;
        Observer.CongratSound += Congratulation;
        Observer.OpenGiftSound += OpenGift;
    }

    private void OnMusicChanged()
    {
        backgroundAudio.mute = !Data.BgSoundState;
    }

    private void OnSoundChanged()
    {
        fxAudio.mute = !Data.FxSoundState;
    }

    public void Setup()
    {
        OnMusicChanged();
        OnSoundChanged();
    }

    private void PlayFX(SoundType soundType)
    {
        SoundData soundData = SoundConfig.GetSoundDataByType(soundType);

        if (soundData != null)
        {
            fxAudio.PlayOneShot(soundData.GetRandomAudioClip());
        }
        else
        {
            Debug.LogWarning("Can't found sound data");
        }
    }

    private void PlayBackground(SoundType soundType)
    {
        SoundData soundData = SoundConfig.GetSoundDataByType(soundType);

        if (soundData != null)
        {
            backgroundAudio.clip = soundData.GetRandomAudioClip();
            backgroundAudio.Play();
        }
        else
        {
            Debug.LogWarning("Can't found sound data");
        }
    }
    void GamePlaySound()
    {
        PlayBackground(SoundType.BackgroundHome);
    }

    public void PauseBackground()
    {
        if (backgroundAudio)
        {
            backgroundAudio.Pause();
        }
    }
    public void StopFXSound()
    {
        fxAudio.Stop();
    }
    void OpenGift()
    {
        PlayFX(SoundType.OpenGift);
    }

    #region ActionEvent
    private void StartLevel(Level level)
    {
        PlayFX(SoundType.StartLevel);
    }
    private void PlayerRun()
    {
        PlayFX(SoundType.MainRunning);
    }
    private void SpinWhee()
    {
        PlayFX(SoundType.SpinWheel);
    }
    void MissionComplete()
    {
        PlayFX(SoundType.MissionComp);
    }
    void Congratulation()
    {
        PlayFX(SoundType.Congrat);
    }

    private void WinLevel()
    {
        PlayFX(SoundType.WinLevel);
    }

    private void LoseLevel(Level level)
    {
        PlayFX(SoundType.LoseLevel);
    }

    private void ClickButton()
    {
        PlayFX(SoundType.ClickButton);
    }
    void SwapTool()
    {
        PlayFX(SoundType.SwapTool);
    }
    void FlyTool()
    {
        PlayFX(SoundType.FlyTool);
    }

    private void CoinMove()
    {
        PlayFX(SoundType.CoinMove);
    }
    #endregion
}
