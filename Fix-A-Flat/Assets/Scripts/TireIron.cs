using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TireIronStatus{
	None,
	OneHandHolding,
	TowHandHolding
}

[RequireComponent(typeof(AudioSource))]
public class TireIron : MonoBehaviour {
	public Transform pivot;
	public TireIronHead head;
	public TireIronTail tail;

	public bool isTwisting = false;
	public float preAngle = 0;

	public TireIronStatus status;

	//vibration
	public ViveVibration vbLeft;
	public ViveVibration vbRight;
	private float vbTimer = 0.0f;

	// audio
	public float volume = 1.0f;
	protected AudioSource audioSource;
	public AudioClip[] progressAudio;
	private float audioTimer = 0.0f;

	public void SetStatus(TireIronStatus s){
		status = s;
	}

	void Start(){
		status = TireIronStatus.None;

		audioSource = GetComponent<AudioSource> ();
		audioSource.loop = false;
		audioSource.volume = volume;
		audioSource.spatialBlend = 1.0f;
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

	bool isReady(){
		return status == TireIronStatus.TowHandHolding && head.hand != null && tail.hand != null;
	}

	void Update(){
		
		if (vbTimer > 0) {
			vbTimer -= Time.deltaTime;
		}

		if (audioTimer > 0) {
			audioTimer -= Time.deltaTime;
		}

		if (isReady ()) {

			//transform.eulerAngles = new Vector3(transform.eulerAngles.x, 37.043f, 0.6760001f);

			Vector3 dir = Vector3.ProjectOnPlane(tail.hand.position - pivot.transform.position, pivot.transform.right);
			Quaternion q = Quaternion.LookRotation (dir);
			transform.rotation = q;

			float progress = head.target == null ? 0 : Mathf.Min (1, Mathf.Max (head.target.progress / head.target.max2, 0));

			Vector3 scale = transform.localScale;

			if (dir.x < 0) {
				transform.localScale = new Vector3 (Mathf.Abs(scale.x) * -1, scale.y, scale.z);
			} else {
				transform.localScale = new Vector3 (Mathf.Abs(scale.x), scale.y, scale.z);

			}
			float curAngle = transform.eulerAngles.x;
			if (head.target != null && preAngle != 0) {
				float diff = (curAngle - preAngle) * transform.localScale.x;
				if (diff != 0) {
					print (diff);


					head.target.SetTwistAngle (diff);

					if (audioTimer <= 0 && Mathf.Abs(diff) > 0.001f) {
						playByProgress (progress);
						audioTimer = 0.5f;
					}

					if (vbTimer <= 0) {
						float intansity = 0.4f + progress * 0.4f;
						vbLeft.VibrateOn (intansity, 0.3f);
						vbRight.VibrateOn (intansity, 0.3f);
						vbTimer = 0.3f;
					}
				}
			}
			preAngle = curAngle;

			//if (!head.position.Equals (Vector3.zero)) {
			//	transform.position = head.position;
			//}
				
		}else {
			isTwisting = false;
			preAngle = 0;
		}
	}
}
