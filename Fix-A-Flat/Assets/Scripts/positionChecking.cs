using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positionChecking : MonoBehaviour {

	public GameObject[] items;
	public float y_limit;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		for (int i = 0; i < items.Length; i++) {
			GameObject it = items [i];
			if (it == null)
				continue;
			if (it.transform.position.y < y_limit) {
				Vector3 pos = it.transform.position;
				pos.y = y_limit;
				it.transform.position = pos;
			}
		}
	}
}
