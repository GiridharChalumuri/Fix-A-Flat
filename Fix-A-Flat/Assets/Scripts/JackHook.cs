using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JackHookState {
	Open,
	Holding,
	Connecting
};
[RequireComponent(typeof(AudioSource))]
public class JackHook : MonoBehaviour {

	public JackHookState state;

	public Transform pivot;
	public JackHookHead head;
	public JackHookTail tail;

	private Rigidbody rig = null;

	public bool isTwisting = false;
	public float preAngle = 0;

	//vibration
	public ViveVibration vbLeft;
	public ViveVibration vbRight;
	private float vbTimer = 0.0f;

	// audio
	public float volume = 1.0f;
	protected AudioSource audioSource;
	public AudioClip[] progressAudio;
	private float audioTimer = 0.0f;

	public bool isRotating = false;

	public bool enable = true;

	void Start(){
		audioSource = GetComponent<AudioSource> ();
		audioSource.loop = false;
		audioSource.volume = volume;
		audioSource.spatialBlend = 1.0f;

		state = JackHookState.Open;

		rig = gameObject.GetComponent<Rigidbody> ();

		if (rig == null) {
			rig = gameObject.AddComponent<Rigidbody> ();
			rig.useGravity = true;
			rig.isKinematic = false;
		}
	}

	public void playByProgress(float val){
		val = Mathf.Abs (val);
		if (progressAudio.Length > 0) {
			playAudioEffect (progressAudio [Mathf.FloorToInt (val * (progressAudio.Length - 1))]);
		}
	}

	public void playAudioEffect(AudioClip src){
		if (src == null)
			return;
		audioSource.PlayOneShot (src);
	}

	public void SetState(JackHookState s){
		state = s;
	}

	public void SetDisable(){
		enable = false;
		transform.localEulerAngles = new Vector3 (0, 0, 0);
	}

	public void SetEnable(){
		enable = true;
	}
	public void connectTo(Vector3 pos){
		if (state == JackHookState.Holding) {
			return;
		}
		clearPhysic ();

		transform.position = pos;
		state = JackHookState.Connecting;
	}

	private void clearPhysic(){
		rig.isKinematic = true;
	}

	bool isReady(){
		return state == JackHookState.Connecting && isRotating && head.target != null && head.hand == null && tail.hand != null;
	}
	
	// Update is called once per frame
	void Update () {
		if (!enable)
			return;
		
		if (vbTimer > 0) {
			vbTimer -= Time.deltaTime;
		}

		if (audioTimer > 0) {
			audioTimer -= Time.deltaTime;
		}

		if (isReady ()) {

			transform.eulerAngles = new Vector3(transform.eulerAngles.x, 37.043f, 0.6760001f);

			Vector3 dir = Vector3.ProjectOnPlane(tail.hand.position - pivot.transform.position, pivot.transform.right);
			Quaternion q = Quaternion.LookRotation (dir);
			transform.rotation = q;

			float progress = head.target == null ? 0 : Mathf.Min (1, Mathf.Max (head.target.progress / head.target.max, 0));

			Vector3 scale = transform.localScale;

			if (dir.x < 0) {
				transform.localScale = new Vector3 (Mathf.Abs(scale.x) * -1, scale.y, scale.z);
			} else {
				transform.localScale = new Vector3 (Mathf.Abs(scale.x), scale.y, scale.z);

			}
			float curAngle = transform.eulerAngles.x;
			if (head.target != null && preAngle != 0) {
				float diff = (curAngle - preAngle) * (transform.localScale.x > 0 ? 1 : -1);
				if (diff != 0) {
					//print (diff);

					head.target.SetTwistAngle (diff);

					if (vbTimer <= 0) {
						float intansity = 0.4f + progress * 0.4f;
						vbLeft.VibrateOn (intansity, 0.3f);
						vbRight.VibrateOn (intansity, 0.3f);
						vbTimer = 0.3f;
					}

					if (audioTimer <= 0) {
						playByProgress (progress);
						audioTimer = 0.5f;
					}
						
				}
			}
			preAngle = curAngle;


		}else {
			isTwisting = false;
			preAngle = 0;
		}
	}
}
