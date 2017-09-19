using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeManager : MonoBehaviour {

	public int CurrentDifficulty { get; set; }

	public List<Direction> AllowedSpawnDirections { get; set; }
	public List<DifficultyTypes> DisabledDifficultyTypeIncreases { get; set; }

	//how long between blade spawns
	public float MinimumSpawnCooldown { get; set; }
	public float MaximumSpawnCooldown { get; set; }

	//what percentage of the surface area spawns blades
	public float SpawnSurface { get; set; }
	
	public float MaximumBladeSize { get; set; }

	public bool BlackBladeEnabled { get; set; } 

	//when was the difficulty last changed
	public float LastDifficultySetTime { get; set; }

	private Vector2 camSize;

	private int NumDifficultyTypes;

	public GameObject BladePrefab;
	public Transform BladeContainer;

	private int currentOrderInLayer;

	public Sprite HorizontalBladeSprite;
	public Sprite VerticalBladeSprite;

	//there are numerous blade slide clips and we choose one
	public AudioClip[] bladeSlideAudioClips;

	// Use this for initialization
	void Start () {
		float osize = Camera.main.orthographicSize;
		float w = osize * Camera.main.aspect;
		camSize = new Vector2(w*2, osize*2);

		ResetDifficulty();
		NumDifficultyTypes = Enum.GetNames(typeof(DifficultyTypes)).Length;

		bladeSlideAudioClips = Resources.LoadAll<AudioClip>("Sounds/Slices");

		StartCoroutine(SpawnBlades());
	}

	// Update is called once per frame
	void Update() {

	}

	public void ResetDifficulty() {
		CurrentDifficulty = 1;
		AllowedSpawnDirections = new List<Direction>() {
			(Direction)UnityEngine.Random.Range(0, 4)
		};
		DisabledDifficultyTypeIncreases = new List<DifficultyTypes>();

		MinimumSpawnCooldown = 1.5f;
		MaximumSpawnCooldown = 2.0f;

		SpawnSurface = 0.1f;
		MaximumBladeSize = 0.1f;

		BlackBladeEnabled = false;

		LastDifficultySetTime = Time.time;

		currentOrderInLayer = -100;
	}

	public void IncreaseDifficulty() {
		CurrentDifficulty++;

		//determine how to increase the difficulty
		DifficultyTypes diffincrease;
		
		//if we've increased the difficulty 5 teams without adding a new direction, add a new direction
		if( CurrentDifficulty % 5 == 0 && CurrentDifficulty / 5 >= AllowedSpawnDirections.Count) {
			Debug.Log("Added Direction by Force");
			diffincrease = DifficultyTypes.Direction;
		}
		else {
			do {
				diffincrease = (DifficultyTypes)UnityEngine.Random.Range(0, NumDifficultyTypes);
			} while( DisabledDifficultyTypeIncreases.Contains(diffincrease) );
		}

		Debug.Log("Difficulty Increased: " + Enum.GetName(typeof(DifficultyTypes), diffincrease));

		switch (diffincrease) {
			case DifficultyTypes.Direction:
				//get a new direction that isn't already in the allowed list of directions
				Direction dir;
				do {
					dir = (Direction)UnityEngine.Random.Range(0, 4);
				} while( AllowedSpawnDirections.Contains(dir) );

				AllowedSpawnDirections.Add(dir);

				if(AllowedSpawnDirections.Count >= 4) {
					DisabledDifficultyTypeIncreases.Add(DifficultyTypes.Direction);
				}
				break;
			case DifficultyTypes.Speed:
				MinimumSpawnCooldown *= 0.75f;
				MaximumSpawnCooldown *= 0.75f;
				break;
			case DifficultyTypes.Area:
				SpawnSurface += 0.1f;
				MaximumBladeSize = 1f / (2f * SpawnSurface); 

				if(SpawnSurface >= 1.0f) {
					DisabledDifficultyTypeIncreases.Add(DifficultyTypes.Area);
				}
				break;
			case DifficultyTypes.BlackBlade:
				BlackBladeEnabled = true;
				DisabledDifficultyTypeIncreases.Add(DifficultyTypes.BlackBlade);
				break;
		}
	}

	IEnumerator SpawnBlades() {
		//how many points will surviving this round grant
		int bladeScore = 0;

		while (true) {
			float spawnPosition = Mathf.Round(UnityEngine.Random.Range(0,0.9f) * 10f) / 10f;

			//grant points for surviving previous round of blades
			GameManager.Instance.scoreManager.IncrementScore(bladeScore);
			bladeScore = 0;		

			//if SpawnSurface is greater than 1, we'll do 2 passes of 
			BladeInfo[] spawnBlade = new BladeInfo[10];

			float initTime = Time.time;
			AudioClip clip = bladeSlideAudioClips[UnityEngine.Random.Range(0, bladeSlideAudioClips.Length - 1)];
			List<Blade> blades = new List<Blade>();

			foreach (Direction d in AllowedSpawnDirections) {
				//determine which blade positions are spawning
				bool leftright = d == Direction.Right || d == Direction.Left;
				float i = leftright ? 1f : 0.5f;

				bool previousCreated = false;
				float size = 0f;

				BladeInfo bi = new BladeInfo() { Size = 0.1f, BladeDirection = d, InitTime = initTime };
				bi.OrderInLayer = leftright ? currentOrderInLayer : currentOrderInLayer + 1;
				bi.AudioClip = clip;

				for (; i < 9; i++) {
					if(UnityEngine.Random.value <= SpawnSurface && size < MaximumBladeSize) {
						if(!previousCreated) {
							int colorOptions = BlackBladeEnabled ? 6 : 5;
							bi.BladeColor = (GameColors)UnityEngine.Random.Range(1, colorOptions);
							size = 0f;
						}

						bi.Position = i / 10f;

						Blade b = Instantiate(BladePrefab, BladeContainer).GetComponent<Blade>();
						b.Init(bi, camSize);

						blades.Add(b);

						bladeScore += bi.BladeColor == GameColors.black ? 2 : 1;

						size += 0.1f;
						previousCreated = true;
					}
					else {
						previousCreated = false;
					}
				}
			}

			foreach(Blade b in blades) {
				//if we're currently muting sfx, mute this blade
				if( !GameManager.Instance.settings.SFX ) {
					b.audioPlayer.mute = true;
				}
				//scale volume based upon number of blades we're spawning
				b.DefaultVolume =  1f / ((float)blades.Count * 2);
				//set volume depending on whether the menu is open or not
				b.audioPlayer.volume = GameManager.Instance.MenuOpen ? Mathf.Min(0.1f, b.DefaultVolume) : b.DefaultVolume;
			}

			//decrement order in layer so that the newest blades can always stay on top
			//acceptable range is -100 to 100 so when we hit -100, we add 150 to reset to +50
			currentOrderInLayer+=2;
			if (currentOrderInLayer > 100) {
				foreach (GameObject o in GameObject.FindGameObjectsWithTag("Blade")) {
					o.GetComponent<SpriteRenderer>().sortingOrder -= 150;
				}
				currentOrderInLayer -= 150;
			}

			//increment difficulty every 10 seconds
			if (Time.time - LastDifficultySetTime > 10f) {
				if(GameManager.Instance.GameActive) {
					IncreaseDifficulty();
				}
				LastDifficultySetTime = Time.time;			
			}

			//if no blades were spawned, speed up the next round of blade spawns
			//otherwise, pick a random number between minimum and maximum blade spawn times
			float waitTime = bladeScore == 0 ? MinimumSpawnCooldown / 4f : UnityEngine.Random.Range(MinimumSpawnCooldown, MaximumSpawnCooldown);
			yield return new WaitForSeconds(waitTime);
		}
	}
}
