using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TwistTarget2 : MonoBehaviour {

	static string TAG_HAND = "vive_hand";

	public float curAngle = 0.0f;

	public float diffAngle = 0.0f;

	public float progress = 0.0f;

	public float max1 = 90.0f;
	public float max2 = 180.0f;
	private Vector3 startPos;
	private Vector3 endPos;

	public SnapTarget snap;
	public HighlighterHelper hl;

	// audio
	public float volume = 1.0f;
	protected AudioSource audioSource;
	public AudioClip clip;
	private float audioTimer = 0.0f;
    public bool enable = true;
    public void SetDisable()
    {
        enable = false;
    }
    public void SetEnable()
    {
        enable = true;
    }
    // Use this for initialization
    void Start () {
		if(GetComponent<SnapTarget> () != null){
			snap = GetComponent<SnapTarget> ();
		}

		if(GetComponent<HighlighterHelper> () != null){
			hl = GetComponent<HighlighterHelper> ();
		}

		audioSource = GetComponent<AudioSource> ();
		audioSource.loop = false;
		audioSource.volume = volume;
		audioSource.spatialBlend = 0.0f;
	}

	public void playAudioEffect(AudioClip src){
		if (src == null)
			return;
		audioSource.PlayOneShot (src);
	}

	public void SetRange(Vector3 start, Vector3 end){
		startPos = start;
		endPos = end;
	}

	void OnTriggerEnter(Collider collider){

		if (collider.gameObject.GetComponent<TwistRange> () != null) {
			TwistRange tr = collider.gameObject.GetComponent<TwistRange> ();
			startPos = tr.start.position;
			endPos = tr.end.position;
			return;
		}

		if (collider.gameObject.tag.Equals (TAG_HAND)) {
			curAngle = collider.gameObject.transform.eulerAngles.z;
			diffAngle = 0;
		}
	}

	void OnTriggerStay(Collider collider){
		if (snap.state < FAFVR.SnapTargetState.Closed)
			return;
		
		if (!collider.gameObject.tag.Equals (TAG_HAND))
			return;
		if (diffAngle == 0) {
			float zAngle = collider.gameObject.transform.eulerAngles.z;
			SetTwistAngle (curAngle - zAngle);
			curAngle = zAngle;

			if (progress >= max1 && diffAngle != 0) {
				diffAngle = 0;
				hl.lightError();
			}

			if(diffAngle != 0 && audioTimer <= 0){
				playAudioEffect (clip);
				audioTimer = 1.0f;
			}

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

		if (progress >= max2 && diffAngle > 0) {
			diffAngle = 0;
			hl.lightError();
		}
	}

	void Update(){
        if (!enable)
        {
            return;
        }
		if (audioTimer > 0) {
			audioTimer -= Time.deltaTime;
		}

		if (diffAngle != 0) {

			print ("1- c: "+ curAngle +"d: " +diffAngle + ", p:" + progress);

			float distance = diffAngle * 3;
			diffAngle = 0;

			if (Mathf.Abs (distance) < 0.1)
				return;
			float pre = progress;
			progress += distance;
			progress = Mathf.Max (0, progress);
			progress = Mathf.Min (max2, progress);
			transform.Rotate(Vector3.right * distance);
			transform.position = Vector3.Lerp(startPos, endPos, progress / max2);
			if (pre < max2 && progress >= max2) {
				hl.lightErrorOff ();
				hl.lightOff ();
			} else if (pre == max2 && progress < max2) {
				hl.lightErrorOff ();
				hl.lightOn (Color.yellow);
			}
			else if (pre < max1 && progress >= max1) {
				hl.lightErrorOff ();
				hl.lightOn (Color.yellow);
			} else if (pre > 0 && progress == 0) {
				hl.lightErrorOff ();
				hl.lightOn (Color.green);
				snap.unlockTarget ();
			} else if (pre == 0 && progress > 0) {
				snap.lockTarget ();
			}

			print ("2- c: "+ curAngle +"d: " +diffAngle + ", p:" + progress);
		}
	}
}
