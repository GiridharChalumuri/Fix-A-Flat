using UnityEngine;
using System.Collections;

public class TireIronHead : MonoBehaviour
{
	static string TAG_HAND = "vive_hand";
	public TwistTarget2 target = null;
	public Transform hand = null;

	public Vector3 position = Vector3.zero;
	public TireIronHead ()
	{
	}

	void OnTriggerEnter(Collider collider){

		if (collider.gameObject.tag.Equals (TAG_HAND) && hand == null) {
			hand = collider.gameObject.transform;

			position = gameObject.transform.parent.position;

		} else if(collider.gameObject.GetComponent<TwistTarget2> () != null && target == null){
			target = collider.gameObject.GetComponent<TwistTarget2> ();
		}
	}

	void OnTriggerExit(Collider collider){

		if (hand != null && collider.gameObject.GetInstanceID () == hand.gameObject.GetInstanceID ()) {
			hand = null;
			position = Vector3.zero;
		} else if (target != null && collider.gameObject.GetInstanceID () == target.gameObject.GetInstanceID ()) {
			target = null;
		}
	}
}
