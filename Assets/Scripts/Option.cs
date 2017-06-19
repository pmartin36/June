using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public abstract class Option : MonoBehaviour {

	protected bool Selected;
	protected TMP_Text SelectionDisplay;

	// Use this for initialization
	public virtual void Awake () {
		SelectionDisplay = GetComponentsInChildren<TMP_Text>().First(t => t.tag == "OptionSelection");
		Selected = false;
	}

	public virtual void Start () {

	}

	public virtual void SetSelected(bool selected) {
		Selected = selected;
		if (Selected) {
			SelectionDisplay.color = Color.cyan;
		}
		else {
			SelectionDisplay.color = Color.gray;
		}
	}

	public virtual void SelectionPressed() {

	}
}
