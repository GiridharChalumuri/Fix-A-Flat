using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FAFVR;

[RequireComponent(typeof(AudioSource))]
public class StepManager : MonoBehaviour {

	public StepInfo[] steps;
	public int stepIndex = 0;
	public float audioVolume = 1;
	protected AudioSource audioSource;
	public float delay = 10;
	private bool started = false;
	private bool completed = false;

	private GameObject curObj = null;

	public TimerGUI timer;
	public Text uiInstruction;

	// Use this for initialization
	void Start () {

		audioSource = gameObject.GetComponent<AudioSource> ();
		audioSource.loop = false;
		audioSource.volume = audioVolume;
		audioSource.spatialBlend = 1.0f;



		timer.index = 0;
	}

	private void playAudioEffect(AudioClip src){
		if (src == null)
			return;
		audioSource.PlayOneShot (src);
	}

	private void enableStep(){
		if (stepIndex >= 18 && stepIndex <= 22) {
			prepareLugNutInstall ();
			return;
		}
		if (stepIndex < 0 || stepIndex >= steps.Length)
			return;

		StepInfo info = steps [stepIndex];
		GameObject[] items = info.items;
		AudioClip[] audios = info.audios;

		// highligt all items
		for (int i = 0; i < items.Length; i++) {
			items [i].SetActive (true);

			HighlighterHelper[] hl = items [i].GetComponentsInChildren<HighlighterHelper> ();
			if (hl.Length > 0) {
				hl[0].lightOn (info.defaultColor);
			}
		}

		// play step instruction audio
		if (audios.Length > 0)
			playAudioEffect (audios [0]);

		uiInstruction.text = info.description;

	}

	private void prepareLugNutInstall(){
		if (stepIndex < 0 || stepIndex >= steps.Length)
			return;

		StepInfo info = steps [stepIndex];
		GameObject[] items = info.items;
		AudioClip[] audios = info.audios;

		// highligt all items
		for (int i = 0; i < items.Length; i++) {
			items [i].SetActive (true);

			if (i == 0) {
				SnapBase sb = items [i].GetComponent<SnapBase> ();
				sb.state = SnapBaseState.Open;
				sb.targetObj = null;
			}
			if (i > 0 && items [i].GetComponent<SnapTarget> ().state != SnapTargetState.Open)
				continue;

			HighlighterHelper[] hl = items [i].GetComponentsInChildren<HighlighterHelper> ();
			if (hl.Length > 0) {
				hl[0].lightOn (info.defaultColor);
			}
		}

		// select a lugNut an put it onto the stud
		if (audios.Length > 0)
			playAudioEffect (audios [0]);

		uiInstruction.text = info.description;
	}

	private void lightOffAll(GameObject[] it){
		for (int i = 0; i < it.Length; i++) {
			HighlighterHelper[] hl = it [i].GetComponentsInChildren<HighlighterHelper> ();
			if (hl.Length > 0) {
				hl[0].lightOff ();
			}
		}
	}

	private void updateStep(){
		if (stepIndex < 0 || stepIndex >= steps.Length)
			return;
		StepInfo info = steps [stepIndex];
		GameObject[] items = info.items;
		AudioClip[] audios = info.audios;

		// pick up the tire iron
		if (stepIndex == 0)
		{
			if (items[0].GetComponent<SnapTarget>().state == SnapTargetState.Locked)
			{
				items[0].GetComponent<SnapTarget>().unlockTarget();
			}
			if (items[1].GetComponent<SnapBase>().state == SnapBaseState.Closed)
				return;
			lightOffAll(items);

		}
		// remove the lug nuts
		else if (stepIndex >= 1 && stepIndex <= 5)
		{
			TwistTarget2 lugNut = items[0].GetComponent<TwistTarget2>();
			if (lugNut.progress != 0)
			{
				return;
			}
			items[0].GetComponent<SnapTarget>().highlighter.lightOff();
			items[0].GetComponent<TwistTarget2>().SetDisable();

		}
		// put the jack to the correct position
		else if (stepIndex == 6)
		{
			if (items[0].GetComponent<SnapTarget>().state == SnapTargetState.Locked)
			{
				items[0].GetComponent<SnapTarget>().unlockTarget();
			}
			if (items[1].GetComponent<SnapBase>().state == SnapBaseState.Closed)
				return;
			lightOffAll(items);

		}
		// put the jack to the correct position
		else if (stepIndex == 7) {
			if (items [0].GetComponent<SnapTarget> ().state != SnapTargetState.Closed)
				return;
			lightOffAll (items);
		}
		// pick up the jack hook
		else if (stepIndex == 8) {
			if (items [0].GetComponent<SnapTarget> ().state == SnapTargetState.Locked) {
				items [0].GetComponent<SnapTarget> ().unlockTarget ();
			}
			if (items [1].GetComponent<SnapBase> ().state == SnapBaseState.Closed)
				return;
			lightOffAll (items);
		} else if (stepIndex == 9) {

			for (int i = 2; i < items.Length; i++) {
				if (items [i].GetComponent<SnapTarget> ().state != SnapTargetState.Locked) {
					items [i].GetComponent<SnapTarget> ().lockTarget ();
					items [i].GetComponent<SnapTarget> ().highlighter.lightOff ();
				}
			}


			if (items [0].GetComponent<JackHook> ().state != JackHookState.Connecting) {
				items [1].GetComponent<HighlighterHelper> ().lightOn (Color.green);
				return;
			}

			// lock the jack once the jack hook is connected.
			items [0].GetComponent<JackHook> ().SetEnable ();
			steps [0].items [0].GetComponent<SnapTarget> ().lockTarget ();

			if (items [1].GetComponent<JackHookBase> ().getProgress () < 1.0f)
				return;

			items [0].GetComponent<JackHook> ().SetDisable ();

			lightOffAll (items);
		}
		// remove the lug nuts
		else if (stepIndex >= 10 && stepIndex <= 14) {
			SnapTarget lugNut = items [0].GetComponent<SnapTarget> ();
			if (lugNut.state == SnapTargetState.Locked) {
				lugNut.unlockTarget ();
			}
			if (lugNut.state != SnapTargetState.Open) {
				return;			
			}
			lugNut.highlighter.lightOff ();
		} else if (stepIndex == 15) {

			SnapTarget flatTire = items [0].GetComponent<SnapTarget> ();
			if (flatTire.state == SnapTargetState.Locked) {
				flatTire.unlockTarget ();
			}

			if (flatTire.state != SnapTargetState.Open) {
				return;
			}
			flatTire.highlighter.lightOff ();
		} 
		// get the spare tire from the truck
		else if (stepIndex == 16) {

			// hide the tool tray over the spare tire
			items [2].SetActive (false);

			if (items [0].GetComponent<SnapTarget> ().state == SnapTargetState.Locked) {
				items [0].GetComponent<SnapTarget> ().unlockTarget ();
			}

			if (items [1].GetComponent<SnapBase> ().state == SnapBaseState.Closed)
				return;

			lightOffAll (items);
		}
		// put the spare tire onto the hub
		else if (stepIndex == 17) {

			if (items [1].GetComponent<SnapBase> ().state != SnapBaseState.Closed)
				return;

			lightOffAll (items);
		}
		// put back the lug nuts and tighten them
		else if (stepIndex >= 18 && stepIndex <= 22) {

			SnapBase stud = items [0].GetComponent<SnapBase> ();

			if (stud.targetObj != null && curObj == null) {
				curObj = stud.targetObj;
				if (curObj.GetComponent<TwistTarget2> () != null) {
					curObj.GetComponent<TwistTarget2> ().SetEnable ();
				}
				// please put on tighten the lugNut with Tire Iron
				if (audios.Length > 1)
					playAudioEffect (audios [1]);
			}

			if (stud.state == SnapBaseState.Closed) {
				HighlighterHelper[] hl = stud.GetComponentsInChildren<HighlighterHelper> ();
				if (hl.Length > 0) {
					hl [0].lightOff ();
				}
			}
			if (stud.state == SnapBaseState.Closed
				&& curObj != null
				&& curObj.GetComponent<TwistTarget2> ().progress >= curObj.GetComponent<TwistTarget2> ().max2) {

				curObj = null;
				// go to next Step
			} else {
				return;
			}

			lightOffAll (items);
		} 
		// jack down the car
		else if (stepIndex == 23) {

			items [0].GetComponent<JackHook> ().SetEnable ();

			if (items [1].GetComponent<JackHookBase> ().getProgress () > 0.0f)
				return;

			lightOffAll (items);

			if (audios.Length > 1)
				playAudioEffect (audios [1]);
		} 
		// put the flat tire to the truck
		else if (stepIndex == 24) {
			items [2].SetActive (false);
			if (items [1].GetComponent<SnapBase> ().state != SnapBaseState.Closed)
				return;

			items [0].GetComponent<SnapTarget> ().lockTarget ();
			items [2].SetActive (true);
			lightOffAll (items);
		}

		// disconnect the jack hook and put it back to the trunk
		else if (stepIndex == 25) {
			// disable the jaskhook base;
			items [1].SetActive (false);

			items [0].GetComponent<JackHook> ().SetState (JackHookState.Open);

			if (items [2].GetComponent<SnapBase> ().state != SnapBaseState.Closed)
				return;

			// unlock the jack
			steps [0].items [0].GetComponent<SnapTarget> ().unlockTarget ();

			lightOffAll (items);

		} 
		// remove the jack and put it back to the trunk
		else if (stepIndex == 26) {
			if (items [1].GetComponent<SnapBase> ().state != SnapBaseState.Closed)
				return;
			lightOffAll (items);

		}
		// put the tire iron to the  tooltray
		else if (stepIndex == 27) {
			if (items [1].GetComponent<SnapBase> ().state != SnapBaseState.Closed)
				return;
			lightOffAll (items);
		} else if (stepIndex == 28) {
			HighlighterHelper car = items [0].GetComponent<HighlighterHelper> ();
			car.lightOn ();
			print ("Completed!!!!!");
			completed = true;
			timer.index = 3;
		}

		info.delay -= Time.deltaTime;
		if (info.delay > 0)
			return;
		stepIndex = info.nextStepIndex;
		enableStep ();
	}



	// Update is called once per frame
	void Update () {

		if (completed)
			return;

		delay -= Time.deltaTime;


		if (delay > 0)
			return;

		if (!started) {
			enableStep ();
			started = true;
		}

		updateStep ();
	}
}
