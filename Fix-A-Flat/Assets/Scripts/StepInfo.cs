using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FAFVR;
public class StepInfo  : MonoBehaviour{
	public string description;
	public Color defaultColor = Color.cyan;
	public GameObject[] items;
	public AudioClip[] audios;
	public GameObject[] sign;
	public int nextStepIndex;
	public float delay = 0;
}
