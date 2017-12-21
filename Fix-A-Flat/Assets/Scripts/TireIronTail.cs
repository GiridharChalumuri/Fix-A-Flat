using UnityEngine;
using System.Collections;

public class TireIronTail: MonoBehaviour
{
	static string TAG_HAND = "vive_hand";
	public Transform hand = null;

	public TireIronTail ()
	{
	}

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

