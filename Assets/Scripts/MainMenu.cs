using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public GameObject mapMenu;
	public GameObject mainMenu;
	public GameObject settingsMenu;
	public GameObject storeMenu;
	public GameObject tutorialMenu;
	public GameObject pauseMenu;
	public GameObject statisticsMenu;
	public GameObject answer_questionMenu;
	public GameObject personalCabinetMenu;

	public GameObject statistic1, statistic2;

	public Sprite red_jar, yellow_jar, green_jar;

	public GameObject questionTextField, noteTextField;

	public Image character;
	public Sprite[] characterImage;

	public Button[] levelButtons;
	public GameObject[] levelButtons_background;
	public int lastLevel;
	public GameObject LockedInfinity, UnlockedInfinity;

	public AudioClip onClick;

	public static bool isPaused = false;
	public bool isBtnAnimated = false;
	public string biggerOrSmaller = "bigger";

	private Questions questions = new Questions();
	private int questionIndex;
	private bool isAnswerReceived = false;
	public int rightAnswers = 0;
	int questionsLevel = 0;
	public GameObject yesButton, noButton, dontKnowButton, continueButton;
	public GameObject Questions_panel, Result_panel, bad_result, normal_result, expert_result;

	DateTime currentTime;
	DateTime oldTime;

	void Start () {
		Tamagochi ();
		isPaused = false;
		if (pauseMenu) {
			pauseMenu.SetActive (false);
		}
		Time.timeScale = 1.0f;
		//LockButtons();
		AndroidNotification.CancelNotification (1);
	}

	void Update() {
		if (mapMenu) {
			if (mapMenu.activeSelf) {
				if (!PlayerPrefs.HasKey ("LevelStars_" + (lastLevel - 1))) {
					if (!isBtnAnimated) {
						StartCoroutine (lastLevelBtnAnimation ());
						//Debug.Log (biggerOrSmaller);
					}
				}
			}
		}

		if (PlayerPrefs.HasKey ("opening_questionLevel")) {
			openQuestionMenu (PlayerPrefs.GetInt ("opening_questionLevel") - 1);
			PlayerPrefs.DeleteKey ("opening_questionLevel");
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			backButton ();
		}
	}

	public void openQuestionMenu(int level) {
		backButton ();
		SaveStats.Load ();
		questionsLevel = level;
		Result_panel.SetActive (false);
		Questions_panel.SetActive (true);
		rightAnswers = 0;
		answer_questionMenu.SetActive (true);
		StartCoroutine (GetQuestionsFromDb());
	}

	IEnumerator GetQuestionsFromDb() {
		SaveStats.answer_attempts += 1;
		int[] usedIndexes = new int[10];
		bool isRandomNeeded = true;
		for (int que = 0; que<10; que++) {
			isRandomNeeded = true;
			while (isRandomNeeded) {
				isRandomNeeded = false;
				this.questionIndex = UnityEngine.Random.Range (0, 60);
				for (int i = 0; i < que; i++) {
					if (questionIndex == usedIndexes [i]) {
						isRandomNeeded = true;
					}
				}
			}
			usedIndexes [que] = questionIndex;

			string question = questions.getQuestion (questionIndex);
			if (questionTextField) {
				questionTextField.GetComponent<Text> ().text = question;
			}
			if (noteTextField) {
				noteTextField.GetComponent<Text> ().text = "";
			}
			isAnswerReceived = false;
			continueButton.SetActive (false);
			yesButton.SetActive (true);
			noButton.SetActive (true);
			dontKnowButton.SetActive (true);
			for (; !isAnswerReceived ;) {
				yield return new WaitForSeconds (0.01f);
			}
		}
		int levelStars = 0;
		Result_panel.SetActive (true);
		Questions_panel.SetActive (false);
		if (rightAnswers < 5) {
			levelStars = 0;
			bad_result.SetActive (true);
			normal_result.SetActive (false);
			expert_result.SetActive (false);
		} else if (rightAnswers >= 5 && rightAnswers < 9) {
			levelStars = 1;
			bad_result.SetActive (false);
			normal_result.SetActive (true);
			expert_result.SetActive (false);
		} else if (rightAnswers >= 9) {
			levelStars = 3;
			bad_result.SetActive (false);
			normal_result.SetActive (false);
			expert_result.SetActive (true);
		}

		if (PlayerPrefs.HasKey ("LevelStars_" + (questionsLevel-1))) {
			if (levelStars > PlayerPrefs.GetInt ("LevelStars_" + (questionsLevel-1))) {			
				PlayerPrefs.SetInt ("LevelStars_" + (questionsLevel-1), levelStars);
			}
		} else {
			PlayerPrefs.SetInt ("LevelStars_" + (questionsLevel-1), levelStars);
		}
		int lastLevelUnlocked = PlayerPrefs.GetInt("LastLevelUnlocked");
		int nextLevel = questionsLevel;
		if (levelStars > 0) {
			if (lastLevelUnlocked < nextLevel) {
				PlayerPrefs.SetInt ("LastLevelUnlocked", nextLevel);
			}
		}
		SaveStats.Save ();
		PlayerPrefs.Save ();
		GameObject.Find ("Result_cifra").GetComponent<Text>().text = "Результат "+ rightAnswers + " из 10";
	}

	public void yes_button() {
		isAnswerReceived = true;
		bool answer = questions.getAnswers (questionIndex);
		//string note = questions.getNotes (questionIndex);
		yesButton.SetActive (false);
		noButton.SetActive (false);
		dontKnowButton.SetActive (false);
		if (answer) {
			//if (noteTextField) {
			//	noteTextField.GetComponent<Text> ().text = note;
			//}
			rightAnswers += 1;
			SaveStats.right_answers += 1;
			//continueButton.SetActive (true);
		} else {
			isAnswerReceived = true;
			SaveStats.fault_answers += 1;
		}
	}
	public void no_button() {
		isAnswerReceived = true;
		bool answer = questions.getAnswers (questionIndex);
		//string note = questions.getNotes (questionIndex);
		yesButton.SetActive (false);
		noButton.SetActive (false);
		dontKnowButton.SetActive (false);
		if (!answer) {
			//if (noteTextField) {
			//	noteTextField.GetComponent<Text> ().text = note;
			//}
			rightAnswers += 1;
			SaveStats.right_answers += 1;
			//continueButton.SetActive (true);
		} else {
			isAnswerReceived = true;
			SaveStats.fault_answers += 1;
		}
	}
	public void continue_button() {
		isAnswerReceived = true;
	}
		

	//***Кнопочки НАЧАЛО***


	//---Кнопки MainMenu---
	public void playButton () {
		if (settingsMenu.activeSelf) {
			settingsMenu.SetActive (false);
		}
		if (answer_questionMenu.activeSelf) {
			answer_questionMenu.SetActive (false);
		}
		if (personalCabinetMenu.activeSelf) {
			personalCabinetMenu.SetActive (false);
		}
		SaveStats.Load ();
		LockButtons ();
		mapMenu.SetActive (true);
		AudioSource.PlayClipAtPoint (onClick , new Vector2 (0, 0));
	}

	public void storeButton () {
		storeMenu.SetActive (true);
		AudioSource.PlayClipAtPoint (onClick , new Vector2 (0, 0));
	}

	public void settingsButton () {	
		settingsMenu.SetActive (true);
		/* if (AudioListener.pause) {
			GameObject.Find ("SoundButton").GetComponentInChildren<Text> ().text = "Включить звук";
		} else if (!AudioListener.pause) {
			GameObject.Find ("SoundButton").GetComponentInChildren<Text> ().text = "Выключить звук";
		} */
		AudioSource.PlayClipAtPoint (onClick , new Vector2 (0, 0));
	}

	public void tutorialButton () {
		//if (pauseMenu.activeSelf) {
		//	pauseMenu.SetActive (false);
		//}
		tutorialMenu.SetActive (true);	
		AudioSource.PlayClipAtPoint (onClick , new Vector2 (0, 0));
	}

	public void personalCabinetButton () {
		personalCabinetMenu.SetActive (true);
		statistic1.SetActive (false);
		statistic2.SetActive (false);
		AudioSource.PlayClipAtPoint (onClick , new Vector2 (0, 0));
	}

	public void ExitButton () {
		PlayerPrefs.Save ();
		//SaveStats.Save ();
		Application.Quit ();
	}

	public void OnApplicationQuit () {
		PlayerPrefs.SetString("theLastTime", System.DateTime.Now.ToBinary().ToString());
		NotifyController.turnOnNotif (20);
		PlayerPrefs.Save ();
		//SaveStats.Save ();
	}

	public void StatisticsButton () {
		AudioSource.PlayClipAtPoint (onClick , new Vector2(0, 0));
		statistic1.SetActive (true);
		SaveStats.Load();
		statistic1.transform.GetChild (1).GetComponent<Text> ().text = "Съедено хорошей еды: " + SaveStats.goodChoice.ToString ();
		statistic1.transform.GetChild (2).GetComponent<Text> ().text = "Допущено ошибок: " + SaveStats.badChoice.ToString ();
		statistic1.transform.GetChild (3).GetComponent<Text> ().text = "Пропущено еды: " + SaveStats.lostChoice.ToString ();
		if (PlayerPrefs.GetInt ("LastLevelUnlocked") > 0) {
			statistic1.transform.GetChild (4).GetComponent<Text> ().text = "Пройдено уровней: " + PlayerPrefs.GetInt ("LastLevelUnlocked");
		} else {
			statistic1.transform.GetChild (4).GetComponent<Text> ().text = "Пройдено уровней: 0";
		}
		if (PlayerPrefs.HasKey ("InfinityRecord")) {
			statistic1.transform.GetChild (5).GetComponent<Text> ().text = "Рекорд бесконечного уровня: " + PlayerPrefs.GetInt ("InfinityRecord");
		} else {
			statistic1.transform.GetChild (5).GetComponent<Text> ().text = "Рекорд бесконечного уровня: 0";
		}

	}

	public void answerStatisticsButton () {
		AudioSource.PlayClipAtPoint (onClick , new Vector2(0, 0));
		statistic2.SetActive (true);
		SaveStats.Load();
		statistic2.transform.GetChild (1).GetComponent<Text> ().text = "Правильных ответов: " + SaveStats.right_answers.ToString ();
		statistic2.transform.GetChild (2).GetComponent<Text> ().text = "Неправильных ответов: " + SaveStats.fault_answers.ToString ();
		statistic2.transform.GetChild (3).GetComponent<Text> ().text = "Попыток: " + SaveStats.answer_attempts.ToString ();
		if (PlayerPrefs.HasKey ("BuyedAnswers")) {
			statistic2.transform.GetChild (6).GetComponent<Text> ().text = "У вас куплено ответов: " + (PlayerPrefs.GetInt ("BuyedAnswers") - 100);
		} else {
			statistic2.transform.GetChild (6).GetComponent<Text> ().text = "У вас куплено ответов: 0";
		}
	}

	public void buyAnswersBtn() {
		if (!PlayerPrefs.HasKey ("BuyedAnswers")) {
			PlayerPrefs.SetInt ("BuyedAnswers", 100);
		}

		PlayerPrefs.SetInt ("BuyedAnswers", PlayerPrefs.GetInt("BuyedAnswers")+1);
		if (statistic2.activeSelf) {
		statistic2.transform.GetChild (6).GetComponent<Text> ().text = "У вас куплено ответов: " + (PlayerPrefs.GetInt ("BuyedAnswers") - 100);
		}
	}

	public void backButton () {	//Кнопка возврата
		AudioSource.PlayClipAtPoint (onClick , new Vector2 (0, 0));
		if (tutorialMenu.activeSelf) {
			tutorialMenu.SetActive (false);
		} else if (personalCabinetMenu.activeSelf) {
			personalCabinetMenu.SetActive (false);
		} else if (settingsMenu.activeSelf) {
			settingsMenu.SetActive (false);
		} else if (storeMenu.activeSelf) {
			storeMenu.SetActive (false);
		} else if (mapMenu.activeSelf) {
			mapMenu.SetActive (false);
		} else if (statisticsMenu.activeSelf) {
			statisticsMenu.SetActive (false);
		} else if (answer_questionMenu.activeSelf) {
			answer_questionMenu.SetActive (false);
		}

	}
	//---Кнопки MainMenu(конец)---

	//---Кнопки Settings---
	public void soundsOff () {
		if (!AudioListener.pause) {
			GameObject.Find ("SoundButton").GetComponentInChildren<Text> ().text = "-";
			AudioListener.pause = true;
		} else if (AudioListener.pause) {
			GameObject.Find ("SoundButton").GetComponentInChildren<Text> ().text = " ";
			AudioListener.pause = false;
		}
		AudioSource.PlayClipAtPoint (onClick, new Vector2 (0, 0));
	}

	public void unlock_all() {
		AudioSource.PlayClipAtPoint (onClick, new Vector2 (0, 0));
		lastLevel = 30;
		for (int i = 0; i < lastLevel; i++) {
			PlayerPrefs.SetInt ("LevelStars_" + i, 3);
		}
		PlayerPrefs.SetInt ("LastLevelUnlocked", 30);
		PlayerPrefs.Save();
	}

	public void resetSettings() {
		AudioSource.PlayClipAtPoint (onClick, new Vector2 (0, 0));
		PlayerPrefs.DeleteAll ();
		PlayerPrefs.SetInt ("LastLevelUnlocked", 0);
		PlayerPrefs.Save ();
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}
	//---Кнопки Settings(Конец)---

	//---Кнопки InGame---
	public void ingameTutorialButton () {
		isPaused = true;
		Time.timeScale = 0.0f;
		tutorialButton ();	
	}

	public void closeTutorial () {
		if (!pauseMenu.activeSelf) {
			isPaused = false;
			Time.timeScale = 1.0f;
		}
		tutorialMenu.SetActive (false);
		AudioSource.PlayClipAtPoint (onClick , new Vector2 (0, 0));		
	}

	public void resumeButton () {
		AudioSource.PlayClipAtPoint (onClick , new Vector2 (0, 0));
		isPaused = false;
		pauseMenu.SetActive (false);
		Time.timeScale = 1.0f;
	}

	public void mainMenuButton () {
		isPaused = false;
		AudioSource.PlayClipAtPoint (onClick , new Vector2 (0, 0));
		SceneManager.LoadScene ("Menu");
	}

	public void pauseButton () {
		isPaused = true;
		Time.timeScale = 0f;
		pauseMenu.SetActive (true);
	}
	//---Кнопки InGame(Конец)---

	//***Кнопочки КОНЕЦ***


	//Блокировка кнопок в зависимости от открытых уровней
	void LockButtons() {
		if (!PlayerPrefs.HasKey ("LastLevelUnlocked")) {
			PlayerPrefs.SetInt ("LastLevelUnlocked", 0);
		}
		//lastLevel = 1;
		lastLevel = (PlayerPrefs.GetInt ("LastLevelUnlocked")+1);
		for (int i = 0; i < levelButtons.Length; i++) {
			if (i < lastLevel) {
				levelButtons_background [i].GetComponent<Image> ().enabled = false;
			}

			Debug.Log ("LastLevel: " +lastLevel);

			levelButtons[i].interactable = i < lastLevel;
			if (PlayerPrefs.HasKey ("LevelStars_" + i)) {
				if (PlayerPrefs.GetInt ("LevelStars_" + i) == 0) {
					levelButtons [i].gameObject.GetComponent<Image> ().sprite = red_jar;
				}
				if (PlayerPrefs.GetInt ("LevelStars_" + i) == 1 || PlayerPrefs.GetInt ("LevelStars_" + i) == 2) {
					levelButtons [i].gameObject.GetComponent<Image> ().sprite = yellow_jar;
				}
				if (PlayerPrefs.GetInt ("LevelStars_" + i) == 3) {
					levelButtons [i].gameObject.GetComponent<Image> ().sprite = green_jar;
				}
			}
		}
		if (LockedInfinity) {
			LockedInfinity.SetActive (true);
		}
		if (PlayerPrefs.HasKey ("LevelStars_29")) {
			if (PlayerPrefs.GetInt ("LevelStars_29") >= 1) {
				LockedInfinity.SetActive (false);
				UnlockedInfinity.SetActive(true);
			}
		}
	}

	IEnumerator lastLevelBtnAnimation() {
		isBtnAnimated = true;
		if (lastLevel > 30) {
			lastLevel = 30;
		}
		if (levelButtons [lastLevel - 1].gameObject.GetComponent<RectTransform> ().localScale.x >= 0.76f) {
			yield return new WaitForSeconds(0.1f);
			biggerOrSmaller = "smaller";
		}
		if (levelButtons [lastLevel - 1].gameObject.GetComponent<RectTransform> ().localScale.x <= 0.56f) {
			yield return new WaitForSeconds(0.1f);
			biggerOrSmaller = "bigger";
		}
		if (biggerOrSmaller == "bigger") {
			levelButtons [lastLevel - 1].gameObject.transform.localScale = new Vector3 (levelButtons [lastLevel - 1].gameObject.transform.localScale.x+0.005f,levelButtons [lastLevel - 1].gameObject.transform.localScale.x+0.005f, 1);
		}
		if (biggerOrSmaller == "smaller") {
			levelButtons [lastLevel - 1].gameObject.transform.localScale = new Vector3 (levelButtons [lastLevel - 1].gameObject.transform.localScale.x-0.005f,levelButtons [lastLevel - 1].gameObject.transform.localScale.x-0.005f, 1);
		}
		yield return new WaitForSeconds(0.015f);
		isBtnAnimated = false;
		yield return biggerOrSmaller;
	}

	public void StartLevel (string level) {
		AudioSource.PlayClipAtPoint (onClick , new Vector2 (0, 0));
		SceneManager.LoadScene (level);
	}

	void Tamagochi () {
		currentTime = System.DateTime.Now;
		long tmp = Convert.ToInt64 (SaveStats.theLastTime);
		oldTime = DateTime.FromBinary (tmp);
		TimeSpan difference = currentTime.Subtract (oldTime);
	}

	public void OpenExceterURL() {
		Application.OpenURL ("https://www.exeter.ac.uk/foodt/about/");
	}

}
