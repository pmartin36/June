using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Menu : MonoBehaviour {

	public int Index { get; set; }
	List<Submenu> submenus;

	public Submenu ActiveSubmenu {
		get {
			return submenus[Index];
		}
	}

	// Use this for initialization
	void Awake () {
		submenus = GetComponentsInChildren<Submenu>().OrderBy( g => g.transform.position.x ).ToList();		
	}

	void Start() {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Activate() {
		this.gameObject.SetActive(true);
		SetActiveSubmenu(0);
	}

	public void Deactivate() {
		this.gameObject.SetActive(false);
	}

	public void SetActiveSubmenu(int i) {
		Index = i;	
		foreach(Submenu s in submenus.Where( m => m.InMenu )) {
			s.DeactivateMenu();
		}
		ActiveSubmenu.ActivateMenu();
	}

	public void ProcessInputs(InputPackage p) {
		if(p.Left) {
			int newIndex = Index - 1;
			newIndex = newIndex < 0 ? 0 : newIndex;
			SetActiveSubmenu( newIndex ); 
		}
		else if(p.Right) {
			int newIndex = Index + 1;
			newIndex = newIndex >= submenus.Count ? submenus.Count - 1 : newIndex;
			SetActiveSubmenu(newIndex);
		}
		else {
			ActiveSubmenu.ProcessInputs(p);
		}
	}
}
