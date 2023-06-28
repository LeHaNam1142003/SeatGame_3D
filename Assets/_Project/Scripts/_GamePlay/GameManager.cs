using System;
using System.Collections.Generic;
using CodeStage.AdvancedFPSCounter;
using DG.Tweening;
using Pancake.GameService;
using UnityEngine;

public class GameManager : SingletonDontDestroy<GameManager>
{
    public LevelController levelController;
    public GameState gameState;

    public AFPSCounter AFpsCounter => GetComponent<AFPSCounter>();

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
    }
    public void WinHardMode(List<SetUpReward> getSetup)
    {
        if (gameState == GameState.WaitingResult || gameState == GameState.LoseGame || gameState == GameState.WinGame) return;
        gameState = GameState.WinGame;
        Observer.WinLevel?.Invoke(levelController.currentLevel);
        PopupController.Instance.HideAll();
        Data.CompletedHardMode += 1;
        var getPopupWinHardMode = PopupController.Instance.Get<PopupCongratulation>() as PopupCongratulation;
        foreach (var g in getSetup)
        {
            if (!getPopupWinHardMode.setupRewards.Contains(g))
            {
                getPopupWinHardMode.setupRewards.Add(g);
            }
        }
        getPopupWinHardMode.isBackHome = true;
        Data.HardModeUnlock++;
        PopupController.Instance.Show<PopupCongratulation>();
    }
    public void WinReplay()
    {
        if (gameState == GameState.WaitingResult || gameState == GameState.LoseGame || gameState == GameState.WinGame) return;
        gameState = GameState.WinGame;
        Observer.WinLevel?.Invoke(levelController.currentLevel);
        PopupController.Instance.HideAll();
        PopupController.Instance.Show<PopupWinReplay>();
    }
    public void LoseHardMode()
    {
        if (gameState == GameState.WaitingResult || gameState == GameState.LoseGame || gameState == GameState.WinGame) return;
        gameState = GameState.LoseGame;
        Observer.LoseLevel?.Invoke(levelController.currentLevel);
        PopupController.Instance.HideAll();
        PopupController.Instance.Show<PopupLoseHardMode>();
    }

    void Start()
    {
        ReturnHome();
        Observer.StartLevel += UpdateScore;
    }

    public void PlayCurrentLevel()
    {
        PrepareLevel();
        StartGame(false);
    }

    public void UpdateScore(Level level)
    {
        if (AuthService.Instance.isLoggedIn && AuthService.Instance.IsCompleteSetupName)
        {
            AuthService.UpdatePlayerStatistics("RANK_LEVEL", Data.CurrentLevel);
        }
    }

    public void PrepareLevel()
    {
        gameState = GameState.PrepareGame;
        levelController.PrepareLevel("Level", Data.CurrentLevel);
    }

    public void ReturnHome()
    {
        PrepareLevel();

        PopupController.Instance.HideAll();
        PopupController.Instance.Show<PopupBackground>();
        PopupController.Instance.Show<PopupHome>();
    }

    public void ReplayGame(bool isHardMode)
    {
        if (isHardMode)
        {
            StartHardModeGame(Data.CurrentHardMode);
        }
        else
        {
            Observer.ReplayLevel?.Invoke(levelController.currentLevel);
            PrepareLevel();
            StartGame(false);
        }
    }

    public void BackLevel(bool isHardMode)
    {
        if (isHardMode)
        {
            if (Data.CurrentHardMode > 1)
            {
                Data.CurrentHardMode--;
            }
            StartHardModeGame(Data.CurrentHardMode);
        }
        else
        {
            Data.CurrentLevel--;

            PrepareLevel();
            StartGame(false);
        }
    }

    public void NextLevel(bool isHardMode)
    {
        if (isHardMode)
        {
            Data.CurrentHardMode++;
            StartHardModeGame(Data.CurrentHardMode);
        }
        else
        {
            Observer.SkipLevel?.Invoke(levelController.currentLevel);
            Data.CurrentLevel++;
            PrepareLevel();
            StartGame(false);
        }
    }

    public void StartGame(bool isHardMode)
    {
        gameState = GameState.PlayingGame;
        Observer.StartLevel?.Invoke(levelController.currentLevel);

        PopupController.Instance.HideAll();
        if (PopupController.Instance.Get<PopupInGame>() is PopupInGame popupInGame)
        {
            popupInGame.isHardMode = isHardMode;
            popupInGame.Show();
        }
        levelController.currentLevel.gameObject.SetActive(true);
    }
    public void StartHardModeGame(int indexHardMode)
    {
        gameState = GameState.PrepareGame;
        levelController.PrepareLevel("HardMode", indexHardMode);
        StartGame(true);
    }

    public void OnWinGame(List<SetUpReward> getSetup, float delayPopupShowTime = 2.5f)
    {
        if (gameState == GameState.WaitingResult || gameState == GameState.LoseGame || gameState == GameState.WinGame) return;
        gameState = GameState.WinGame;
        Observer.WinLevel?.Invoke(levelController.currentLevel);
        Data.PlayLevel += 1;
        Data.CurrentLevel++;
        DOTween.Sequence().AppendInterval(delayPopupShowTime).AppendCallback(() =>
        {
            PopupController.Instance.HideAll();
            if (PopupController.Instance.Get<PopupWin>() is PopupWin popupWin)
            {
                popupWin.Show();
                foreach (var g in getSetup)
                {
                    if (!popupWin.setupRewards.Contains(g))
                    {
                        popupWin.setupRewards.Add(g);
                    }
                }
                popupWin.Show();
            }
        });
    }

    public void OnLoseGame(float delayPopupShowTime = 2.5f)
    {
        if (gameState == GameState.WaitingResult || gameState == GameState.LoseGame || gameState == GameState.WinGame) return;
        gameState = GameState.LoseGame;
        Observer.LoseLevel?.Invoke(levelController.currentLevel);

        DOTween.Sequence().AppendInterval(delayPopupShowTime).AppendCallback(() =>
        {
            PopupController.Instance.Hide<PopupInGame>();
            PopupController.Instance.Show<PopupLose>();
        });
    }

    public void ChangeAFpsState()
    {
        if (Data.IsTesting)
        {
            AFpsCounter.enabled = !AFpsCounter.isActiveAndEnabled;
        }
    }
}

public enum GameState
{
    PrepareGame,
    PlayingGame,
    WaitingResult,
    LoseGame,
    WinGame,
}
