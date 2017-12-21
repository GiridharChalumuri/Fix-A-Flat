using UnityEngine;
using System.Collections;
using FAFVR;

public enum ColliderType {
	LugNut,
	FlatTire,
	SpareTire,
	TireIron,
	Tools,
	CarBody,
	Ground,
	JackHook,
	Jack,
	Others
};
[RequireComponent(typeof(AudioSource))]
public class CollisionAudio:MonoBehaviour
{

	public ColliderType colliderType = ColliderType.LugNut;

	public float volume = 1.0f;
	public AudioLibray audioLib;
	protected AudioSource audioSource;

	void Start () {
		Collider[] colliders = gameObject.GetComponents<Collider> ();
		if (colliders.Length == 0) {
			gameObject.AddComponent<BoxCollider> ();
		}

		Rigidbody rig = gameObject.GetComponent<Rigidbody> ();

		if (rig == null) {
			gameObject.AddComponent<Rigidbody> ();
		}
		audioSource = GetComponent<AudioSource> ();
		audioSource.loop = false;
		audioSource.volume = volume;
		audioSource.spatialBlend = 1.0f;
	}

	private void playAudioEffect(AudioClip src){
		if (src == null)
			return;
		audioSource.PlayOneShot (src);
	}

	void OnCollisionEnter(Collision collision){
		if (audioLib == null)
			return;

		CollisionAudio ca = collision.gameObject.GetComponent<CollisionAudio> ();

		if (gameObject.GetInstanceID () < collision.gameObject.GetInstanceID ()) {
			return;
		}

		if (ca == null)
			return;

		ColliderType ta = ca.colliderType;
		ColliderType tb = colliderType;
		if (ta > tb) {
			ColliderType tmp = ta;
			ta = tb;
			tb = tmp;
		}

		AudioClip c = audioLib.getClip (string.Format ("{0}_{1}", ta.ToString (), tb.ToString ()));

		if (c == null) {
			c = audioLib.getClip (string.Format ("{1}_{0}", ta.ToString (), tb.ToString ()));
		}
		playAudioEffect (c);
	}
}

