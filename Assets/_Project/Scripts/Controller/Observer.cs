using System;
using Spine.Unity.Examples;
using UnityEngine.UI;

public static class Observer
{
    #region GameSystem
    // Debug
    public static Action DebugChanged;
    // Currency
    public static Action<bool> SaveCurrencyTotal;
    public static Action<bool> CurrencyTotalChanged;
    // Level Spawn
    public static Action CurrentLevelChanged;
    // Setting
    public static Action MusicChanged;
    public static Action SoundChanged;
    public static Action PlayGamePlayMusic;
    public static Action VibrationChanged;
    // Ads
    public static Action RequestBanner;
    public static Action ShowBanner;
    public static Action RequestInterstitial;
    public static Action ShowInterstitial;
    public static Action RequestReward;
    public static Action ShowReward;
    // Other
    public static Action CoinMove;
    public static Action ClickButton;
    public static Action<string> TrackClickButton;
    public static Action ShowTrackingButton;
    public static Action<Ground,Passenger> StartPoint;
    public static Action<Ground> CalculatePath;
    public static Action PurchaseFail;
    public static Action PurchaseSucceed;
    public static Action ClaimReward;
    public static Action IntroWinGame;
    public static Action ClickonGround;
    public static Action DoneLevel;
    public static Action ShipMove;
    public static Action OnSwapping;
    public static Action EndSwapping;
    public static Action CountSwap;
    public static Action CountFly;
    public static Action PlaySwapSound;
    public static Action PlayFlySound;
    public static Action PlaySpinSound;
    public static Action MissionSound;
    public static Action CongratSound;
    public static Action OpenGiftSound;
    public static Action OnWin;
    public static Action OnLost;
    public static Action<bool> DoSpin;
    public static Action<int> PlayHardMode;
    public static Action UpdateStarReward;
    public static Action NewDailyReWard;
    public static Action UpdateText;
    public static Action<EMissionQuest> LoadTrackingMission;
    public static Action<bool> ShowNoticeIcon;
    public static Action UpdateProcressDaily;
    #endregion

    #region Gameplay
    // Game event
    public static Action<Level> StartLevel;
    public static Action<Level> ReplayLevel;
    public static Action<Level> SkipLevel;
    public static Action<Level> WinLevel;
    public static Action<Level> LoseLevel;
    public static Action PlayRunMusic;
    public static Action PlayWinSound;
    #endregion
}
