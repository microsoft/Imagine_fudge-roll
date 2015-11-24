using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// User interface manager. This controls most of the UI logic for the game.
/// </summary>
public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	public GameObject[] screens;
	public GameObject hud;

	public Text countText;
	public Text timerText;
	public Text countdownText;
	public Text rankText;
	public Text infoText;

	public AudioClip buttonClickSound;
	AudioSource buttonClick;

	public AudioClip countdownSound;
	AudioSource countdownSource;

	public AudioClip goSound;
	AudioSource goSource;

	float infoDisplayTime = 0f;

	void Awake()
	{
		Instance = this;
		infoText.text = "";
		infoText.CrossFadeAlpha(0f, 0f, true);
	}

	void Start()
	{
		// Setup audio
		buttonClick = AudioHelper.CreateAudioSource(gameObject, buttonClickSound);
		countdownSource = AudioHelper.CreateAudioSource(gameObject, countdownSound);
		goSource = AudioHelper.CreateAudioSource(gameObject, goSound);
	}

	void Update()
	{
		// Update our fade timer for info text that we want to disappear
		if (infoDisplayTime > 0f)
		{
			infoDisplayTime -= Time.deltaTime;
			if (infoDisplayTime <= 0f)
			{
				// Fade over half a second
				infoText.CrossFadeAlpha(0, 0.5f, false);
			}
		}
	}

	/// <summary>
	/// Show a specific screen and hide others.
	/// </summary>
	/// <param name="name">Screen name.</param>
	public void ShowScreen(string name)
	{
		// Show the screen with the given name and hide everything else
		foreach (GameObject screen in screens)
		{
			screen.SetActive(screen.name == name);
		}
	}

	/// <summary>
	/// Shows/hides the HUD.
	/// </summary>
	/// <param name="show">Do we show the HUD?</param>
	public void ShowHUD(bool show)
	{
		hud.SetActive(show);
	}

	/// <summary>
	/// Updates the HUD.
	/// </summary>
	/// <param name="time">Time spent.</param>
	/// <param name="rank">Rank.</param>
	/// <param name="count">Count.</param>
	/// <param name="stage">Stage.</param>
	public void UpdateHUD(float time, PlayerRank rank, int count, GameplayStage stage)
	{
		ShowTimer(time);
		ShowRank(rank);
		ShowCount(count, stage);
	}

	/// <summary>
	/// Updates the timer on the HUD.
	/// </summary>
	/// <param name="timeSpent">Time spent.</param>
	void ShowTimer(float timeSpent)
	{
		var tSpan = TimeSpan.FromSeconds(timeSpent);
		var timeString = String.Format("{0:00}:{1:00}:{2:000}", tSpan.Minutes, tSpan.Seconds, tSpan.Milliseconds);
		timerText.text = String.Format(LocalizationManager.Instance.GetString("HUD Time"), timeString);
	}

	/// <summary>
	/// Shows game over text on the HUD.
	/// </summary>
	public void ShowGameOver()
	{
		countdownText.text = LocalizationManager.Instance.GetString("Finished");
	}

	/// <summary>
	/// Updates the 
	/// </summary>
	/// <param name="propsCollected">Properties collected.</param>
	/// <param name="stage">Stage.</param>
	void ShowCount(int propsCollected, GameplayStage stage)
	{
		var format = LocalizationManager.Instance.GetString("HUD Count");
		countText.text = string.Format(format, GetStageString(stage), propsCollected.ToString());
	}

	/// <summary>
	/// Shows the countdown.
	/// </summary>
	/// <param name="countdown">Countdown.</param>
	public void ShowCountdown(float countdown)
	{
		// Show a number for countdown if it's not 0 yet
		if (countdown > 0)
		{
			countdownText.text = countdown.ToString();
			countdownSource.Play();
		}
		// Show "Go" for 0
		else if (countdown == 0)
		{
			countdownText.text = LocalizationManager.Instance.GetString("Go");
			goSource.Play();
		}
		// Hide otherwise
		else
		{
			countdownText.text = "";
		}
	}

	/// <summary>
	/// Updates HUD rank text.
	/// </summary>
	/// <param name="rank">Rank.</param>
	void ShowRank(PlayerRank rank)
	{
		string rankKey = "Rank " + rank.ToString();
		rankText.text = string.Format(LocalizationManager.Instance.GetString("HUD Rank"), LocalizationManager.Instance.GetString(rankKey));
	}

	/// <summary>
	/// Updates instructions to collect X things.
	/// </summary>
	/// <param name="numToCollect">Number to collect.</param>
	/// <param name="stage">The object type to collect.</param>
	public void ShowCollectionText(int numToCollect, GameplayStage stage)
	{
		var str = LocalizationManager.Instance.GetString("Collection");
		infoText.text = string.Format(str, numToCollect, GetStageString(stage));
		infoText.CrossFadeAlpha(1, 0.25f, false);
		infoDisplayTime = 5f;
	}

	/// <summary>
	/// Shows non-specific info text for a duration.
	/// </summary>
	/// <param name="text">Text.</param>
	/// <param name="time">Time.</param>
	public void ShowInfoText(string key, float time)
	{
		infoText.text = LocalizationManager.Instance.GetString(key);
		// Fade in over 0.25 seconds
		infoText.CrossFadeAlpha(1, 0.25f, false);
		infoDisplayTime = time;
	}

	/// <summary>
	/// Gets a readable string for the object type.
	/// </summary>
	/// <returns>The stage string.</returns>
	/// <param name="val">Value.</param>
	string GetStageString(GameplayStage val)
	{
		return LocalizationManager.Instance.GetString(val.ToString());
	}

	/// <summary>
	/// Called when a screen button is clicked.
	/// </summary>
	public void OnButtonClick()
	{
		// Play audio
		buttonClick.Play();
	}

	public void OnLanguageChanged()
	{
		foreach (GameObject o in screens)
		{
			var staticText = o.GetComponent<StaticTextManager>();
			if (staticText)
			{
				staticText.OnLanguageChanged();
			}
		}
	}
}