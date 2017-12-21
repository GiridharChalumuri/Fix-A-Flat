using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackHookBase : MonoBehaviour {

	public Transform pos;
	public float progress = 0.0f;
	public float max = 0.0f;
	public float curAngle = 0.0f;
	public float diffAngle = 0.0f;
	public HighlighterHelper hl;

	void Start(){
		if (progress >= max) {
			hl.lightOn (Color.red);
		} else if (progress < max && progress > 0) {
			hl.lightOn (Color.yellow);
		} else {
			hl.lightOn (Color.green);
		}
	}
	void OnTriggerStay(Collider collider){
		
		JackHook  jk= collider.gameObject.GetComponent<JackHook> ();
		if (jk == null || jk.state == JackHookState.Holding)
			return;

		if (jk.state != JackHookState.Connecting) {
			jk.connectTo (pos.position);
		}
	}

	void OnTriggerExist(Collider collider){

		JackHook  jk= collider.gameObject.GetComponent<JackHook> ();
		if (jk != null && jk.state == JackHookState.Connecting) {
			jk.state = JackHookState.Open;
		}
	}

	public void SetTwistAngle(float angle){
		if (angle < -180) {
			angle = 360 - angle;
		} else if (angle > 180) {
			angle = angle - 180;
		}
		angle = Mathf.Max (-10, angle);
		angle = Mathf.Min (10, angle);

		diffAngle = angle;

		if (progress >= max && diffAngle > 0) {
			diffAngle = 0;
			//hl.lightError();
		}
	}

	public float getProgress(){
		return Mathf.Min(1.0f, Mathf.Max (0, progress / max));
	}

	void Update(){

		if (diffAngle != 0) {

			print ("1- c: "+ curAngle +"d: " +diffAngle + ", p:" + progress);

			float distance = diffAngle * 2;
			diffAngle = 0;

			if (Mathf.Abs (distance) < 0.1)
				return;
			float pre = progress;
			progress += distance;
			//progress = Mathf.Floor (progress);
			progress = Mathf.Max (0, progress);
			progress = Mathf.Min (max, progress);
			transform.Rotate(Vector3.right * distance);
			if (progress >= max) {
				hl.lightOn (Color.red);
			} else if (progress < max && progress > 0) {
				hl.lightOn (Color.yellow);
			} else {
				hl.lightOn (Color.green);
			}

			//print ("c: "+ curAngle +"d: " +diffAngle + ", p:" + progress);
		}
	}
}
