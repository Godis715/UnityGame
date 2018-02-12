using UnityEngine;
using System.Collections;

public class RedCrystalScript : MonoBehaviour {

	public GameObject stats;
	public int bonusPoints;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void JumpOn(GameStats stats, float k) {
		stats.pointCount += 1 + (int)Mathf.Floor(bonusPoints * k);
		Destroy(this.gameObject);
	}
}
