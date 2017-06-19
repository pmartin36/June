using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuManager : MonoBehaviour {

	public bool InMenu {
		get {
			return Index >= 0;
		}
	}
	public List<Menu> menus;
	public int Index { get; set; }
	public Menu ActiveMenu {
		get {
			return menus[Index];
		}
	}

	public Camera menuCamera;

	// Use this for initialization
	void Start () {
		menuCamera.gameObject.SetActive(true);
		ActivateMenu(MenuTypes.Start);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ExitMenu() {
		ActiveMenu.Deactivate();
		Index = -1;

		menuCamera.gameObject.SetActive(false);
	}

	public void EnterMenu(MenuTypes type) {
		menuCamera.gameObject.SetActive(true);
		ActivateMenu(type);
	}

	public void ActivateMenu(MenuTypes m) {
		Index = (int)m;
		for(int i = 0; i < menus.Count; i++) {
			if( i == Index) {
				menus[i].Activate();
			}
			else {
				menus[i].Deactivate();
			}
		}
	}

	public void ProcessInputs(InputPackage p) {
		if(p.Pause) {
			switch ((MenuTypes)Index) {
				case MenuTypes.Pause:
					GameManager.Instance.ExitMenu();
					break;
				case MenuTypes.Start:
					//Quit Game
					Application.Quit();
					break;
				case MenuTypes.Score:
					//Exit to main menu
					GameManager.Instance.ToMainMenu(true);
					break;
			}
		}
		else {
			ActiveMenu.ProcessInputs(p);
		}
	}
}
