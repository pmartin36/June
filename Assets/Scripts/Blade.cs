using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Blade : MonoBehaviour {

	public Direction StartDirection { get; set; }
	public GameColors BladeColor { get; set; }
	public Color FillColor { get; set; }

	//Range 0-1f, Increments of 0.1f
	//Each 0.1f corresponds to 1/10th of the screen size in the particular direction
	public float Size { get; set; }

	//Has the blade left the start dock
	public bool HasLaunched { get; set; }
	//Has the blade reached the end dock or collided with another
	public bool HasStopped { get; set; }

	//master collision handlers will place both blades so they their heads line up
	public bool MasterCollisionHandler { get; set; }

	public float InitTime { get; set; }

	public AudioSource audioPlayer;
	public float DefaultVolume { get; set; }
	public bool HasImpaledPlayer { get; set; }

	Vector3 startLinePosition;
	Vector3 endPosition;

	Vector3 velocity;

	BoxCollider2D box;

	public static GameObject sparkPrefab;
	public static Sprite HorizontalBlade;
	public static Sprite VerticalBlade;

	private SpriteRenderer spriteRenderer;

	//for when the player and another blade hit the same frame
	public bool JustCollided { get; private set; }

	// Use this for initialization
	void Start () {
		HasLaunched = false;
		HasImpaledPlayer = false;
		velocity = Vector3.zero;

		//sparkPrefab is static
		if(sparkPrefab == null) {
			sparkPrefab = Resources.Load<GameObject>("Prefabs/Sparks");
		}

		StartCoroutine(Launch());
	}
	
	// Update is called once per frame
	void Update () {
		if(HasStopped) {
			StartCoroutine(StartDestroy());
		}
	}

	public void Init(BladeInfo bi, Vector2 camSize) {
		Init(	
			bi.BladeDirection,
			bi.BladeColor,
			bi.Position,
			bi.Size,
			bi.InitTime,
			bi.OrderInLayer,
			bi.AudioClip,
			camSize
		);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="startDirection"></param>
	/// <param name="bladeColor"></param>
	/// <param name="position">Range 0-0.9f, the starting index of the blade</param>
	/// <param name="size"></param>
	/// <param name="cameraSize"></param>
	public void Init(	Direction startDirection,
						GameColors bladeColor,
						float position,
						float size,
						float initTime,
						int orderInLayer,
						AudioClip audioClip,
						Vector2 cameraSize) {

		if (HorizontalBlade == null) {
			HorizontalBlade = Resources.Load<Sprite>("Sprites/Blade_Horizontal");
		}
		if (VerticalBlade == null) {
			VerticalBlade = Resources.Load<Sprite>("Sprites/Blade_Vertical");
		}

		StartDirection = startDirection;
		BladeColor = bladeColor;
		FillColor = Blade.GetColorFromGameColor(BladeColor);
		InitTime = initTime;
		Size = size;

		float size10 = size * 10;

		spriteRenderer = GetComponent<SpriteRenderer>();

		//Rendering
		Shader shader = EnumHelper.IsLeftRight(StartDirection) ? Shader.Find("Sprites/BladeHorizontal") : Shader.Find("Sprites/BladeVertical");

		Material m = new Material(shader);
		m.SetFloat("_Size", size10);
		m.SetFloat("_ShineLocation", -0.5f);
		spriteRenderer.material = m;

		spriteRenderer.color = FillColor;
		spriteRenderer.sortingOrder = orderInLayer;


		box = GetComponent<BoxCollider2D>();

		audioPlayer = GetComponent<AudioSource>();
		audioPlayer.clip = audioClip;

		float scale = 1.45f;

		//offset from position start 
		//(half of a camera strip) * size * 10 == (0.5 * camSize/10 * size * 10) == 0.5 * camSize * size
		float offset = 0.5f * size;

													  //center around 0
		//position = (position + offset)*camerSize    -  cameraSize/2 
		Vector3 pokeDistance;
		switch (StartDirection) {
			case Direction.Up:
				position = (position + offset - 0.5f) * cameraSize.x;

				pokeDistance = -new Vector3(0, cameraSize.x * 0.055f, 0);

				spriteRenderer.sprite = VerticalBlade;

				transform.localScale = new Vector2( scale * size10, scale );
				transform.position = new Vector2( position, cameraSize.y );
				startLinePosition = transform.position + pokeDistance;
				endPosition = transform.position - pokeDistance - new Vector3(0, cameraSize.y, 0);

				box.size = new Vector2( 1.85f, 0.24f );
				box.offset = new Vector2( 0f, -5.28f );

				MasterCollisionHandler = true;
				break;
			case Direction.Right:
				position = (position + offset - 0.5f) * cameraSize.y;

				pokeDistance = -new Vector3(cameraSize.x * 0.055f, 0, 0);

				spriteRenderer.sprite = HorizontalBlade;

				transform.localScale = new Vector2(scale, scale * size10);
				transform.position = new Vector2(cameraSize.x, position);
				startLinePosition = transform.position + pokeDistance;
				endPosition = transform.position - pokeDistance * 0.85f - new Vector3(cameraSize.x, 0, 0);

				box.size = new Vector2( 0.26f, 0.98f);
				box.offset = new Vector2( -9.46f, 0f );

				MasterCollisionHandler = true;
				break;
			case Direction.Down:
				position = (position + offset - 0.5f) * cameraSize.x;

				pokeDistance = new Vector3(0, cameraSize.x * 0.055f, 0);

				spriteRenderer.sprite = VerticalBlade;

				transform.localScale = new Vector2(scale * size10, -scale);
				transform.position = new Vector2(position, -cameraSize.y);
				startLinePosition = transform.position + pokeDistance;
				endPosition = transform.position - pokeDistance + new Vector3(0, cameraSize.y, 0);

				box.size = new Vector2(1.85f, 0.24f);
				box.offset = new Vector2(0f, -5.28f);

				MasterCollisionHandler = false;
				break;
			case Direction.Left:
				position = (position + offset - 0.5f) * cameraSize.y;

				pokeDistance = new Vector3(cameraSize.x * 0.055f, 0, 0);

				spriteRenderer.sprite = HorizontalBlade;

				transform.localScale = new Vector2(-scale, scale * size10);
				transform.position = new Vector2(-cameraSize.x, position);
				startLinePosition = transform.position + pokeDistance;
				endPosition = transform.position - pokeDistance * 0.85f + new Vector3(cameraSize.x, 0, 0);

				box.size = new Vector2(0.26f, 0.98f);
				box.offset = new Vector2(-9.46f, 0f);

				MasterCollisionHandler = false;
				break;
		}
	}

	public void CheckForCollisions() {
		List<RaycastHit2D> rayhits = new List<RaycastHit2D>();		
		Vector2 start, end;

		//we don't want to shoot a ray at the min or the max so we add 1 to the size
		// Example: size 3 blade - shoot rays at 1/4, 2/4, and 3/4
		int size10 = (int)(Size * 10f) + 1;

		//determine the minimum and maximum positions 
		//left/right blades start at the bottom and go up
		//bottom/top blades start at the left and go to the right
		if (EnumHelper.IsLeftRight(StartDirection)) {
			start = new Vector2(box.bounds.center.x, box.bounds.min.y);
			end = new Vector2(box.bounds.center.x, box.bounds.max.y);
		}
		else {	
			start = new Vector2(box.bounds.min.x, box.bounds.center.y);
			end = new Vector2(box.bounds.max.x, box.bounds.center.y);
		}

		for (int i = 1; i < size10; i++) {
			float percentThru = (float)i / (float)size10;
			Vector2 startpoint = Vector2.Lerp(start, end, percentThru);
			rayhits.AddRange( Physics2D.RaycastAll(startpoint, velocity, velocity.magnitude * 2) ) ;
			Debug.DrawRay( startpoint, (Vector2)velocity * 2, Color.magenta);
		}


		foreach(RaycastHit2D hit in rayhits) {
			Collider2D other = hit.collider;
			if (other.tag == "Blade") {
				Blade b = other.GetComponent<Blade>();
				//if the blade we collided with is going in the other direction
				if (b.StartDirection == EnumHelper.GetOppositeDirection(this.StartDirection)) {
					if (HasLaunched && !HasStopped && b.HasLaunched && !b.HasStopped) {
						//make position so that it butts heads with other blade
						//in a collision, the blades with Direction of UP or RIGHT will determine the middle position of the two blades and move them there
						Vector2 midpoint = Vector2.zero;
						if (StartDirection == Direction.Right) {
							midpoint = (this.transform.position + other.transform.position) / 2f;

							other.transform.position = new Vector2(midpoint.x - spriteRenderer.bounds.extents.x, other.transform.position.y);
							this.transform.position = new Vector2(midpoint.x + spriteRenderer.bounds.extents.x, this.transform.position.y);

							this.endPosition = this.transform.position;
							b.endPosition = other.transform.position;

							HasStopped = true;
							b.HasStopped = true;
							JustCollided = true;
						}
						else if (StartDirection == Direction.Up) {
							midpoint = (this.transform.position + other.transform.position) / 2f;
							other.transform.position = new Vector2(other.transform.position.x, midpoint.y - spriteRenderer.bounds.extents.y);
							this.transform.position = new Vector2(this.transform.position.x, midpoint.y + spriteRenderer.bounds.extents.y);
 
							this.endPosition = this.transform.position;
							b.endPosition = other.transform.position;
								
							HasStopped = true;
							b.HasStopped = true;
							JustCollided = true;
						}

						//stop the blade from moving any further
						StopCoroutine(Launch());

						//show spark particles
						if(this.Size < b.Size || (this.Size == b.Size && EnumHelper.IsMasterDirection(this.StartDirection))) {
							Vector2 location = EnumHelper.IsLeftRight(this.StartDirection) ? new Vector2( midpoint.x, transform.position.y ) : new Vector2( transform.position.x, midpoint.y);
							Spark(location, b.BladeColor);
						}

						//play collision sound			
						AudioSource a = gameObject.AddComponent<AudioSource>();
						a.mute = !GameManager.Instance.settings.SFX;
						a.volume = GameManager.Instance.MenuOpen ? 0.25f : 0.75f;
						a.clip = Resources.Load<AudioClip>("Sounds/clash7");
						a.Play();
					}
				}
			}
		}		
	}

	/// <summary>
	/// Move blade towards its end destination
	/// </summary>
	/// <returns></returns>
	IEnumerator Launch() {
		//move up slowly to starting line
		float startTime = Time.time;
		Vector3 startPosition = transform.position;

		float tripTime = 1f;
		while(Time.time - startTime < tripTime + Time.deltaTime) {
			float jtime = (Time.time - startTime) / tripTime;
			transform.position = Vector3.Lerp(startPosition, startLinePosition, jtime);
			yield return new WaitForEndOfFrame();
		}

		//pause for seconds
		yield return new WaitForSeconds(1f);

		//launch
		HasLaunched = true;
		float launchTime = Time.time;

		//play sound
		audioPlayer.Play();

		//move blade
		while(Time.time - launchTime < tripTime + Time.deltaTime) {
			JustCollided = false;
			if(HasStopped) {
				yield break;
			}

			Vector3 currentPos = transform.position;

			float jtime = (Time.time - launchTime) / tripTime;
			transform.position = Vector3.Lerp(startLinePosition, endPosition, jtime);
			spriteRenderer.material.SetFloat("_ShineLocation", Mathf.Lerp(-0.5f, 1.1f, jtime));

			velocity = transform.position - currentPos;

			CheckForCollisions();

			yield return new WaitForFixedUpdate();
		}

		HasStopped = true;
		velocity = Vector3.zero;

		if(HasImpaledPlayer) {
			//play hit wall sound		
			AudioSource a = gameObject.AddComponent<AudioSource>();
			a.mute = !GameManager.Instance.settings.SFX;
			a.volume = GameManager.Instance.MenuOpen ? 0.25f : 0.75f;
			a.clip = Resources.Load<AudioClip>("Sounds/wall");
			a.Play();
		}
	}

	public void PlayerDodged() {
		//play collision sound			
		AudioSource a = gameObject.AddComponent<AudioSource>();
		a.mute = !GameManager.Instance.settings.SFX;
		a.volume = GameManager.Instance.MenuOpen ? 0.25f : 0.75f;
		a.clip = Resources.Load<AudioClip>("Sounds/ding");
		a.Play();

		box.enabled = false;
		StartCoroutine(FadeToTransparent());
	}

	IEnumerator FadeToTransparent() {
		float startTime = Time.time;
		float journeyTime = 0.5f;
		Color startColor = spriteRenderer.color;

		while(Time.time - startTime < journeyTime + Time.deltaTime) {
			spriteRenderer.color = Color.Lerp( startColor, Color.clear, (Time.time - startTime) / journeyTime);
			yield return new WaitForEndOfFrame();
		}
	}

	/// <summary>
	/// Called after a collsion with another blade or after the blade has reached its end location
	/// </summary>
	/// <returns></returns>
	public IEnumerator StartDestroy() {
		foreach(Transform child in transform) {
			if(child.tag == "Player") {
				child.GetComponent<Player>().SetDead();
				child.parent = null;				
			}
		}
		yield return new WaitForSeconds(0.5f);
		Destroy(this.gameObject);
	}

	public static Color GetColorFromGameColor(GameColors c) {
		switch (c) {
			default:
			case GameColors.white:
				return new Color(1,1,1,1);
			case GameColors.yellow:
				return new Color(0.5f, 0.5f, 0, 1f);
			case GameColors.red:
				return new Color(0.5f, 0, 0, 1f);
			case GameColors.green:
				return new Color(0, 0.5f, 0, 1f);
			case GameColors.blue:
				return new Color(0, 0, 0.5f, 1f);
			case GameColors.black:
				return new Color(0.2f, 0.2f, 0.2f, 1f);
		}
	}

	/// <summary>
	/// Shorten the end location that the blade will be travel 
	/// Called when a player is impaled by a blade
	/// </summary>
	/// <param name="amount"></param>
	public void ShortenTravelDistance(float amount) {
		switch (StartDirection) {
			case Direction.Up:
				endPosition += new Vector3(0, amount, 0);
				break;
			case Direction.Right:
				endPosition += new Vector3(amount, 0, 0);
				break;
			case Direction.Down:
				endPosition += new Vector3(0, -amount, 0);
				break;
			case Direction.Left:
				endPosition += new Vector3(-amount, 0, 0);
				break;
		}
	}

	/// <summary>
	/// Create a spark at the collision point of the 2 blades
	/// The smaller blade in the collision creates the spark
	/// </summary>
	/// <param name="spawnLocation"></param>
	/// <param name="othercolor"></param>
	public void Spark(Vector3 spawnLocation, GameColors othercolor) {
		Spark spark = Instantiate(sparkPrefab, spawnLocation, Quaternion.identity).GetComponent<Spark>();
		spark.Init(Size * 10, BladeColor, othercolor);
	}
}
