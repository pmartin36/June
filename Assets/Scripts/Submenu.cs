using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Submenu : MonoBehaviour {

	public bool InMenu { 
		get{
			return Index >= 0;
		}
	}

	//-1 is no Options currrently selected
	public int Index { get; set; }
	protected List<Option> Options;

	void Awake() {
		Options = GetComponentsInChildren<Option>().OrderByDescending(o => o.transform.position.y).ToList();
	}

	void Start() {
		
	}

	public virtual void ChangeIndex(int diff) {
		//the indices go up as the selection gets lower
		//diff is negated for this reason
		int newIndex = (Index - diff) % Options.Count;
		newIndex = newIndex < 0 ? newIndex + Options.Count : newIndex;
		SetIndex(newIndex);
	}

	public virtual void SetIndex(int index) {
		Index = index;
		SetSelected();
	}

	public virtual void SetSelected() {
		for (int i = 0; i < Options.Count; i++) {
			Options[i].SetSelected(Index == i);
		}
	}

	public virtual void ActivateMenu() {
		SetIndex(0);
	}

	public virtual void DeactivateMenu() {
		SetIndex(-1);
	}

	public virtual void ProcessInputs(InputPackage p) {
		//process vertical inputs
		if (p.Up) {
			ChangeIndex(1);
		}
		else if (p.Down) {
			ChangeIndex(-1);
		}
		else if (p.Select) {
			//process selected
			Options[Index].SelectionPressed();
		}
	}
}
