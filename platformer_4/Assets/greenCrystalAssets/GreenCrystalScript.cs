using UnityEngine;
using System.Collections;

public class GreenCrystalScript : MonoBehaviour {

	public int lives;
	public GameObject stats;
	public int bonusPoints;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void JumpOn(GameStats stats, float k) {
		--lives;
		if (lives == 0) {
			stats.pointCount += 1 + (int)Mathf.Floor(bonusPoints * k);
			Destroy(this.gameObject);
		}
	}
}
