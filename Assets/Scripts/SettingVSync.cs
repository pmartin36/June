using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingVSync : Setting {

	public override void Awake() {
		base.Awake();
	}

	public void OnEnable() {
		//On = GameManager.Instance.settings.Fullscreen;
		//SetStateText();
	}


	public override void SetSelected(bool selected) {
		base.SetSelected(selected);

		//custom logic

	}

	public override void SelectionPressed() {
		base.SelectionPressed();

		//custom logic

	}
}
