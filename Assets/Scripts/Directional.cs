using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directional : MonoBehaviour {
	public Transform PlayerTransform { get; set; }

	float positionTime;
	Vector3 targetPosition;
	Vector3 sourcePosition;
	float journeyTime = 0.1f;

	void Start() {
		positionTime = Time.time;
	}

	// Update is called once per frame
	void Update () {	
		if(Time.time - positionTime > journeyTime + Time.deltaTime) {
			sourcePosition = transform.position;
			targetPosition = PlayerTransform.position;
			positionTime = Time.time;
		}

		float tdiff = (Time.time - positionTime) / journeyTime;
		transform.position = Vector3.Lerp(sourcePosition, targetPosition, tdiff);
	}
}
