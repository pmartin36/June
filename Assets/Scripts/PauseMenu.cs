using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

	public bool InMenu { 
		get {
			return Index >= 0;
		}
	}

	//-1 means the selector is in the settings menu
	public int Index { get; set; }

	List<Option> options;
	SettingsMenu settingsMenu;

	// Use this for initialization
	void Awake () {
		options = GetComponentsInChildren<Option>().Where(g => g.transform.parent == this.transform).OrderByDescending(g => g.transform.position.y).ToList();
		settingsMenu = GetComponentInChildren<SettingsMenu>();
	}
	
	public void ProcessInputs(InputPackage p) {
		if(p.Horizontal > 0.2f && InMenu) {
			//switch to settings menu
			SetIndex(-1);
			settingsMenu.SetIndex(0);
		}
		else if(p.Horizontal < -0.2f && !InMenu) {
			//switch to this menu
			SetIndex(0);
			settingsMenu.SetIndex(-1);
		}
		else {
			//process vertical inputs
			if(p.Up) {
				if (InMenu) {
					ChangeIndex(1);
				}
				else {
					settingsMenu.ChangeIndex(1);
				}
			}
			else if(p.Down) {
				if (InMenu) {
					ChangeIndex(-1);
				}
				else {
					settingsMenu.ChangeIndex(-1);
				}
			}

			//process selected
			if(p.Select) {
				if(InMenu) {
					options[Index].SelectionPressed();
				}
				else {
					settingsMenu.SelectPressed();
				}
			}
		}
	}

	public void ChangeIndex(int diff) {
		//the indices go up as the selection gets lower
		//diff is negated for this reason
		int newIndex = (Index - diff) % options.Count;
		newIndex = newIndex < 0 ? newIndex + options.Count : newIndex;
		SetIndex(newIndex);
	}

	public void SetIndex(int index) {
		Index = index;

		if(Index >= 0 && settingsMenu.Index >= 0)
			settingsMenu.SetIndex(-1);

		for (int i = 0; i < options.Count; i++) {
			options[i].SetSelected(i == Index);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
