using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu {

	List<Image> title;

	public override void Awake() {
		base.Awake();
		GameObject titleParent = GameObject.FindGameObjectWithTag("Title");
		title = titleParent.GetComponentsInChildren<Image>().OrderByDescending( t => t.GetComponent<RectTransform>().position.y ).ToList();
	}

	// Use this for initialization
	void OnEnable() {
	}

	void OnDisable() {
	}

	public IEnumerator ShowTitle() {
		float journeyTime = 0.3f;

		List<RectTransform> rts = new List<RectTransform>();
		List<Vector3> startPositions = new List<Vector3>();
		List<Vector3> endPositions = new List<Vector3>();

		for (int i = 0; i < 3; i++) {	
			RectTransform rt = title[i].GetComponent<RectTransform>();
			rts.Add(rt);		
			endPositions.Add(rt.localPosition);

			rt.localPosition = new Vector3((i % 2 == 0 ? -1f : 1f) * 600, rt.localPosition.y, 0);

			startPositions.Add(rt.localPosition);
		}

		for (int i = 0; i < 3; i++) {
			float startTime = Time.time;

			RectTransform rt = title[i].GetComponent<RectTransform>();		
			Vector3 startPosition = startPositions[i];
			Vector3 endPosition = endPositions[i];

			if(GameManager.Instance.settings.SFX)
				rt.gameObject.GetComponent<AudioSource>().Play();

			while (Time.time - startTime < journeyTime + Time.deltaTime) {
				float jtime = (Time.time - startTime) / journeyTime;

				//blade animation
				rts[i].localPosition = Vector3.Lerp(startPosition, endPosition, jtime);

				yield return new WaitForFixedUpdate();
			}
		}
	}
}

