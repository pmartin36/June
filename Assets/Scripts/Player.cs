using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour {

	private float horizontal;
	private float vertical;

	private float velxSmoothing;
	private float velySmoothing;

	private float moveSpeed = 0.35f;

	private Bounds bounds;

	private List<SpriteRenderer> spriteRenderers;

	private BoxCollider2D box;

	public bool Impaled { get; set; }
	public GameObject playerTop;

	public GameObject directionalPrefab;
	private Directional directional;

	public GameColors PlayerColor { get; set; }
	Color fillColor;

	// Use this for initialization
	void Start () {
		float osize = Camera.main.orthographicSize;
		float w = osize * Camera.main.aspect;
		float pokeDistanceX = w * 2 * 0.1025f;
		float pokeDistanceY = w * 2 * 0.1125f;
		bounds = new Bounds(Camera.main.transform.position, new Vector3(w*2 - pokeDistanceX, osize*2 - pokeDistanceY));

		spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
		spriteRenderers[0].material.SetFloat("_AlphaModifier", 1);

		PlayerColor = GameColors.white;
		fillColor = Color.white;

		box = GetComponent<BoxCollider2D>();

		directional = Instantiate(directionalPrefab, new Vector3(-0.0825f, 0.1725f, 0), Quaternion.identity).GetComponent<Directional>();
		directional.PlayerTransform = this.transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(!Impaled) {
			transform.position += new Vector3(horizontal * moveSpeed, vertical * moveSpeed, 0);
		}

		Vector2 factor3D = new Vector2(0.1f, 0.1f);

		//ensure player can't move out of bounds
		Vector2 size = box.bounds.extents;
		if (transform.position.x + size.x + factor3D.x > bounds.max.x) {
			transform.position = new Vector3(bounds.max.x - size.x - factor3D.x, transform.position.y, 0);
		}
		else if (transform.position.x - size.x + factor3D.x < bounds.min.x) {
			transform.position = new Vector3(bounds.min.x + size.x - factor3D.x, transform.position.y, 0);
		}

		if (transform.position.y + size.y - factor3D.y > bounds.max.y) {
			transform.position = new Vector3(transform.position.x, bounds.max.y - size.y + factor3D.y, 0);
		}
		else if (transform.position.y - size.y - factor3D.y < bounds.min.y) {
			transform.position = new Vector3(transform.position.x, bounds.min.y + size.y + factor3D.y, 0);
		}
	}

	public void SetMoveDirections(float h, float v) {
		if(!Impaled) {
			float smoothxTime = Mathf.Abs(h) > Mathf.Abs(horizontal) ? 0.1f : 0.25f;
			float smoothyTime = Mathf.Abs(v) > Mathf.Abs(vertical) ? 0.1f : 0.25f;

			horizontal = Mathf.SmoothDamp(horizontal, h, ref velxSmoothing, smoothxTime);
			vertical = Mathf.SmoothDamp(vertical, v, ref velySmoothing, smoothyTime);

			DirectionColor(horizontal, vertical);
		}
	}

	private void DirectionColor(float horizontal, float vertical) {
		if(Mathf.Abs(horizontal) < 0.05f && Mathf.Abs(vertical) < 0.05f) {
			if(fillColor != Color.white) {
				StartCoroutine(PauseBeforeWhite());
			}
			return;
		}

		if(Mathf.Abs(horizontal) > Mathf.Abs(vertical)) {
			if(horizontal > 0) {
				ColorVertices(GameColors.red);
			}
			else {
				ColorVertices(GameColors.blue);
			}
		}
		else {
			if(vertical > 0) {
				ColorVertices(GameColors.yellow);
			}
			else {
				ColorVertices(GameColors.green);
			}
		}
	}

	private void ColorVertices(GameColors c) {
		if(PlayerColor == c)
			return;

		PlayerColor = c;
		fillColor = GetColorFromGameColor(PlayerColor);
		GameManager.Instance.background.SetColor(c);
		foreach(SpriteRenderer s in spriteRenderers) {
			s.color = fillColor;
		}
	}

	IEnumerator PauseBeforeWhite() {
		yield return new WaitForSeconds(0.1f);
		if (Mathf.Abs(horizontal) < 0.05f && Mathf.Abs(vertical) < 0.05f && !Impaled) {
			ColorVertices(GameColors.white);
		}
	}

	public void SetDead() {
		StartCoroutine(GameOver());
	}

	IEnumerator GameOver() {
		yield return new WaitForSeconds(0.5f);
		GameManager.Instance.GameOver();
	}

	public static Color GetColorFromGameColor(GameColors c) {
		switch (c) {
			default:
			case GameColors.white:
				return Color.white;
			case GameColors.yellow:
				return Color.yellow;
			case GameColors.red:
				return Color.red;
			case GameColors.green:
				return Color.green;
			case GameColors.blue:
				return Color.blue;
		}
	}

	public void OnDestroy() {
		if(directional != null)
			Destroy(directional.gameObject);
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if(other.tag == "Blade") {
			Blade b = other.GetComponent<Blade>();
			if (b.HasLaunched && (!b.HasStopped || b.JustCollided)) {
				if(b.BladeColor != this.PlayerColor) {
					//ded
					if(!Impaled) {
						Impaled = true;
						this.transform.parent = other.transform;

						//move player to back so that the blade renders in front of it
						spriteRenderers[0].material.SetFloat("_AlphaModifier", 0);
						spriteRenderers[0].sortingLayerName = "PlayerBack";

						//add layer on top of player so that it 
						Instantiate(playerTop, this.transform).GetComponent<SpriteRenderer>().color = GetColorFromGameColor(PlayerColor);

						//stop all player movement
						this.horizontal = 0f;
						this.vertical = 0f;

						b.ShortenTravelDistance(box.bounds.extents.x);
						b.HasImpaledPlayer = true;

						//play impale sound	
						AudioSource a = gameObject.AddComponent<AudioSource>();
						a.mute = !GameManager.Instance.settings.SFX;
						a.volume = GameManager.Instance.MenuOpen ? 0.25f : 1f;
						a.clip = Resources.Load<AudioClip>("Sounds/impale");
						a.Play();

						//if center is outside boudns of blade, move it so that it's halfway in
						if ( b.StartDirection == Direction.Down || b.StartDirection == Direction.Up ) {
							float factor = b.StartDirection == Direction.Down ? 1 : -1;
							if(transform.position.x > (other.bounds.center.x + other.bounds.extents.x)) {
								transform.position = other.bounds.center + new Vector3(other.bounds.extents.x, factor * other.bounds.extents.y, 0);
							}
							else if(transform.position.x < (other.transform.position.x - other.bounds.extents.x)) {
								transform.position = other.bounds.center + new Vector3(-other.bounds.extents.x, factor * other.bounds.extents.y, 0);
							}
							else {
								transform.position = new Vector3( transform.position.x, other.bounds.center.y + factor * other.bounds.extents.y);
							}
						}
						else {
							float factor = b.StartDirection == Direction.Left ? 1 : -1;
							if (transform.position.y > (other.bounds.center.y + other.bounds.extents.y)) {
								transform.position = other.bounds.center + new Vector3(	factor * other.bounds.extents.x, other.bounds.extents.y, 0 );									
							}
							else if (transform.position.y < (other.transform.position.y - other.bounds.extents.y)) {
								transform.position = other.bounds.center + new Vector3(factor * other.bounds.extents.x, -other.bounds.extents.y, 0);
							}
							else {
								transform.position = new Vector3(other.bounds.center.x + factor * other.bounds.extents.x, transform.position.y);
							}
						}
					}
				}
				else {
					//player walked through the blade and matched color, get reward of 2 points -- TODO: fix potential race condition?
					GameManager.Instance.scoreManager.IncrementScore(2);
					b.PlayerDodged();
				}
			}
		}
	}
}
