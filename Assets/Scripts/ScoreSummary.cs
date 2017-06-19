using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ScoreSummary : MonoBehaviour {

	private TMP_Text newRecordText;
	private TMP_Text highScoreText;
	private TMP_Text playerScoreText;

	void Awake() {
		TMP_Text[] texts = GetComponentsInChildren<TMP_Text>().Where( t => t.tag == "OptionState").OrderByDescending( t => t.transform.position.y).ToArray();
		if(texts.Length == 3) {
			newRecordText = texts[0];
			playerScoreText = texts[1];
			highScoreText = texts[2];
		}
	}

	void OnEnable() {
		int score = GameManager.Instance.scoreManager.CurrentScore;
		int highScore = GameManager.Instance.scoreManager.HighScore;

		if(score >= highScore) {
			newRecordText.color = Color.white;
		}
		else {
			newRecordText.color = Color.clear;
		}

		highScoreText.text = highScore.ToString();
		playerScoreText.text = score.ToString();
	}
}
