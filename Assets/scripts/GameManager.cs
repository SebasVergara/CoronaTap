using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;

public class GameManager : MonoBehaviour {

	public delegate void GameDelegate();
	public static event GameDelegate OnGameStarted;
	public static event GameDelegate OnGameOverConfirmed;

	public static GameManager Instance;

	public GameObject startPage;
	public GameObject gameOverPage;
	public GameObject countdownPage;
	public TextMeshProUGUI scoreText;
	public Button Continue;
	private RewardedAd rewardedAd;
	private InterstitialAd interstitial;

	enum PageState {
		None,
		Start,
		Countdown,
		GameOver
	}

	int score = 0;
	bool gameOver = true;

	public bool GameOver { get { return gameOver; } }

    public void Start()
    {
		MobileAds.Initialize(initStatus => { });
		CreateAndLoadRewardedAd();
	}

    void Awake() {
		if (Instance != null) {
			Destroy(gameObject);
		}
		else {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	void OnEnable() {
		TapController.OnPlayerDied += OnPlayerDied;
		TapController.OnPlayerScored += OnPlayerScored;
		CountdownText.OnCountdownFinished += OnCountdownFinished;
		SetPageState(PageState.Countdown);
	}

	void OnDisable() {
		TapController.OnPlayerDied -= OnPlayerDied;
		TapController.OnPlayerScored -= OnPlayerScored;
		CountdownText.OnCountdownFinished -= OnCountdownFinished;
	}

	void OnCountdownFinished() {
		SetPageState(PageState.None);
		OnGameStarted();
		//score = 0;
		gameOver = false;
	}

	void OnPlayerScored() {
		score++;
		scoreText.text = score.ToString();
	}

	void OnPlayerDied() {
		gameOver = true;
		int savedScore = PlayerPrefs.GetInt("HighScore");
		if (score > savedScore) {
			PlayerPrefs.SetInt("HighScore", score);
		}
		SetPageState(PageState.GameOver);
	}

	void SetPageState(PageState state) {
		switch (state) {
			case PageState.None:
				startPage.SetActive(false);
				gameOverPage.SetActive(false);
				countdownPage.SetActive(false);
				break;
			case PageState.Start:
				startPage.SetActive(true);
				gameOverPage.SetActive(false);
				countdownPage.SetActive(false);
				break;
			case PageState.Countdown:
				startPage.SetActive(false);
				gameOverPage.SetActive(false);
				countdownPage.SetActive(true);
				break;
			case PageState.GameOver:
				startPage.SetActive(false);
				gameOverPage.SetActive(true);
				countdownPage.SetActive(false);
				break;
		}
	}
	
	public void ConfirmGameOver() {
		SetPageState(PageState.Countdown);
		score = 0;
		scoreText.text = "0";
		OnGameOverConfirmed();
		Continue.interactable = true;
	}

	public void ConfirmGameOverContinue()
	{
        showInterstitialAdGame();
    }

	public void StartGame() {
		// SceneManager.LoadScene(1); 
		SetPageState(PageState.Countdown);
	}
	public void CreateAndLoadRewardedAd()
    {
        string adUnitIdInter = "ca-app-pub-3940256099942544/1033173712"; //your AdMob interstitial ID block
        this.interstitial = new InterstitialAd(adUnitIdInter);
        AdRequest requestInter = new AdRequest.Builder().Build();
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        this.interstitial.LoadAd(requestInter);
    }
    private void showInterstitialAdGame()
    {
        #if UNITY_ANDROID
		    if (this.interstitial.IsLoaded())
		    {
			    this.interstitial.Show();
		    } else
            {
			    ContinueWithScore();
		    }
        #endif

        #if UNITY_EDITOR
		    ContinueWithScore();
	    }
        #endif
	public void HandleUserEarnedReward(object sender, Reward args)
	{
		string type = args.Type;
		double amount = args.Amount;
		MonoBehaviour.print(
			"HandleRewardedAdRewarded event received for "
						+ amount.ToString() + " " + type);
        SetPageState(PageState.Countdown);
        OnGameOverConfirmed();
        Continue.interactable = false;
    }
	public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
	{
		SetPageState(PageState.Countdown);
		OnGameOverConfirmed();
		Continue.interactable = false;
	}
	public void HandleRewardedAdClosed(object sender, EventArgs args)
	{
		this.CreateAndLoadRewardedAd();
	}
	public void HandleOnAdClosed(object sender, EventArgs args)
	{
		SetPageState(PageState.Countdown);
		OnGameOverConfirmed();
		Continue.interactable = false;
	}
	public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
	{
		MonoBehaviour.print(
			"HandleRewardedAdFailedToLoad event received with message: "
							 + args.Message);
	}
	public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		MonoBehaviour.print(
			"HandleRewardedAdFailedToLoad event received with message: "
							 + args.Message);
	}
    public void ContinueWithScore()
    {
        SetPageState(PageState.Countdown);
        OnGameOverConfirmed();
        Continue.interactable = false;
    }

}
