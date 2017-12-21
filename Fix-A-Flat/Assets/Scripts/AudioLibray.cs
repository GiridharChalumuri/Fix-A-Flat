using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioLibray : MonoBehaviour {


	public AudioLibrayItem[] lib;

	private Dictionary<string, AudioClip[]> map;

	private System.Random rand = new System.Random ();
	void Start () {
		map = new Dictionary<string, AudioClip[]> ();

		for (int i = 0; i < lib.Length; i++) {
			if (lib [i] == null)
				continue;
			map.Add (lib [i].key, lib [i].clips);
		}
	}
	public AudioClip getClip(string key){
		print("Collison : " + key);
		if (!map.ContainsKey (key))
			return null;
		int i = rand.Next (0, map [key].Length);
		return map[key][i];
	}
}
