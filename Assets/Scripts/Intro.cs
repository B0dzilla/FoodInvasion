using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {
    public Toggle[] textToggles;
    public Text[] texts;
    public GameObject introHolder;
    public GameObject loginHolder;
    public GameObject choseCharacterHolder;
    public GameObject registrationHolder;
    public GameObject enterHolder;
    public GameObject areYouSureHolder;
	public GameObject introCanvas;

    public float animationTime = 0.8f;
    private int activeTextToggle;
    private bool isMoving = false;
	// Use this for initialization
	void Start () {
		introCanvas.SetActive (false);
		PlayerPrefs.DeleteKey ("opening_questionLevel");
		SceneManager.LoadScene("Menu");
        //activeTextToggle = 0;
        //for (int i = 0; i < textToggles.Length; i++) {
        //    textToggles[i].isOn = i == activeTextToggle;
        //    texts[i].gameObject.SetActive(i == activeTextToggle);
        //}
	}

    //Button functions
    public void SkipIntro() {
        if (!isMoving) {
            StartCoroutine(TwoUIAnimations(0, -800, Vector3.right, introHolder, 800, 0, Vector3.right, loginHolder, 0.3f));
        }
    }

    public void NextText() {
        if (activeTextToggle < textToggles.Length - 1) {
            activeTextToggle++;
            for (int i = 0; i < textToggles.Length; i++) {
                textToggles[i].isOn = i == activeTextToggle;
                texts[i].gameObject.SetActive(i == activeTextToggle);
            }
        }
        else {
            SkipIntro();
        }
    }
    
    public void Registration() {
        if (!isMoving) {
            StartCoroutine(TwoUIAnimations(0, -1420, Vector3.up, enterHolder, -1420, 0, Vector3.up, registrationHolder, 0.6f));
        }
    }

    public void CancelRegistration() {
        if (!isMoving) {
            StartCoroutine(TwoUIAnimations(0, -1420, Vector3.up, registrationHolder, -1420, 0, Vector3.up, enterHolder, 0.6f));
        }
    }

    public void CompleteRegistration() {
        //Code to verify registration
        CancelRegistration();
    }

    public void Login() {
        //Code to verify login information
        if (!isMoving) {
            StartCoroutine(TwoUIAnimations(0, -800, Vector3.right, loginHolder, 800, 0, Vector3.right, choseCharacterHolder, 0.3f));
        }
    }

    public void VerifyChoice() {
        if (!isMoving) {
            StartCoroutine(OneUIAnimation(-1420, 0, Vector3.up, areYouSureHolder));
        }
    }

    public void ChoiceVerified() {
        if (!isMoving) {
            StartCoroutine(OneUIAnimation(0, -1420, Vector3.up, areYouSureHolder));
        }
    }

    public void LoadMenu() {
        SceneManager.LoadScene("Menu");
    }

    //Animations
    IEnumerator OneUIAnimation(float startValue, float endValue, Vector3 direction, GameObject uiElement) {
        float percent = 0;
        float animationSpeed = 1 / animationTime;
        while (percent < 1) {
            isMoving = true;
            percent += Time.deltaTime * animationSpeed;
            float interpolation = Mathf.Sin((Mathf.PI * percent) / 2);
            uiElement.transform.localPosition = direction * Mathf.Lerp(startValue, endValue, interpolation);
            yield return null;
        }
        isMoving = false;
    }

    IEnumerator TwoUIAnimations(float startValue, float endValue, Vector3 direction, GameObject uiElement, float secondStartValue, float secondEndValue, Vector3 secondDirection, GameObject secondUIElement, float secondAnimationPercent) {
        float percent = 0;
        float animationSpeed = 1 / animationTime;
        bool secondAnimationStarted = false;
        while (percent < 1) {
            isMoving = true;
            if (percent >= secondAnimationPercent && !secondAnimationStarted) {
                secondAnimationStarted = true;
                StartCoroutine(OneUIAnimation(secondStartValue, secondEndValue, secondDirection, secondUIElement));
            }
            percent += Time.deltaTime * animationSpeed;
            float interpolation = Mathf.Sin((Mathf.PI * percent) / 2);
            uiElement.transform.localPosition = direction * Mathf.Lerp(startValue, endValue, interpolation);
            yield return null;
        }
        isMoving = false;
    }
}
