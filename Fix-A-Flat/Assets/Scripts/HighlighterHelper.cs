using UnityEngine;
using System.Collections;
using FAFVR;
using HighlightingSystem;

public enum HighligerState {
	Off,
	On,
	Flashing
};
[RequireComponent(typeof(Highlighter))]
public class HighlighterHelper:MonoBehaviour
{
	public Color hlDefault = Color.cyan;

	public Color hlError = Color.red;

	private HighligerState state = HighligerState.Off;
	private HighligerState preState = HighligerState.Off;
	private Highlighter highlighter;

	public float flashingTime = 1.5f;
	public int flashingFreq = 5;
	private float timer = 0;
	private Color curColor = new Color(0,0,0,0);
	public void lightOff(){

		if (highlighter == null)
			return;

		highlighter.ConstantOff ();
		highlighter.FlashingOff ();
		timer = 0.0f;

		state = HighligerState.Off;
	}

	public void lightOn(){
		lightOn (hlDefault);
	}

	public void lightOn(Color color){

		if (highlighter == null)
			return;
		curColor = color;
		highlighter.ConstantOn (color);
		state = HighligerState.On;
	}

	public void lightError(){

		if (highlighter == null)
			return;

		if (timer > 0) {
			//timer = flashingTime;
			//highlighter.FlashingOn (hlError, new Color(0,0,0,0), flashingFreq);
			return;
		}
		highlighter.FlashingOn (hlError, new Color(0,0,0,0), flashingFreq);

		timer = flashingTime;
		preState = state;
		state = HighligerState.Flashing;
	}

	public void lightErrorOff(){

		if (highlighter == null)
			return;
		highlighter.FlashingOff ();
		timer = 0;
		state = preState;
	}

	void Start () {
		
		highlighter = gameObject.GetComponent<Highlighter> ();

		lightOff ();
		//lightOn ();
		//highlighter.seeThrough = true;
		highlighter.seeThrough = false;

	}

	void Update(){

		if (state == HighligerState.Flashing) {

			timer -= Time.deltaTime;

			if (timer <= 0) {
				lightErrorOff ();

				if (state == HighligerState.On) {
					lightOn (curColor);
				} else {
					lightOff ();
				}
			}
		}
	}
}

