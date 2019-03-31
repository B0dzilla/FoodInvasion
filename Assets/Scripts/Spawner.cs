using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public float minTimeBetweenSpawns = 0.5f;
    public float maxTimeBetweenSpawns = 0.5f;
	public int lastSpawnPos = 0;
	public int spawnedFood = 0;
    

    [Range(-4.5f, 4.5f)]
    public float minSpawnHeight = -4.5f;
    [Range(-4.5f, 4.5f)]
    public float maxSpawnHeight = 4.5f;

	[Range(0.0f, 1.0f)]
	public float MovementSpeed = 2.0f;
	float normalMovementSpeed = 0.0f;
	float timerToNormalSpeed = 0.0f;

    public bool removeScoreOnFoodPassage = false;

    public GameObject[] spawnObjects;
    [Range(0.0f, 1.0f)]
    public float[] spawnChance;


    private float screenHalfWidthInWorldsUnits;
    private float totalProbability;
    private float nextSpawnTime = 0.0f;

	public AudioClip spawn_sound;

    private void Start() {
		normalMovementSpeed = MovementSpeed;
        screenHalfWidthInWorldsUnits = Camera.main.aspect * Camera.main.orthographicSize;
        totalProbability = 0;
        for (int i = 0; i < spawnChance.Length; i++) {
            totalProbability += spawnChance[i];
        }
    }
    // Update is called once per frame
    void Update () {
        if (GameManager.gm && GameManager.gm.gameOver) {
            return;
        }
        else {
			if (!GameManager.gm.infinityMode) {
				if (Time.time > nextSpawnTime && GameManager.gm.spawningFoodAmount > spawnedFood) {
					SpawnObject ();
					float timeBetweenSpawns = Random.Range (minTimeBetweenSpawns, maxTimeBetweenSpawns);
					nextSpawnTime = Time.time + timeBetweenSpawns;
				}
			} else {
				if (Time.time > nextSpawnTime) {
					SpawnObject ();
					float timeBetweenSpawns = Random.Range (minTimeBetweenSpawns, maxTimeBetweenSpawns);
					nextSpawnTime = Time.time + timeBetweenSpawns;
				}
			}
        }

		if (!GameManager.gm.infinityMode) {
			if (MovementSpeed != normalMovementSpeed) {
				timerToNormalSpeed += Time.deltaTime;
			}
			if (timerToNormalSpeed >= 5f) {
				MovementSpeed = normalMovementSpeed;
			}
		}
	}

    void SpawnObject() {
        //int index = GetObjectToSpawnIndex();
		int index = Random.Range (0,spawnObjects.Length);
		//Debug.Log (spawnObjects.Length);

		float yPosition = 0;
		int rand = Random.Range (0,3);
		if (rand == lastSpawnPos) { //делаем так что бы не спавнились дважды на одной позиции
			if (rand == 0) {
				rand = 1;
			} else if (rand == 1) {
				rand = 2;
			} else if (rand == 2) {
				rand = 0;
			}
		}
		switch (rand) {
		case 0: 
			yPosition = 2.5f;
			break;
		case 1:
			yPosition = 0;
			break;
		case 2:
			yPosition = -2.5f;	
			break;
		}
		lastSpawnPos = rand;
		spawnedFood++;
		AudioSource.PlayClipAtPoint (spawn_sound, new Vector2(0,0));
        Vector3 spawnPosition = new Vector3(-screenHalfWidthInWorldsUnits, yPosition, transform.position.z);
        GameObject spawnedObject = Instantiate(spawnObjects[index],spawnPosition, Quaternion.identity) as GameObject;

        spawnedObject.transform.localPosition += Vector3.left * spawnedObject.transform.localScale.y;
        spawnedObject.transform.SetParent(transform);
        spawnedObject.GetComponent<MovementPC>().foodInSpeed = MovementSpeed;
        spawnedObject.GetComponent<FoodBehavior>().removeScoreOnScreenPassage = removeScoreOnFoodPassage;
    }

	public void speedUpOnMistake() {
		if (!GameManager.gm.infinityMode) {
			MovementSpeed += 0.015f;
			timerToNormalSpeed = 0f;
		}
	}

    int GetObjectToSpawnIndex() {
        float spawningObjectProbability = Random.Range(0.0f, totalProbability);
        float currentProbability = 0;
        int objectToSpawn = -1;
        for (int i = 0; i < spawnObjects.Length; i++) {
            currentProbability += spawnChance[i];
            if (currentProbability >= spawningObjectProbability) {
                objectToSpawn = i;
                break;
            }
        }
        return objectToSpawn;
    }
}
