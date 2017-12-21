using UnityEngine;
using System.Collections;
using FAFVR;
using HighlightingSystem;
public class SnapTarget:MonoBehaviour
{
	public SnapType targetType = SnapType.None;
	public SnapTargetState state = SnapTargetState.Open;

	private Transform dist = null;
	public float speed = 1.0f;
	private float timeLasp = 0.0f;

	public AudioClip lockSound;
	public AudioClip unlockSound;
	public HighlighterHelper highlighter;
	private float volume = 1.0f;

	protected AudioSource audioSource;
	private Rigidbody rig = null;

	public SnapTarget ()
	{
	}

	public bool isFree(){
		if (state != SnapTargetState.Open)
			return false;

		return true;
	}

	public void lockTarget(){
		if (state == SnapTargetState.Closed) {
			state = SnapTargetState.Locked;
		}
	}

	public void unlockTarget(){
		if (state == SnapTargetState.Locked) {
			state = SnapTargetState.Closed;
		}
	}

	public void snapTo(Transform d){
		if (d == null || state != SnapTargetState.Open) {
			return;
		}
		dist = d;

		clearPhysic ();

		state = SnapTargetState.Moving;
	}

	public void disconnect(){
		
		state = state == SnapTargetState.Holding? state : SnapTargetState.Open;
		dist = null;
		timeLasp = 0.0f;

		resetPhysic ();

		playAudioEffect (unlockSound);
	}

	public void setState(SnapTargetState s){
		state = s;
	}

	private void playAudioEffect(AudioClip src){
		if (src == null)
			return;

		if (audioSource == null) {
			audioSource = gameObject.AddComponent<AudioSource> ();
		}
		audioSource.loop = false;
		audioSource.volume = volume;
		audioSource.spatialBlend = 1.0f;

		audioSource.PlayOneShot (src);
	}

	private void clearPhysic(){
		rig.isKinematic = true;
	}

	private void resetPhysic(){
		if (state != SnapTargetState.Holding) {
			rig.isKinematic = false;
		}
	}

	void Start () {
		rig = gameObject.GetComponent<Rigidbody> ();

		if (rig == null) {
			rig = gameObject.AddComponent<Rigidbody> ();
			rig.useGravity = true;
			rig.isKinematic = false;
		}


		if(highlighter == null)
			highlighter = gameObject.GetComponent<HighlighterHelper> ();

		if (state == SnapTargetState.Locked) {
			clearPhysic ();
		} else {
		
		}
	}

	// FixedUpdate is not called every graphical frame but rather every physics frame
	void FixedUpdate ()
	{
		if (state == SnapTargetState.Moving && timeLasp / speed <= 1.0f) {
			timeLasp += Time.deltaTime;

			if (timeLasp >= speed) {
				state = SnapTargetState.Closed;
				timeLasp = 0.0f;
				playAudioEffect (lockSound);
			}
			gameObject.transform.position = Vector3.Lerp (gameObject.transform.position, dist.transform.position, timeLasp / speed);
			gameObject.transform.rotation = dist.transform.rotation;
		} 


	}
}

