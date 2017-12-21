using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class TimerGUI : MonoBehaviour {

	// Use this for initialization
	Text state;

	public int startTime;
	public float curTime;

	public int index = -1;

	protected AudioSource audioSource;
	public AudioClip countDown;
	public AudioClip go;
	public float delay;
	public AudioClip welcome;
	void Start () {
		state = gameObject.GetComponent<Text> ();
		state.text = startTime.ToString();
		curTime = startTime;
		audioSource = GetComponent<AudioSource> ();
		audioSource.clip = welcome;
		audioSource.PlayDelayed (1.0f);
	}

	private void playAudioEffect(AudioClip src){
		if (src == null)
			return;
		audioSource.PlayOneShot (src);
	}
	
	// Update is called once per frame
	void Update () {
		delay -= Time.deltaTime;


		if (delay > 0)
			return;
		
		if (index == 0) {
			float tmpTime = curTime - Time.deltaTime;
			if (Mathf.CeilToInt (tmpTime) != Mathf.CeilToInt (curTime)) {
				curTime = Mathf.CeilToInt (tmpTime);
				state.text = curTime.ToString ();
				playAudioEffect(countDown);
			}
			curTime = tmpTime;
			if (curTime < 0.0f) {
				state.text = "GO";
				index = 1;
				curTime = 0;
				//playAudioEffect(go);
			}
		} else if (index == 1) {
			curTime += Time.deltaTime;

			if (curTime > 1) {
				curTime = 1;
				index = 2;
				state.fontSize = 20;
				state.alignment = TextAnchor.UpperCenter;
				state.text = Mathf.FloorToInt (curTime).ToString ();
			}
		} else if (index == 2) {
			curTime += Time.deltaTime;
			int numOfSecond = Mathf.FloorToInt (curTime);
			int min = numOfSecond / 60;
			int a = min / 10, b = min % 10;
			int second = numOfSecond % 60;
			int c = second / 10, d = second % 10;
			state.text = a + "" + b + ":" + c + "" + d;
		} else if (index == 3) {

			int score = (int)(curTime / 60);
			if (score < 5) {
				score = 100;
			} else if (score < 10) {
				score = 100 - 10 * (score - 5);
			} else {
				score = 0;
			}
			state.fontSize = 60;
			state.text = "Socre: " + score + "/100";
		}
	}
}
