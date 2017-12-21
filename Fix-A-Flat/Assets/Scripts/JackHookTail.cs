using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackHookTail : MonoBehaviour {

	public string TAG_HAND = "vive_hand";
	public Transform hand = null;

	void OnTriggerEnter(Collider collider){

		if (collider.gameObject.tag.Equals (TAG_HAND) && hand == null) {
			hand = collider.gameObject.transform;
		}
	}

	void OnTriggerExit(Collider collider){

		if (hand != null && collider.gameObject.GetInstanceID () == hand.gameObject.GetInstanceID ()) {
			hand = null;
		}
	}
}
