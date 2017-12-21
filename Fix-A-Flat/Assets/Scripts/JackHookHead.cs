using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackHookHead : MonoBehaviour {

	static string TAG_HAND = "vive_hand";

	public JackHookBase target = null;
	public Transform hand = null;

	void OnTriggerEnter(Collider collider){

		if (collider.gameObject.tag.Equals (TAG_HAND) && hand == null) {
			hand = collider.gameObject.transform;
		}else if(collider.gameObject.GetComponent<JackHookBase> () != null && target == null){
			target = collider.gameObject.GetComponent<JackHookBase> ();
		}
	}

	void OnTriggerExit(Collider collider){

		if (hand != null && collider.gameObject.GetInstanceID () == hand.gameObject.GetInstanceID ()) {
			hand = null;
		}else if (target != null && collider.gameObject.GetInstanceID () == target.gameObject.GetInstanceID ()) {
			target = null;
		}
	}
}
