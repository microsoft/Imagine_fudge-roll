using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameplayManager : MonoBehaviour
{
	public static GameplayManager Instance;

	[System.Serializable]
	public class Prop
	{
		public GameObject prefab;
		public float minScale;
		public float maxScale;
		public int spawnCount;
		public float minSpawnRange;
		public float maxSpawnRange;
	}

	[System.Serializable]
	public class Level
	{
		public GameplayStage stage;
		public int numToCollect;
	}

	public Prop[] props;
	public GameObject propContainer;

	public Level[] levels;

	public float silverTime;
	public float goldTime;

	public BallController ball;
	
	public int PropsCollected { get; private set; }

	int currentLevel;
	bool finishedLevels;

	public AudioClip stageCompletedSound;
	AudioSource stageCompletedSource;

	public GameObject stageCompleteAnim;

	public enum GameState
	{
		Countdown,
		InGame,
		GameOver
	};
	GameState state = GameState.Countdown;

	float timeSpent = 0f;
	int countdown = -1;

	void Awake()
	{
		Instance = this;
	}

	/// <summary>
	/// Logic that runs on object initialization.
	/// </summary>
	void Start()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		stageCompletedSource = AudioHelper.CreateAudioSource(gameObject, stageCompletedSound);

		currentLevel = 0;
		finishedLevels = false;

		// Spawn the collectible objects
		SpawnProps();

		//update UI
		UIManager.Instance.UpdateHUD(timeSpent, GetRankForTime(timeSpent), Mathf.Max(0, levels[currentLevel].numToCollect - PropsCollected), levels[currentLevel].stage);

		UIManager.Instance.ShowCountdown(countdown);
		UIManager.Instance.ShowHUD(false);
		UIManager.Instance.ShowScreen("Tutorial");
	}

	/// <summary>
	/// Logic that runs when we actually start the game
	/// </summary>
	public void OnStartGame()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		// Hide all screens except the HUD and start a countdown
		UIManager.Instance.ShowHUD(true);
		UIManager.Instance.ShowScreen("");
		countdown = 3;

		// Call ShowCountdown every 1 second, starting now
		InvokeRepeating("ShowCountdown", 0f, 1f);
	}

	/// <summary>
	/// Logic that runs every frame
	/// </summary>
	void Update()
	{
		if (state == GameState.InGame)
		{
			// Update the timer and rank
			timeSpent += Time.deltaTime;
			if (timeSpent > 0f)
			{
				UIManager.Instance.UpdateHUD(timeSpent, GetRankForTime(timeSpent), Mathf.Max(0, levels[currentLevel].numToCollect - PropsCollected), levels[currentLevel].stage);
			}
		}
	}

	/// <summary>
	/// Logic that runs after everything else every frame
	/// </summary>
	void LateUpdate()
	{	
		// Quit game on escape
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	/// <summary>
	/// Spawns collectible objects int he world.
	/// </summary>
	void SpawnProps()
	{
		// *** Add your source code here ***
	}

	/// <summary>
	/// Update the game start countdown timer, and start the game if it has finished.
	/// </summary>
	void ShowCountdown()
	{
		UIManager.Instance.ShowCountdown(countdown);

		if (countdown >= 0)
		{
			// Countdown is done
			if (countdown == 0)
			{
				state = GameState.InGame;
			}

			// Countdown should continue
			--countdown;
		}
		else
		{
			// Stop the countdown and show the first collection UI
			ShowCollectionText();
			CancelInvoke("ShowCountdown");
			
			// We are done counting down, start the game!
			state = GameState.InGame;
		}
	}
	
	/// <summary>
	/// Determines whether this instance can move.
	/// </summary>
	/// <returns><c>true</c> if this instance can move; otherwise, <c>false</c>.</returns>
	public bool CanMove()
	{
		// We can only move the ball when in gameplay
		return (state == GameState.InGame);
	}

	/// <summary>
	/// Increments the count of collected objects and update the game state
	/// </summary>
	public void IncrementPropCount()
	{
		++PropsCollected;
		//Update the HUD
		UIManager.Instance.UpdateHUD(timeSpent, GetRankForTime(timeSpent), Mathf.Max(0, levels[currentLevel].numToCollect - PropsCollected), levels[currentLevel].stage);

		// If we finished the level, call level completion logic
		if (!finishedLevels && PropsCollected >= levels[currentLevel].numToCollect)
		{
			OnStageCompleted();
		}
	}

	/// <summary>
	/// Gets the correct rank for the time spent.
	/// </summary>
	/// <returns>The rank.</returns>
	/// <param name="time">Time spent in game.</param>
	PlayerRank GetRankForTime(float time)
	{
		if (time <= goldTime)
		{
			return PlayerRank.Gold;
		}
		else if (time <= silverTime)
		{
			return PlayerRank.Silver;
		}
		else
		{
			return PlayerRank.Bronze;
		}
	}

	/// <summary>
	/// Logic that runs when we have detected the player has finished a level.
	/// </summary>
	public void OnStageCompleted()
	{
		// Play an animation
		var anim = Instantiate(stageCompleteAnim, ball.transform.position, Quaternion.identity);
		Destroy (anim, 5f);

		// Play audio
		stageCompletedSource.Play();

		// Update the level information
		// If we haven't finished all the levels...
		if (currentLevel + 1 < levels.Length)
		{
			currentLevel++;

			// Update ball properties
			ball.SetStickyness(currentLevel);
			// Reset prop count
			PropsCollected = 0;
			// Update UI
			UIManager.Instance.UpdateHUD(timeSpent, GetRankForTime(timeSpent), Mathf.Max(0, levels[currentLevel].numToCollect - PropsCollected), levels[currentLevel].stage);
			UIManager.Instance.ShowInfoText("Level Complete", 2f);
			// Call ShowCollectionText in 3 seconds
			Invoke("ShowCollectionText", 3f);
		}
		// If we finished the last level...
		else if (!finishedLevels)
		{
			finishedLevels = true;
			UIManager.Instance.ShowInfoText("Level Complete", 2f);
			// Call ShowFinishText in 3 seconds
			Invoke("ShowFinishText", 3f);
		}
	}

	/// <summary>
	/// Determines whether the player can finish the game by going through the goal.
	/// </summary>
	/// <returns><c>true</c> if the player can finish the game; otherwise, <c>false</c>.</returns>
	public bool CanFinishGame()
	{
		// We can finish the game if we're in-game and completed all levels.
		return (state == GameState.InGame) && finishedLevels;
	}

	/// <summary>
	/// Logic that runs when the game has been finished.
	/// </summary>
	public void OnGameComplete()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		// Update state and show game over screen
		state = GameState.GameOver;
		UIManager.Instance.ShowScreen("Game Complete");
	}

	/// <summary>
	/// Show appropriate UI text for collecting the current collectible object.
	/// </summary>
	void ShowCollectionText()
	{
		UIManager.Instance.ShowCollectionText(levels[currentLevel].numToCollect, levels[currentLevel].stage);
	}

	/// <summary>
	/// Shows instructions for finishing the game after completing all the levels.
	/// </summary>
	void ShowFinishText()
	{
		UIManager.Instance.ShowInfoText("Goal", 5f);
	}

	/// <summary>
	/// Restarts the game.
	/// </summary>
	public void OnRestart()
	{
		// Reload the scene
		Application.LoadLevel("Main");
	}

	/// <summary>
	/// Updates the HUD on language change.
	/// </summary>
	public void OnLanguageChanged()
	{
		UIManager.Instance.OnLanguageChanged();
		UIManager.Instance.UpdateHUD(timeSpent, GetRankForTime(timeSpent), Mathf.Max(0, levels[currentLevel].numToCollect - PropsCollected), levels[currentLevel].stage);
	}
}
