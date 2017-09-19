using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directional : MonoBehaviour {
	public Transform PlayerTransform { get; set; }

	/* FOR TRAILING DIRECTIONAL
	float positionTime;
	Vector3 targetPosition;
	Vector3 sourcePosition;
	float journeyTime = 0.1f;

	void Update () {	
		if(Time.time - positionTime > journeyTime + Time.deltaTime) {
			sourcePosition = transform.position;
			targetPosition = PlayerTransform.position + offset;
			positionTime = Time.time;
		}

		float tdiff = (Time.time - positionTime) / journeyTime;
		transform.position = Vector3.Lerp(sourcePosition, targetPosition, tdiff);
	}
	*/

	Vector3 offset;

	void Start() {
		offset = transform.position;
		//positionTime = Time.time;
	}

	void Update() {
		transform.position = PlayerTransform.position + offset;
	}
}
