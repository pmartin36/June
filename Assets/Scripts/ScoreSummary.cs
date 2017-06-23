using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ScoreSummary : MonoBehaviour {

	private TMP_Text newRecordText;
	private TMP_Text highScoreText;
	private TMP_Text playerScoreText;

	private RectTransform scoreDetail;
	private Light light;

	void Awake() {
		TMP_Text[] texts = GetComponentsInChildren<TMP_Text>().Where( t => t.tag == "OptionState").OrderByDescending( t => t.transform.position.y).ToArray();
		if(texts.Length == 3) {
			newRecordText = texts[0];
			playerScoreText = texts[1];
			highScoreText = texts[2];

			scoreDetail = highScoreText.transform.parent.parent.GetComponent<RectTransform>();
		}

		light = GetComponentInChildren<Light>();
	}

	void OnEnable() {
		int score = GameManager.Instance.scoreManager.CurrentScore;
		int highScore = GameManager.Instance.scoreManager.HighScore;

		if(score >= highScore) {
			newRecordText.color = Color.white;
			scoreDetail.localPosition = new Vector3( scoreDetail.localPosition.x, 28.6f);
		}
		else {
			newRecordText.color = Color.clear;
			scoreDetail.localPosition = new Vector3(scoreDetail.localPosition.x, 48.6f);
		}

		highScoreText.text = highScore.ToString();
		playerScoreText.text = score.ToString();

		StartCoroutine(ChangeLightColor());
	}

	private void OnDisable() {
		StopCoroutine(ChangeLightColor());
	}

	IEnumerator ChangeLightColor(){
		List<Color> colorchoices = new List<Color>(){
			new Color(0.5f, 0.05f, 0.05f, 1),
			new Color(0.05f, 0.5f, 0.05f, 1),
			new Color(0.05f, 0.05f, 0.5f, 1),
			new Color(0.5f, 0.5f, 0.05f, 1),
		};

		int lastIndex = UnityEngine.Random.Range(0, colorchoices.Count - 1);
		light.color = colorchoices[lastIndex];
		float journeyTime = 2f;

		while(true){
			float startTime = Time.time;

			int index;
			do{
				index = UnityEngine.Random.Range(0, colorchoices.Count - 1);
			} while( index == lastIndex );

			Color startColor = light.color;
			Color endColor = colorchoices[index];
			lastIndex = index;

			while (Time.time - startTime < journeyTime + Time.deltaTime){
				float jtime = (Time.time - startTime) / journeyTime;
				light.color = Color.Lerp(startColor, endColor, jtime);
				yield return new WaitForEndOfFrame();
			}	
		}
	}
}
