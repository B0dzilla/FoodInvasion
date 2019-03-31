using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBehavior : MonoBehaviour {

    public int scoreAmount = 0;
    public bool removeScoreOnScreenPassage = false;

    private float screenHalfWidthInWorldsUnits;

	public AudioClip right_choise_snd;
	public AudioClip fault1_choise_snd;
	public AudioClip fault2_choise_snd;
	public AudioClip fault3_choise_snd;
	public AudioClip kick_good_food;
	public AudioClip miss_choise_snd;

    private void Start() {
        screenHalfWidthInWorldsUnits = Camera.main.aspect * Camera.main.orthographicSize;
    }
    // Update is called once per frame
    void Update () {
        if (GameManager.gm && GameManager.gm.gameOver) {
            return;
        }
		if (transform.localPosition.x > (screenHalfWidthInWorldsUnits + transform.localScale.x / 2)+1f) {
			AudioSource.PlayClipAtPoint (miss_choise_snd, new Vector2(0,0));
			GameManager.gm.rightChoiseSeries = 0;
            if (removeScoreOnScreenPassage) {
				if (GameManager.gm) {
                    GameManager.gm.foodPulled(-Mathf.Abs(scoreAmount));
                }
            }
			GameManager.gm.statsLostFood += 1;
			SaveStats.lostChoice += 1;
			SaveStats.allChoice += 1;
			GameManager.gm.removeIndicatorPoint ();
			GameManager.gm.usedFood += 1;
            Destroy(gameObject);
        }

        if (transform.localPosition.y > Camera.main.orthographicSize + transform.localScale.y / 2) {
            if (GameManager.gm) {
                GameManager.gm.foodPulled(-scoreAmount);
				if (scoreAmount > 0) {
					GameManager.gm.statsBadFood += 1;
					SaveStats.badChoice += 1;
					SaveStats.allChoice += 1;
					Handheld.Vibrate();

					AudioSource.PlayClipAtPoint (kick_good_food, new Vector2 (0, 0));

					//int rand = Random.Range (0, 3);
					//switch (rand) {
					//case 0:
					//	AudioSource.PlayClipAtPoint (fault1_choise_snd, new Vector2 (0, 0));
					//	break;
					//case 1:
					//	AudioSource.PlayClipAtPoint (fault2_choise_snd, new Vector2 (0, 0));
					//	break;
					//case 2:
					//	AudioSource.PlayClipAtPoint (fault3_choise_snd, new Vector2 (0, 0));
					//	break;
					//}
					GameManager.gm.rightChoiseSeries = 0;
					GameManager.gm.removeIndicatorPoint ();
					GameManager.gm.usedFood += 1;
				} else {
					GameManager.gm.statsGoodFood += 1;
					SaveStats.goodChoice += 1;
					GameManager.gm.amountScoreForInfinity += 1;
					GameManager.gm.counterForInfinity += 1;
					SaveStats.allChoice += 1;
					AudioSource.PlayClipAtPoint (right_choise_snd, new Vector2 (0, 0));
					GameManager.gm.rightChoiseSeries += 1;
					GameManager.gm.addIndicatorPoint ();
					GameManager.gm.usedFood += 1;
				}
                Destroy(gameObject);
            }
        }
        else if (transform.localPosition.y < -(Camera.main.orthographicSize + transform.localScale.y / 2)) {
            if (GameManager.gm) {
                GameManager.gm.foodPulled(scoreAmount);
				if (scoreAmount > 0) {
					GameManager.gm.statsGoodFood += 1;
					SaveStats.goodChoice += 1;
					GameManager.gm.counterForInfinity += 1;
					GameManager.gm.amountScoreForInfinity += 1;
					SaveStats.allChoice += 1;
					AudioSource.PlayClipAtPoint (right_choise_snd, new Vector2 (0, 0));
					GameManager.gm.rightChoiseSeries += 1;
					GameManager.gm.addIndicatorPoint ();
					GameManager.gm.usedFood += 1;
				} else {
					GameManager.gm.statsBadFood += 1;
					SaveStats.badChoice += 1;
					SaveStats.allChoice += 1;
					int rand = Random.Range (0, 3);
					Handheld.Vibrate();
					switch (rand) {
					case 0:
						AudioSource.PlayClipAtPoint (fault1_choise_snd, new Vector2 (0, 0));
						break;
					case 1:
						AudioSource.PlayClipAtPoint (fault2_choise_snd, new Vector2 (0, 0));
						break;
					case 2:
						AudioSource.PlayClipAtPoint (fault3_choise_snd, new Vector2 (0, 0));
						break;
					}
					GameObject.Find("FoodSpawner").GetComponent<Spawner>().speedUpOnMistake();
					GameManager.gm.rightChoiseSeries = 0;
					GameManager.gm.removeIndicatorPoint ();
					GameManager.gm.usedFood += 1;
				}
                Destroy(gameObject);
            }
        }
    }

	void OnDestroy () {
		SaveStats.Save ();
	}
}
