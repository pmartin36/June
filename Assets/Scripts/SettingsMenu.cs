using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingsMenu : MonoBehaviour {

	//-1 is no Options currrently selected
	public int Index { get; set; }
	List<Setting> Settings;

	// Use this for initialization
	void Awake () {
		Settings = GetComponentsInChildren<Setting>().OrderByDescending( o => o.transform.position.y ).ToList();	
	}

	void Start() {
		SetIndex(-1);
	}
	
	public void SelectPressed() {
		if(Index >= 0) {
			Settings[Index].SelectionPressed();
		}
	}

	public void ChangeIndex (int diff) {
		//the indices go up as the selection gets lower
		//diff is negated for this reason
		int newIndex = (Index - diff) % Settings.Count;
		newIndex = newIndex < 0 ? newIndex + Settings.Count : newIndex;
		SetIndex(newIndex);
	}

	public void SetIndex(int index) {
		Index = index;
		SetSelected();
	}

	public void SetSelected() {
		for (int i = 0; i < Settings.Count; i++) {
			Settings[i].SetSelected(Index == i);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
