using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class OptionDifficulty : Option {

	public static int Difficulty { get; set; }
	public static int MaxDifficulty { get; set; }

	TMP_Text DifficultyText;

	public override void Awake() {
		base.Awake();
		DifficultyText = GetComponentsInChildren<TMP_Text>().First(t => t.tag == "OptionState");
		Selected = false;

		Difficulty = 1;
		MaxDifficulty = 10;
	}

	public void OnEnable() {
		DifficultyText.text = Difficulty.ToString();
	}

	public override void Start() {
		base.Start();
	}

	public override void SelectionPressed() {
		base.SelectionPressed();

		Difficulty++;
		if(Difficulty > MaxDifficulty) {
			Difficulty = 1;
			GameManager.Instance.bladeManager.ResetDifficulty();
		}
		else {
			GameManager.Instance.bladeManager.IncreaseDifficulty();
		}

		DifficultyText.text = Difficulty.ToString();
	}
}
