using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

public class ScoreManager : MonoBehaviour {

	private TMP_Text scoreText;
	private TMP_Text scoreValueText;

	private TMP_Text highScoreText;
	private TMP_Text highScoreValueText;

	GameObject panel;

	public int CurrentScore { get; private set; }
	public int HighScore { get; private set; }

	TMP_Text [] texts;

	// Use this for initialization
	void Awake () {
		texts = GetComponentsInChildren<TMP_Text>().OrderBy( g => g.transform.position.x ).ToArray();
		scoreText = texts[0];
		scoreValueText = texts[1];
		
		highScoreValueText = texts[2];
		highScoreText = texts[3];

		foreach(Transform child in transform){
			if(child.tag == "ScorePanel"){
				panel = child.gameObject;
			}
		}
	}

	void Start() {
		HighScore = GameManager.Instance.settings.HighScore;
		highScoreValueText.text = HighScore.ToString();
	}

	public void Show() {
		panel.SetActive(true);
		//foreach(TMP_Text t in texts) {
		//	t.color = Color.white;
		//}
	}

	public void Hide() {
		panel.SetActive(false);
		//foreach (TMP_Text t in texts) {
		//	t.color = Color.clear;
		//}
	}

	public void ClearScore() {
		CurrentScore = 0;
		scoreValueText.text = CurrentScore.ToString();
	}

	//only increment score if the player is active in the scene
	public void IncrementScore(int add) {
		if(GameManager.Instance.GameActive) {
			CurrentScore += add;
			scoreValueText.text = CurrentScore.ToString();
			if(CurrentScore > HighScore) {
				HighScore = CurrentScore;
				highScoreValueText.text = HighScore.ToString();
			}
		}
	}

	/// <summary>
	/// Update high score if current score is higher than the high score
	/// </summary>
	public void FinalizeScore() {
		if(CurrentScore > GameManager.Instance.settings.HighScore) {
			GameManager.Instance.settings.HighScore = HighScore;
			GameManager.Instance.settings.Save();
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
