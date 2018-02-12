using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameStats : MonoBehaviour {

	public int pointCount;
	public GameObject text;

	void Start () {
		pointCount = 0;
	}
	
	void Update () {
		text.GetComponent<Text>().text = pointCount.ToString();
	}
}
