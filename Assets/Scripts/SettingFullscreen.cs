using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class SettingFullscreen : Setting {

	int resolutionX, resolutionY;

	public override void Awake() {
		base.Awake();
		resolutionX = 1600;
		resolutionY = 900;
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
			resolutionX = Screen.width;
			resolutionY = Screen.height;
			Screen.SetResolution(Screen.width, Screen.height, true);
		}
		else {
			Screen.SetResolution(resolutionX, resolutionY, false);
		}
	}
}
