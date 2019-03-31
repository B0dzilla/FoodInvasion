using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPC : MonoBehaviour {

    [Range(0.0f, 1.0f)]
    public float swipePercent = 0.15f;
    public float foodInSpeed = 2.0f;
    public float foodOutSpeed = 2.0f;

    private float dragDistance;
    
	private bool foodSelected = false;
    private bool movingOut = false;
    private float startPosition;
    private float currentPosition;

	public AudioClip move_sound;

	// Use this for initialization
	void Start () {
        dragDistance = Screen.height * swipePercent;
        StartCoroutine(MoveFoodIn());
    }

    void OnMouseDown() {
        if (!foodSelected) {
            foodSelected = true;
            if (GameManager.gm && GameManager.gm.isMobile) {
                startPosition = Input.GetTouch(0).position.y;
                currentPosition = Input.GetTouch(0).position.y;
            }
            else {
                startPosition = Input.mousePosition.y;
                currentPosition = Input.mousePosition.y;
            }
        }
    }
    private void OnMouseUp() {
        if (foodSelected) {
            if (GameManager.gm && GameManager.gm.isMobile) {
                currentPosition = Input.GetTouch(0).position.y;
            }
            else {
                currentPosition = Input.mousePosition.y;
            }
            float offset = currentPosition - startPosition;
            if (Mathf.Abs(offset) > dragDistance&&!movingOut) {
                StartCoroutine(MoveFoodOut(Mathf.Sign(offset)));
            }
            foodSelected = false;
        }
    }

    IEnumerator MoveFoodOut(float direction) {
        movingOut = true;
		AudioSource.PlayClipAtPoint (move_sound, new Vector2(0,0));
        float startPosition = transform.position.y;
        float endPosition = direction * (Camera.main.orthographicSize + transform.localScale.y);
        float percent = 0;
        float movementSpeed = 1/foodOutSpeed;
        while (percent < 1) {
            percent += Time.deltaTime * movementSpeed;
            float interpolation = Mathf.Sin((Mathf.PI * percent) / 2);
			transform.localPosition = Vector3.up * Mathf.Lerp (startPosition, endPosition, interpolation) + Vector3.right*transform.localPosition.x;
            yield return null;
        }
    }

    IEnumerator MoveFoodIn() {
        float percent = 0;
        float movementSpeed = foodInSpeed;
        while (percent < 10) {
            if (movingOut) {
                break;
            }
			if (!MainMenu.isPaused) {
				transform.Translate (new Vector2 (movementSpeed, 0));
				percent += Time.deltaTime;
			}
            yield return null;
        }
	}
}
