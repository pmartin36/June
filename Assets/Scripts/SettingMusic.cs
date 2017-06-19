using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMusic : Setting {

	public override void Awake() {
		base.Awake();
	}

	public void OnEnable() {
		On = GameManager.Instance.settings.Music;
		SetStateText();
	}

	public override void SetSelected(bool selected) {
		base.SetSelected(selected);

		//custom logic

	}

	public override void SelectionPressed() {
		base.SelectionPressed();

		//custom logic
		GameManager.Instance.settings.Music = On;
		GameManager.Instance.settings.Save();

		if(On) {
			GameManager.Instance.InGameMusic.mute = false;
			GameManager.Instance.MenuMusic.mute = false;
		}
		else {
			GameManager.Instance.InGameMusic.mute = true;
			GameManager.Instance.MenuMusic.mute = true;
		}
	}
}
