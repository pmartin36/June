using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSFX : Setting {

	public override void Awake() {
		base.Awake();
	}

	public void OnEnable() {
		On = GameManager.Instance.settings.SFX;
		SetStateText();
	}

	public override void SetSelected(bool selected) {
		base.SetSelected(selected);

		//custom logic

	}

	public override void SelectionPressed() {
		base.SelectionPressed();

		GameManager.Instance.settings.SFX = On;
		GameManager.Instance.settings.Save();

		GameObject[] b = GameObject.FindGameObjectsWithTag("Blade");
		if(On) {
			foreach(GameObject o in b) {
				AudioSource s = o.GetComponent<AudioSource>();
				s.mute = false;
			}
		}
		else {
			foreach (GameObject o in b) {
				AudioSource s = o.GetComponent<AudioSource>();
				s.mute = true;
			}
		}
	}
}
