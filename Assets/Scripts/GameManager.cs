using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager gm;

    public float levelTime = 30.0f;
    public int negativeScoreMultiplier = 1;
    
	public int spawningFoodAmount = 30;
	public int usedFood = 0;
	public int scoreToBeatLevel = 20;
	public int twoStarsScore = 25;
	public int threeStarsScore = 30;
	public int rightChoiseSeries = 0;

	public int statsGoodFood;
	public int statsBadFood;
	public int statsLostFood;

	public GameObject statisticWindow;
	public GameObject status_norm, status_perfect;
	public GameObject bankaIndicator, smallBankaIndicator, indicatorPolka;
	public GameObject explosion;

	public int levelStars;
	public int counterForInfinity;
	public int amountScoreForInfinity = 0;

	public bool gameOver = true;
	public bool isMobile = false;

	public GameObject gameWonMessage;
	public GameObject gameLostMessage;
	public GameObject seriesTextSpawner;
	public GameObject NewRecordText;

	public Text timeDisplay;
	public Text scoreDisplay;
    public Text gameOverScoreDisplayWon;
	public Text gameOverScoreDisplayLost;
	public Text gameOverInfiniteScore;
	public Text gameWonInfiniteScore;
	public Text gameLoseInfiniteScore;
	public Text WonInfiniteRecord;
	public Text LoseInfiniteRecord;
	public Text scoreToGetDisplay;
	public Text gameStartingTimeDisplay;

    public Canvas gameStartingCanvas;
    public Canvas gameOverCanvas;
    public Canvas mainCanvas;

    public Button nextLevelButton;
	public GameObject backgroundCanvas;

	public Rigidbody2D[] serieTexts;
	public GameObject [] stats;

	private float currentTime;
	private int currentRightSerie = 0;
	private int score;
	public Transform[] indicatorsArray;

	public bool infinityMode;

	private const int menuScenesNumber = 2;

	void SpawnIndicators(int indicatorsOnShelf, int scrnHeightDiv) {
		int derr = 0;
		int indicators_left = spawningFoodAmount;
		int shelves_amount;
		if (spawningFoodAmount <= 30) {
			shelves_amount = 1;
		} else if (spawningFoodAmount > 30 && spawningFoodAmount <= 60) {
			shelves_amount = 2;
		} else {
			shelves_amount = (Mathf.CeilToInt (spawningFoodAmount / indicatorsOnShelf)) + 1;
		}
		for (int s = 0; s < shelves_amount; s++) {
			if (indicators_left > 30) {
				derr = 0;
			} else if (indicators_left <= indicatorsOnShelf) {
				derr = 30 - indicators_left;
			}
			if (indicators_left > 0) {
				GameObject polka = Instantiate (indicatorPolka as GameObject, GameObject.Find ("Indicator_panel").transform);
				polka.name = "Shelf_" + s;
				polka.transform.localPosition = new Vector3 (370, (-Screen.height / scrnHeightDiv) * s, -40);
			}
			for (int i = 0; i < spawningFoodAmount/shelves_amount; i++) {
				GameObject banka = Instantiate (bankaIndicator as GameObject, GameObject.Find ("Shelf_" + s).transform);
				banka.name = "indicator_" + (i + (spawningFoodAmount/shelves_amount * s));
				//Debug.Log ("Дистанция: " + distance + ", доп. дистанция: " + (distance / 1.7f) + ", в сумме расстояние: " + (distance + (distance / 1.7f)));
			}
			if (indicators_left > indicatorsOnShelf) {
				indicators_left -= indicatorsOnShelf;
			} else {
				indicators_left = 0;
			}
		}

		indicatorsArray = GameObject.Find ("Indicator_panel").GetComponentsInChildren<Transform>();
		for (int i = 0 ; i< indicatorsArray.Length ; i++) {
			if (indicatorsArray [i].gameObject.name.Contains("indicator")) {
				indicatorsArray [i].gameObject.GetComponent<Image> ().enabled = false;
			}
		}
	}

	void SpawnSmallIndicators(int indicatorsOnShelf, int scrnHeightDiv) {
		int derr = 0;
		int indicators_left = spawningFoodAmount;
		int shelves_amount;
		if (spawningFoodAmount <= 60) {
			shelves_amount = 1;
		} else if (spawningFoodAmount > 60 && spawningFoodAmount <= 120) {
			shelves_amount = 2;
		} else {
			shelves_amount = (Mathf.CeilToInt (spawningFoodAmount / indicatorsOnShelf)) + 1;
		}
		for (int s = 0; s < shelves_amount; s++) {
			if (indicators_left > indicatorsOnShelf) {
				derr = 0;
			} else if (indicators_left <= indicatorsOnShelf) {
				derr = 60 - indicators_left;
			}
			if (indicators_left > 0) {
				GameObject polka = Instantiate (indicatorPolka as GameObject, GameObject.Find ("Indicator_panel").transform);
				polka.name = "Shelf_" + s;
				polka.GetComponent<HorizontalLayoutGroup>().padding.left = 20;
				polka.GetComponent<HorizontalLayoutGroup>().padding.right = 20;
				polka.GetComponent<HorizontalLayoutGroup>().padding.bottom = 14;
				polka.transform.localPosition = new Vector3 (370, (-Screen.height / scrnHeightDiv) * s, -40);
			}
			for (int i = 0; i < spawningFoodAmount/shelves_amount; i++) {
				GameObject banka = Instantiate (smallBankaIndicator as GameObject, GameObject.Find ("Shelf_" + s).transform);
				banka.name = "indicator_" + (i + (spawningFoodAmount/shelves_amount * s));
				//Debug.Log ("Дистанция: " + distance + ", доп. дистанция: " + (distance / 1.7f) + ", в сумме расстояние: " + (distance + (distance / 1.7f)));
			}
			if (indicators_left > indicatorsOnShelf) {
				indicators_left -= indicatorsOnShelf;
			} else {
				indicators_left = 0;
			}
		}

		indicatorsArray = GameObject.Find ("Indicator_panel").GetComponentsInChildren<Transform>();
		for (int i = 0 ; i< indicatorsArray.Length ; i++) {
			if (indicatorsArray [i].gameObject.name.Contains("indicator")) {
				indicatorsArray [i].gameObject.GetComponent<Image>().enabled = false;
			}
		}
	}

	// Use this for initialization
	void Start () {
		if (SceneManager.GetActiveScene ().name != "LevelInfinity") {
			if (spawningFoodAmount <= 60) {
				SpawnIndicators (30, 22); //2.3f
			} else if (spawningFoodAmount > 60) {
				SpawnSmallIndicators (60, 24); //2.1f		
			}
		}

		backgroundCanvas.SetActive (true);
        currentTime = levelTime;
        score = 0;

        if (gm == null) {
            gm = this;
        }

        if (timeDisplay) {
            timeDisplay.text = currentTime.ToString();
        }
        if (scoreDisplay) {
            scoreDisplay.text = score.ToString();
        }

        if (gameStartingTimeDisplay) {
            gameStartingTimeDisplay.text = levelTime + " секунд";
        }

		if (scoreToGetDisplay && !infinityMode) {
            scoreToGetDisplay.text = "Набрать" + scoreToBeatLevel + " очков, чтобы пройти уровень";
        }

        if (gameStartingCanvas) {
            gameStartingCanvas.gameObject.SetActive(true);
        }

       // if (mainCanvas) {
       //     mainCanvas.gameObject.SetActive(false);
      //  }

        if (gameOverCanvas) {
            gameOverCanvas.gameObject.SetActive(false);
        }
	}

    // Update is called once per frame
    void Update() {
		//if (Input.GetKeyDown(KeyCode.R)) {
		//	score += 1;
		//}

		if (infinityMode && score > 0) {
			currentTime += Time.deltaTime;
			timeDisplay.text = currentTime.ToString ("0.00");
			if (counterForInfinity == 10) {
				Spawner sp = FindObjectOfType<Spawner> ();
				if (sp.MovementSpeed < 0.16f) {
					sp.MovementSpeed += 0.01f;
				}
				if (sp.minTimeBetweenSpawns > 0.2f && sp.maxTimeBetweenSpawns > 0.4f) {
					sp.minTimeBetweenSpawns -= 0.1f;
					sp.maxTimeBetweenSpawns -= 0.1f;
				}
				counterForInfinity = 0;
			}

			if (currentTime > 60 && currentTime < 120) {
				negativeScoreMultiplier = 3;
			} else if (currentTime > 120 && currentTime < 240) {
				negativeScoreMultiplier = 5;
			} else if (currentTime > 240 && currentTime < 300) {
				negativeScoreMultiplier = 5;
			} if (currentTime > 300) {
				negativeScoreMultiplier = 7;
			}
		}

		if (infinityMode && score == 0 && !gameOver) {
			EndInfinityGame ();
			//showStatsInfinity ();
		}

		if (currentRightSerie != rightChoiseSeries) {
			currentRightSerie = rightChoiseSeries;
			RightSerie(rightChoiseSeries);
		}

		if (usedFood == spawningFoodAmount && !infinityMode) {
			currentTime = 0.01f;
		}

        if (!gameOver && !infinityMode) {
            currentTime -= Time.deltaTime;
            if (currentTime < 0) {
                EndGame();
            }
            else {
                if (timeDisplay) {
                    timeDisplay.text = currentTime.ToString("0.00");
                }
            }
        }
    }

    public void StartGame() {
        gameOver = false;
        if (gameStartingCanvas) {
            gameStartingCanvas.gameObject.SetActive(false);
        }
        if (mainCanvas) {
            mainCanvas.gameObject.SetActive(true);
        }
    }

	public void StartGameInfinity() {
		amountScoreForInfinity = 0;
		gameOver = false;
		score = 10;
		scoreDisplay.text = score.ToString ();
		infinityMode = true;
		if (gameStartingCanvas) {
			gameStartingCanvas.gameObject.SetActive(false);
		}
		if (mainCanvas) {
			mainCanvas.gameObject.SetActive(true);
		}

	}

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMenu() {
		//EndGame ();
        SceneManager.LoadScene("Menu");
    }

    public void NextLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

	public void NextQuestionLevel(int levelnum) {
		PlayerPrefs.DeleteKey ("opening_questionLevel");
		PlayerPrefs.SetInt ("opening_questionLevel",levelnum);
		SceneManager.LoadScene("Menu");
	}

    void EndGame() {
		backgroundCanvas.SetActive (false);
        gameOver = true;
        if (score >= scoreToBeatLevel) {
            GameWon();
        }
        else {
            GameLost();
        }
        if (nextLevelButton) {
            UnlockNextLevelButton();
        }
        if (gameOverCanvas) {
            gameOverCanvas.gameObject.SetActive(true);
        }
    }

	public void EndInfinityGame() {
		backgroundCanvas.SetActive (false);
		gameOver = true;
		if (score > 0) {
			InfinityGameWon();
		}
		else {
			InfinityGameLost();
		}
		if (gameOverCanvas) {
			gameOverCanvas.gameObject.SetActive(true);
		}
	}

	public void addIndicatorPoint () {
		//Debug.Log (score);
		if (score > 0) {
			for (int i = 0; i < indicatorsArray.Length; i++) {
				if (indicatorsArray [i].gameObject.name == "indicator_" + (score - 1)) {
					indicatorsArray [i].gameObject.GetComponent<Image> ().enabled = true;
				}
			}
		}
	}

	public void removeIndicatorPoint () {
		//Debug.Log (score);
		if (score > 0) {
			for (int i = 0; i < indicatorsArray.Length; i++) {
				if (indicatorsArray [i].gameObject.name == "indicator_" + score) {
					indicatorsArray [i].gameObject.GetComponent<Image> ().enabled = false;
					GameObject exp = Instantiate (explosion, indicatorsArray [i].transform);
					exp.transform.localPosition = new Vector3 (0f, 0f, 0f);
					Destroy (exp, 0.8f);
				}
			}
		}
	}

	void RightSerie (int currentSerie) {
		if (currentSerie == 5) {
			Rigidbody2D currentSerieGameObject = Instantiate(serieTexts[0],seriesTextSpawner.transform.position, Quaternion.identity) as Rigidbody2D;
			currentSerieGameObject.velocity = (Vector2.up * 2f);
		} else if (currentSerie == 10) {
			Rigidbody2D currentSerieGameObject = Instantiate(serieTexts[1],seriesTextSpawner.transform.position, Quaternion.identity) as Rigidbody2D;
			currentSerieGameObject.velocity = (Vector2.up * 2f);
		} else if (currentSerie == 15) {
			Rigidbody2D currentSerieGameObject = Instantiate(serieTexts[2],seriesTextSpawner.transform.position, Quaternion.identity) as Rigidbody2D;
			currentSerieGameObject.velocity = (Vector2.up * 2f);
		} else if (currentSerie == 20) {
			Rigidbody2D currentSerieGameObject = Instantiate(serieTexts[3],seriesTextSpawner.transform.position, Quaternion.identity) as Rigidbody2D;
			currentSerieGameObject.velocity = (Vector2.up * 2f);
			//currentSerie = 0;
		}
	}

    void GameWon() {
        if (gameOverScoreDisplayWon) {
            gameOverScoreDisplayWon.text += score.ToString();
        }
		levelStars++;
		if (score >= twoStarsScore) {
			levelStars++;
		}
		if (score >= threeStarsScore) {
			levelStars++;
		}
        if (gameWonMessage) {
            gameWonMessage.SetActive(true);
        }
        if (gameLostMessage) {
            gameLostMessage.SetActive(false);
        }

		if (levelStars < 3) {
			status_norm.SetActive (false);
			status_perfect.SetActive (true);
		}
		if (levelStars == 3) {
			status_norm.SetActive (false);
			status_perfect.SetActive (true);
		}
		if (PlayerPrefs.HasKey ("LevelStars_" + (SceneManager.GetActiveScene ().buildIndex - 2))) {
			if (levelStars > PlayerPrefs.GetInt ("LevelStars_" + (SceneManager.GetActiveScene ().buildIndex - 2))) {			
				PlayerPrefs.SetInt ("LevelStars_" + (SceneManager.GetActiveScene ().buildIndex - 2), levelStars);
			}
		} else {
			PlayerPrefs.SetInt ("LevelStars_" + (SceneManager.GetActiveScene ().buildIndex - 2), levelStars);
		}			
		stats [0].GetComponent<Text> ().text += GameManager.gm.statsGoodFood.ToString ();
		stats [1].GetComponent<Text> ().text += GameManager.gm.statsBadFood.ToString ();
		stats [2].GetComponent<Text> ().text += GameManager.gm.statsLostFood.ToString ();

		//int currentlevel = (SceneManager.GetActiveScene ().buildIndex - 2);

        int lastLevelUnlocked = PlayerPrefs.GetInt("LastLevelUnlocked");
        int nextLevel = SceneManager.GetActiveScene().buildIndex - menuScenesNumber + 1;
        if (lastLevelUnlocked < nextLevel) {
			PlayerPrefs.SetInt ("LastLevelUnlocked", nextLevel);
        }
		//SaveStats.Save ();
		PlayerPrefs.Save ();
    }

    void GameLost() {
		levelStars = 0;
		if (PlayerPrefs.HasKey ("LevelStars_" + (SceneManager.GetActiveScene ().buildIndex - 2))) {
			if (levelStars > PlayerPrefs.GetInt ("LevelStars_" + (SceneManager.GetActiveScene ().buildIndex - 2))) {			
				PlayerPrefs.SetInt ("LevelStars_" + (SceneManager.GetActiveScene ().buildIndex - 2), levelStars);
			}
		} else {
			PlayerPrefs.SetInt ("LevelStars_" + (SceneManager.GetActiveScene ().buildIndex - 2), levelStars);
		}
        if (gameOverScoreDisplayLost) {
			gameOverScoreDisplayLost.text += score + "/" + scoreToBeatLevel;
        }
        if (gameWonMessage) {
            gameWonMessage.SetActive(false);
        }
        if (gameLostMessage) {
            gameLostMessage.SetActive(true);
        }

		stats [3].GetComponent<Text> ().text += GameManager.gm.statsGoodFood.ToString ();
		stats [4].GetComponent<Text> ().text += GameManager.gm.statsBadFood.ToString ();
		stats [5].GetComponent<Text> ().text += GameManager.gm.statsLostFood.ToString ();
		PlayerPrefs.Save ();
    }

	void InfinityGameWon() {
		if (PlayerPrefs.HasKey ("InfinityRecord")) {
			if (PlayerPrefs.GetInt ("InfinityRecord") < score) {
				PlayerPrefs.SetInt ("InfinityRecord", score);
				status_norm.SetActive (false);
				status_perfect.SetActive (true);
				NewRecordText.SetActive (true);
			}
		} else {
			PlayerPrefs.SetInt ("InfinityRecord", score);
			status_norm.SetActive (false);
			status_perfect.SetActive (true);
			NewRecordText.SetActive (true);
		}
		if (!status_perfect.activeSelf) {
			status_perfect.SetActive (false);
			status_norm.SetActive (true);
		}

		if (gameWonMessage) {
			gameWonMessage.SetActive(true);
		}
		if (gameLostMessage) {
			gameLostMessage.SetActive(false);
		}

		if (gameOverInfiniteScore) {
			gameOverInfiniteScore.text += score + " БАЛЛОВ";
		}
		if (gameWonInfiniteScore) {
			gameWonInfiniteScore.text += score + " БАЛЛОВ";
		}
		if (WonInfiniteRecord) {
			WonInfiniteRecord.text += PlayerPrefs.GetInt ("InfinityRecord") + " БАЛЛОВ";
		}

		stats [3].GetComponent<Text> ().text += GameManager.gm.statsGoodFood.ToString ();
		stats [4].GetComponent<Text> ().text += GameManager.gm.statsBadFood.ToString ();
		stats [5].GetComponent<Text> ().text += GameManager.gm.statsLostFood.ToString ();
		PlayerPrefs.Save ();
	}

	void InfinityGameLost() {
		if (!PlayerPrefs.HasKey ("InfinityRecord")) {
			PlayerPrefs.SetInt ("InfinityRecord", 0);
		}

		if (gameLostMessage) {
			gameLostMessage.SetActive(true);
		}
		if (gameWonMessage) {
			gameWonMessage.SetActive(false);
		}

		if (gameOverInfiniteScore) {
			gameOverInfiniteScore.text += score + " БАЛЛОВ";
		}
		if (gameLoseInfiniteScore) {
			gameLoseInfiniteScore.text += score + " БАЛЛОВ";
		}
		if (LoseInfiniteRecord) {
			LoseInfiniteRecord.text += PlayerPrefs.GetInt ("InfinityRecord") + " БАЛЛОВ";
		}

		stats [3].GetComponent<Text> ().text += GameManager.gm.statsGoodFood.ToString ();
		stats [4].GetComponent<Text> ().text += GameManager.gm.statsBadFood.ToString ();
		stats [5].GetComponent<Text> ().text += GameManager.gm.statsLostFood.ToString ();
		PlayerPrefs.Save ();
	}

	public void showStatistic() {
		statisticWindow.SetActive (true);
	}
	public void closeStatistic() {
		statisticWindow.SetActive (false);
	}

	void showStatsInfinity () {
		infinityMode = false;
		gameOver = true;

		if (!gameLostMessage) {
			gameLostMessage.SetActive(true);
		}

		if (gameOverScoreDisplayWon) {
			gameOverScoreDisplayWon.text = (10 + amountScoreForInfinity).ToString();
		}

		gameOverCanvas.gameObject.SetActive (true);
		stats [3].GetComponent<Text> ().text += GameManager.gm.statsGoodFood.ToString ();
		stats [4].GetComponent<Text> ().text += GameManager.gm.statsBadFood.ToString ();
		stats [5].GetComponent<Text> ().text += GameManager.gm.statsLostFood.ToString ();
	}

    void UnlockNextLevelButton ()
	{
		int lastLevelUnlocked = PlayerPrefs.GetInt ("LastLevelUnlocked");
		int nextLevel = SceneManager.GetActiveScene ().buildIndex - menuScenesNumber + 1;
		nextLevelButton.interactable = nextLevel <= lastLevelUnlocked;
	}

	void OnApplicationQuit() {
		GameLost ();
	}
	        

    public void foodPulled(int scoreAmount) {
        if (scoreAmount < 0) {
            scoreAmount *= negativeScoreMultiplier;
        }
        score += scoreAmount;
        if (score < 0) {
            score = 0;
        }
        if (scoreDisplay) {
            if (score >= scoreToBeatLevel) {
                scoreDisplay.text = score.ToString();
            }
            else {
                scoreDisplay.text = score + "/" + scoreToBeatLevel;
            }
        }
    }
}