using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JackItems{
	public GameObject[] items;

	public void SetActive(bool a){
		for(int i = 0; i<items.Length; i++){
			items [i].SetActive (a);
		}
	}
}

public class JackController : MonoBehaviour {

	public GameObject[] jack;
	public JackItems[] itemGroupA;
	public JackItems[] itemGroupB;

	public GameObject flatTire;
	public GameObject spareTire;
	public JackHookBase jackBase;
	public int curIndex = 0;
	public bool isCompleted = false;
	public float progress = 0.0f;

	public SnapTarget snap;

	public Transform lugNutPos1;
	public Transform lugNutPos2;
	public Transform lugNutHolder;
	public Transform world;
	public Transform[] items;

	public void SetComplete(){
		isCompleted = true;
		itemGroupA [curIndex].SetActive (false);
		itemGroupB [curIndex].SetActive (true);
		updateTires ();
	}

	public void disableLugNuts(){
		for (int i = 0; i < items.Length; i++)
		{
			items [i].gameObject.SetActive (false);               
		}
	}

	public void enableLugNuts(){
		for (int i = 0; i < items.Length; i++)
		{
			items [i].gameObject.SetActive (true);               
		}
	}

	public void moveLugNuts(){
		for (int i = 0; i < items.Length; i++)
		{
			items[i].parent = lugNutHolder;                
		}

		lugNutHolder.parent = lugNutPos1;
		lugNutHolder.localPosition = Vector3.zero;

		for (int i = 0; i < items.Length; i++)
		{
			items[i].parent = world;
		}

		lugNutHolder.parent = world;
	}

	// Use this for initialization
	void Start () {
		for (int i = 0; i < jack.Length; i++) {
			if (jack [i] != null) {
				jack [i].SetActive (false);
				itemGroupA [i].SetActive (false);
				itemGroupB [i].SetActive (false);
			}
		}

		jack [curIndex].SetActive(true);
		if(!isCompleted){
			itemGroupA [curIndex].SetActive (true);
		}else{
			itemGroupB [curIndex].SetActive (true);
		}
		updateTires ();

	}

	void updateHL(float p){
		if (p == 0 && progress != 0) {
			snap.highlighter.lightOff ();
			snap.highlighter.lightOn (Color.green);
			snap.unlockTarget ();
		} else if (progress == 0 && p != 0) {
			snap.highlighter.lightOff ();
			snap.lockTarget ();
		} else if (p == 0 && progress == 0) {
			snap.highlighter.lightOff ();
			snap.highlighter.lightOn (Color.green);
		}
	}

	void updateLock(float p){
		if (p == 0 && progress != 0) {
			snap.unlockTarget ();
		} else if (progress == 0 && p != 0) {
			snap.lockTarget ();
		}
	}

	// Update is called once per frame
	void Update () {

		float p = jackBase.getProgress ();

		//updateLock (p);

		progress = p;

		int index = Mathf.FloorToInt(progress * (jack.Length-1));

		if (index != curIndex) {

			JackItems[] gp = isCompleted ? itemGroupB : itemGroupA;
			jack [curIndex].SetActive(false);
			gp [curIndex].SetActive (false);
			jack [index].SetActive(true);
			gp [index].SetActive (true);
			curIndex = index;

			updateTires ();
			if (index == 1) {
				disableLugNuts ();
			}
			else if(index == 2)
			{

				moveLugNuts ();
				enableLugNuts ();
			}

			if (!isCompleted && curIndex == 2) {

				SetComplete ();
			}

		}
	}

	void updateTires(){
		if (isCompleted == false) {
			if (curIndex == 2) {
				flatTire.SetActive (true);
			} else {
				flatTire.SetActive (false);
			}

		} else {
			if (curIndex == 2) {
				spareTire.SetActive (true);
			} else {
				spareTire.SetActive (false);
			}
			flatTire.SetActive (true);
		}
	}
}
