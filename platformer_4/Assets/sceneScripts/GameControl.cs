using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {

	[Header("Crystal Prefabs")]
	public GameObject[] redCrystalPrefab;
	public GameObject[] yellowCrystalPrefab;
	public GameObject[] greenCrystalPrefab;

	[Header("Physics Params")]
	public float horizontalAcceleration;
	public float maxHorizontalVelocity;
	public float verticalAcceleration;
	public float maxVerticalVelocity;
	public float groundVelocity;

	[Header("Ground Params")]
	public Transform ground; 
	public float crystalDistance; 
	public float spawnCrystalDistance; 
	public int numberOfCrystalTypes; 

	[Header("Player Params")]
	public Transform player;
	
	public float playerSideBorder;
	public float playerUpperBorder;
	public float playerBottom;

	private GameObject[][] crystalArray;
	private System.Random rand = new System.Random();
	private Vector2 playerVelocity = new Vector2(0, 0);
	private float maxCrystalHeight = 0f;
	private float lastCrystalRightSidePositionX;
	private GameStats stats;

	void Start () {

		crystalArray = new GameObject[numberOfCrystalTypes][];
		crystalArray[0] = redCrystalPrefab;
		crystalArray[1] = yellowCrystalPrefab;
		crystalArray[2] = greenCrystalPrefab;

		for (int i = 0; i < crystalArray.Length; i++) {
			for (int j = 0; j < crystalArray[i].Length; ++j) {
				float crystalHeight = crystalArray[i][j].transform.GetChild(0).transform.position.y;
				if (crystalHeight > maxCrystalHeight) {
					maxCrystalHeight = crystalHeight;
				}
			}
		}
		stats = this.GetComponent<GameStats>();
		stats.pointCount = 0;
		lastCrystalRightSidePositionX = -spawnCrystalDistance;
		SpawnCrystals();

		if (player.position.y > playerUpperBorder) {
			ground.position = new Vector3(0, playerUpperBorder - player.position.y);
			player.position = new Vector3(0, playerUpperBorder, 0);
		}
	}
	
	void Update () {

		float deltaTime = 0.03f;
		if (Time.deltaTime < deltaTime) {
			deltaTime = Time.deltaTime;
		}

		ChangeCurrentVelocity(deltaTime);
		MoveGameSceneHorizontal(deltaTime);
		MoveGameSceneVertical(deltaTime);
		SpawnCrystals();
	}
	void ChangeCurrentVelocity(float deltaTime) {

		playerVelocity.y += verticalAcceleration * deltaTime;

		if (Mathf.Abs(playerVelocity.y) > maxVerticalVelocity) {
			playerVelocity.y = maxVerticalVelocity * Mathf.Sign(playerVelocity.y);
		}

		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
		{
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				playerVelocity.x -= horizontalAcceleration * deltaTime;
				if (playerVelocity.x < -maxHorizontalVelocity)
				{
					playerVelocity.x = -maxHorizontalVelocity;
				}
			}
			else
			{
				playerVelocity.x += horizontalAcceleration * deltaTime;
				if (playerVelocity.x > maxHorizontalVelocity)
				{
					playerVelocity.x = maxHorizontalVelocity;
				}
			}
		}
		else
		{
			playerVelocity.x = 0;
		}
	}
	void MoveGameSceneHorizontal(float deltaTime) {

		MoveGroundAt(-groundVelocity * deltaTime, true);

		float playerPositionX = player.position.x + playerVelocity.x * deltaTime;
		if (playerPositionX > playerSideBorder) {
			MoveGroundAt(playerSideBorder - playerPositionX, true);
			playerPositionX = playerSideBorder;
		}
		else {
			if (playerPositionX < -playerSideBorder) {
				MoveGroundAt(-playerSideBorder - playerPositionX, true);
				playerPositionX = -playerSideBorder;
			}
		}
		player.position = new Vector3(playerPositionX, player.position.y, 0);
	}
	void MoveGameSceneVertical(float deltaTime) {

		float playerPositionY = player.position.y + playerVelocity.y * deltaTime;

		if (playerPositionY >= playerUpperBorder)
		{
			if (player.position.y < playerUpperBorder)
			{
				ground.position = new Vector3(0, (playerUpperBorder - playerPositionY), 0);
				playerPositionY = playerUpperBorder;
			}
			else
			{
				playerPositionY = playerUpperBorder;
				MoveGroundAt(-playerVelocity.y * deltaTime, false);
			}
		}
		else
		{
			if (player.position.y == playerUpperBorder)
			{
				MoveGroundAt(-playerVelocity.y * deltaTime, false);
				if (ground.position.y > 0)
				{
					playerPositionY = playerUpperBorder - ground.position.y;
					ground.position = new Vector3(0, 0, 0);
				}
				else {
					playerPositionY = playerUpperBorder;
				}

			}
		}

		if (playerPositionY <= maxCrystalHeight) {
			Transform crystal = null;
			for (int i = 0; i < ground.childCount; i++) {
				Transform currentCrystal = ground.GetChild(i).transform;
				if ((currentCrystal.GetChild(0).transform.position.x - player.position.x - playerBottom) * 
					((currentCrystal.GetChild(1).transform.position.x - player.position.x + playerBottom)) < 0) {
					crystal = currentCrystal;
				}
			}
			if (crystal != null) {
				float crystalHeight = crystal.GetChild(0).transform.position.y;
				int bonusPoints = 0;
				if (playerPositionY < crystalHeight && player.position.y >= crystalHeight)
				{
					playerPositionY = 2 * crystalHeight - playerPositionY;
					switch (crystal.tag)
					{
						case "Red":
							{
								crystal.gameObject.GetComponent<RedCrystalScript>().JumpOn(stats, Mathf.Abs(playerVelocity.y) / maxVerticalVelocity);
								playerVelocity.y = Mathf.Abs(playerVelocity.y) * 0.95f;
								break;
							}
						case "Yellow":
							{
								crystal.gameObject.GetComponent<YellowCrystalScript>().JumpOn(stats, Mathf.Abs(playerVelocity.y) / maxVerticalVelocity);
								playerVelocity.y = 25f;
								break;
							}
						case "Green":
							{
								crystal.gameObject.GetComponent<GreenCrystalScript>().JumpOn(stats, Mathf.Abs(playerVelocity.y) / maxVerticalVelocity);
								playerVelocity.y = Mathf.Abs(playerVelocity.y) * 1.05f;
								break;
							}
					}
				}
			}
		}
		player.position = new Vector3(player.position.x, playerPositionY, 0);
	}
	void SpawnCrystals() {
		if (spawnCrystalDistance - lastCrystalRightSidePositionX > crystalDistance)
		{
			Transform newCrystal = PlaceCrystal();
			float newCrystalPositionX = lastCrystalRightSidePositionX + crystalDistance + Random.Range(-crystalDistance / 2f, 0f);
			float newCrystalPositionY = ground.position.y;
			newCrystal.position = new Vector3(newCrystalPositionX, newCrystalPositionY, 0);
			newCrystal.SetParent(ground);

			lastCrystalRightSidePositionX = newCrystal.GetChild(1).position.x;
		}
	}
	void MoveGroundAt(float deltaPos, bool isHorizontally) {
		if (isHorizontally) {
			for (int i = 0; i < ground.childCount; i++)
			{
				Transform crystalTransform = ground.GetChild(i).transform;
				crystalTransform.position = new Vector3(crystalTransform.position.x + deltaPos, crystalTransform.position.y, 0);
			}
			lastCrystalRightSidePositionX += deltaPos;
		}
		else {
			ground.position = new Vector3(0, ground.position.y + deltaPos, 0);
		}
	}
	Transform PlaceCrystal() {

		// choicing the type of crystals
		GameObject[] crystalsOfSameTypes = crystalArray[numberOfCrystalTypes - 1];

		for (int i = 0; i < numberOfCrystalTypes; i++) {

			if (rand.Next(0, 2) == 1) {
				crystalsOfSameTypes = crystalArray[i];
				break;
			}
			else {
				if (i == numberOfCrystalTypes - 1) {
					crystalsOfSameTypes = crystalArray[0];
				}
			}

		}

		// choicing the variation of the same crystals
		GameObject crystalObj = crystalsOfSameTypes[rand.Next(0, crystalsOfSameTypes.Length)];

		//placing the crystal
		Transform crystal = Instantiate(crystalObj).transform;

		return crystal;
	}
}
