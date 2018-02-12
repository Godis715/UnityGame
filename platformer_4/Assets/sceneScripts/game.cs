using UnityEngine;
using System.Collections;

public class game : MonoBehaviour {

	private const float eps = 1e-3f;
	private Vector2 velocity;
	private float lastCrystal;
	
	public Transform player;
	public Transform ground;
	public Transform bottom;

	public float acc;
	public float horVel;
	public float horAcc;
	public float height;
	public float bottomLength;
	public float delta;
	public float groundVelocity;
	public float playerBorder;
	public float crystalSpawn;
	public float crystalDistance;

	public GameObject crystalPref;

	void Start () {
		velocity = new Vector2(0, 0);
		Transform tempCrystal = Instantiate(crystalPref).transform;
		tempCrystal.position = new Vector3(crystalSpawn, 0, 0);
		tempCrystal.SetParent(ground);
		lastCrystal = tempCrystal.position.x;
	}

	// Update is called once per frame
	void Update()
	{

		delta = Time.deltaTime;
		if (delta > 0.03f)
		{
			delta = 0.03f;
		}

		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) {
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				velocity.x -= horAcc * delta;
				if (velocity.x < -horVel) {
					velocity.x = -horVel;
				}
			}
			else {
				velocity.x += horAcc * delta;
				if (velocity.x > horVel) {
					velocity.x = horVel;
				}
			}
		}
		else {
			velocity.x = groundVelocity;
		}


		MoveVertically();
		MoveHorizontally();
		SpawnCrystal();

		velocity.y += acc * delta;

		
	}

	void SpawnCrystal() {
		if (crystalSpawn - lastCrystal >= crystalDistance) {
			if (Mathf.Round(Random.Range(0, 200)) % 3 != 0)
			{
				Transform tempCrystal = Instantiate(crystalPref).transform;
				tempCrystal.position = new Vector3(crystalSpawn, 0, 0);
				tempCrystal.SetParent(ground);
			}
			lastCrystal = crystalSpawn;
		}
	}

	void MoveHorizontally() {


		if (player.position.x + velocity.x * delta < playerBorder && player.position.x + velocity.x * delta > -playerBorder)
		{
			Vector3 pos = player.position;
			pos.x += velocity.x * delta;
			player.position = pos;

			for (int i = 0; i < ground.childCount; i++)
			{
				Transform crystal = ground.GetChild(i).transform;
				Vector3 crystPos = crystal.position;
				crystPos.x -= groundVelocity * delta;
				crystal.position = crystPos;
			}
		}
		else {
			if (player.position.x + velocity.x * delta >= playerBorder)
			{
				player.position = new Vector3(playerBorder, player.position.y, 0f);
				float deltaPos = player.position.x + velocity.x * delta - playerBorder;

				lastCrystal -= deltaPos;
				for (int i = 0; i < ground.childCount; i++)
				{
					Transform crystal = ground.GetChild(i).transform;
					Vector3 pos = crystal.position;
					pos.x -= deltaPos;
					crystal.position = pos;
				}
			}
			else {
				player.position = new Vector3(-playerBorder, player.position.y, 0f);
				float deltaPos = player.position.x + velocity.x * delta + playerBorder;

				lastCrystal -= deltaPos;
				for (int i = 0; i < ground.childCount; i++)
				{
					Transform crystal = ground.GetChild(i).transform;
					Vector3 pos = crystal.position;
					pos.x -= deltaPos;
					crystal.position = pos;
				}
			}
		}
		
	}

	void MoveByGravity() {

		Vector3 pos = player.position;

		pos.y += velocity.y * delta;
		player.position = pos;

	}

	void MoveVertically() {
		Vector3 pos = player.position;

		if (pos.y < height) {
			MoveByGravity();
			return;
		}

		if (Mathf.Abs(pos.y - height) < eps && velocity.y * delta < eps)
		{
			pos.y = height;
			velocity.y = 0;
			return;
		}

		if (pos.y + velocity.y * delta > height)
		{
			MoveByGravity();
		}
		else {

			for (int i = 0; i < ground.childCount; i++) {

				Transform crystal = ground.GetChild(i);
				if ((crystal.GetChild(0).position.x - bottomLength - player.position.x) * (crystal.GetChild(1).position.x + bottomLength - player.position.x) <= 0)
				{
					pos.y = 2 * height - (pos.y + velocity.y * delta);
					velocity.y = Mathf.Abs(velocity.y);
					Destroy(ground.GetChild(i).gameObject);
					return;
				}
			}
			MoveByGravity();
		}
	}
}
