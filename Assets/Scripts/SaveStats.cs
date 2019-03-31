using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveStats : MonoBehaviour {

	public static int goodChoice;
	public static int badChoice;
	public static int allChoice;
	public static int lostChoice;
	public static float percentGoodChoice;
	public static string theLastTime;

	public static int answer_attempts;
	public static int right_answers;
	public static int fault_answers;

		
	public static void Save () {
		PlayerPrefs.SetString ("theLastTime", theLastTime);
		PlayerPrefs.SetInt ("GoodChoice", goodChoice);
		PlayerPrefs.SetInt ("BadChoice", badChoice);
		PlayerPrefs.SetInt ("AllChoice", allChoice);
		PlayerPrefs.SetFloat ("PercentGoodChoice", percentGoodChoice);
		PlayerPrefs.SetInt ("LostChoice", lostChoice);
		PlayerPrefs.SetInt ("AnswerAttempts", answer_attempts);
		PlayerPrefs.SetInt ("RightAnswers", right_answers);
		PlayerPrefs.SetInt ("FaultAnswers", fault_answers);
		PlayerPrefs.Save ();
	}

	public static void Load () {
		theLastTime = PlayerPrefs.GetString ("theLastTime");
		goodChoice = PlayerPrefs.GetInt ("GoodChoice");
		badChoice = PlayerPrefs.GetInt ("BadChoice");
		allChoice = PlayerPrefs.GetInt ("AllChoice");
		percentGoodChoice = PlayerPrefs.GetFloat ("PercentGoodChoice");
		lostChoice = PlayerPrefs.GetInt ("LostChoice");
		answer_attempts = PlayerPrefs.GetInt ("AnswerAttempts");
		right_answers = PlayerPrefs.GetInt ("RightAnswers");
		fault_answers = PlayerPrefs.GetInt ("FaultAnswers");
	}

	public static void Check () {
		if (!PlayerPrefs.HasKey ("GoodChoice")) {
			PlayerPrefs.SetInt ("GoodChoice", 0);
		}
		if (!PlayerPrefs.HasKey ("BadChoice")) {
			PlayerPrefs.SetInt ("BadChoice", 0);
		}
		if (!PlayerPrefs.HasKey ("LostChoice")) {
			PlayerPrefs.SetInt ("LostChoice", 0);
		}
		if (!PlayerPrefs.HasKey ("AllChoice")) {
			PlayerPrefs.SetInt ("AllChoice", 0);
		}
		if (!PlayerPrefs.HasKey ("PercentGoodChoice")) {
			PlayerPrefs.SetFloat ("PercentGoodChoice", 0);
		}
		if (!PlayerPrefs.HasKey ("AnswerAttempts")) {
			PlayerPrefs.SetInt ("AnswerAttempts", 0);
		}
		if (!PlayerPrefs.HasKey ("RightAnswers")) {
			PlayerPrefs.SetInt ("RightAnswers", 0);
		}
		if (!PlayerPrefs.HasKey ("FaultAnswers")) {
			PlayerPrefs.SetInt ("FaultAnswers", 0);
		}

	}

}
