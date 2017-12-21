using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VirtualHandState {
	Open,
	Touching,
	Holding,
	Twisting
};

public class VirtualHandInfo : MonoBehaviour
{
	// Enumerate states of virtual hand interactions

	[Tooltip("The tracking device used for tracking the real hand.")]
	public CommonTracker tracker;

	[Tooltip("The interactive used to represent the virtual hand.")]
	public Affect hand;

	[Tooltip("The button required to be pressed to grab objects.")]
	public CommonButton button;
	// Private interaction variables
	public VirtualHandState state;

	public GameObject target;
}