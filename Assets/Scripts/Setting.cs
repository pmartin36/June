using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public abstract class Setting: Option {

	protected bool On;
	TMP_Text StateDisplay;

	public override void Awake() {
		base.Awake();
		StateDisplay = GetComponentsInChildren<TMP_Text>().First(t => t.tag == "OptionState");
		On = true;
	}

	public override void SetSelected (bool selected) {
		base.SetSelected(selected);

	}
	public override void SelectionPressed () {
		On = !On;
		SetStateText();
	}

	public virtual void SetStateText() {
		if (On) {
			StateDisplay.text = "On";
		}
		else {
			StateDisplay.text = "Off";
		}
	}

}
