using UnityEngine;
using System.Collections;

public class YellowCrystalScript : MonoBehaviour {

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
