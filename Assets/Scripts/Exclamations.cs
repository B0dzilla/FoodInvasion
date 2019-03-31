using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exclamations : MonoBehaviour {
	float acolor;
	public float fadeSpeed = 1f;
	public float delay = 5;
	float lifeTime = 0f;
	Color _color;

	// Use this for initialization
	void Start () {
	_color = gameObject.GetComponent <SpriteRenderer>().color;
	}
	
	// Update is called once per frame
	void Update () {
		if (lifeTime < delay) {
			lifeTime += Time.deltaTime;
		} else {
			_color.a -= Time.deltaTime * (fadeSpeed/10);
			acolor = _color.a;
			gameObject.GetComponent <SpriteRenderer>().color = _color;
			if (acolor <= 0) {
				Destroy(gameObject);
			}
		}
	}
}
