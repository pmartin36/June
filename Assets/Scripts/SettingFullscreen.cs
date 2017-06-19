using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class SettingFullscreen : Setting {

	public override void Awake() {
		base.Awake();
	}

	public void OnEnable() {
		On = GameManager.Instance.settings.Fullscreen;
		SetStateText();
	}

	public override void SetSelected(bool selected) {
		base.SetSelected(selected);

		//custom logic

	}

	public override void SelectionPressed() {
		base.SelectionPressed();

		GameManager.Instance.settings.Fullscreen = On;
		GameManager.Instance.settings.Save();

		if(On) {
			Screen.SetResolution(Screen.width, Screen.height, true);
		}
		else {
			Screen.SetResolution(1280, 720, false);
		}
	}
}
