using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerCreator : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		GameManager gm = GameManager.Instance;
		Destroy(this.gameObject);
	}
}
