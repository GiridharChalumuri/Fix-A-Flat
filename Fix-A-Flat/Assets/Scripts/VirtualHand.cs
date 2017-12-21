/*
Copyright ©2017. The University of Texas at Dallas. All Rights Reserved. 

Permission to use, copy, modify, and distribute this software and its documentation for 
educational, research, and not-for-profit purposes, without fee and without a signed 
licensing agreement, is hereby granted, provided that the above copyright notice, this 
paragraph and the following two paragraphs appear in all copies, modifications, and 
distributions. 

Contact The Office of Technology Commercialization, The University of Texas at Dallas, 
800 W. Campbell Road (AD15), Richardson, Texas 75080-3021, (972) 883-4558, 
otc@utdallas.edu, https://research.utdallas.edu/otc for commercial licensing opportunities.

IN NO EVENT SHALL THE UNIVERSITY OF TEXAS AT DALLAS BE LIABLE TO ANY PARTY FOR DIRECT, 
INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES, INCLUDING LOST PROFITS, ARISING 
OUT OF THE USE OF THIS SOFTWARE AND ITS DOCUMENTATION, EVEN IF THE UNIVERSITY OF TEXAS AT 
DALLAS HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

THE UNIVERSITY OF TEXAS AT DALLAS SPECIFICALLY DISCLAIMS ANY WARRANTIES, INCLUDING, BUT 
NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
PURPOSE. THE SOFTWARE AND ACCOMPANYING DOCUMENTATION, IF ANY, PROVIDED HEREUNDER IS 
PROVIDED "AS IS". THE UNIVERSITY OF TEXAS AT DALLAS HAS NO OBLIGATION TO PROVIDE 
MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
*/

using UnityEngine;
using System.Collections;

public class VirtualHand : MonoBehaviour {

	public Transform world;
	public VirtualHandInfo left;

	public VirtualHandInfo right;

	public GameObject dhTraget;
	public Vector3 dhPivotOffset;
	// Called at the end of the program initialization
	void Start () {
		resetHand(left);
		resetHand(right);
	}

	void resetHand(VirtualHandInfo hand){
		hand.state = VirtualHandState.Open;
		hand.hand.type = AffectType.Virtual;
		if (hand.target != null) {
			Rigidbody rig = hand.target.GetComponent<Rigidbody> ();
			rig.isKinematic = false;
			rig.useGravity = true;
			hand.target.transform.parent = world;
		}
		hand.target = null;
	}

	bool isDoubleHolding(){
		return left.button.GetPress () 
				&& right.button.GetPress ()
				&& left.hand.triggerOngoing 
				&& right.hand.triggerOngoing
				&& left.hand.ongoingTriggers [0].GetInstanceID() == right.hand.ongoingTriggers [0].GetInstanceID()
				&& left.hand.ongoingTriggers [0].GetComponent<Interactive>().isHeavy;
	}

	bool isTwoHandTwisting(){
		return left.button.GetPress () 
			&& right.button.GetPress ()
			&& left.hand.triggerOngoing 
			&& right.hand.triggerOngoing
			&& left.hand.ongoingTriggers [0].GetInstanceID() == right.hand.ongoingTriggers [0].GetInstanceID()
			&& !left.hand.ongoingTriggers [0].GetComponent<Interactive>().isHeavy
			&& left.hand.ongoingTriggers [0].GetComponent<TireIron>() != null;
	}
	// FixedUpdate is not called every graphical frame but rather every physics frame
	void FixedUpdate ()
	{

		if (isDoubleHolding ()) {

			SnapTarget snap = left.hand.ongoingTriggers [0].gameObject.GetComponent<SnapTarget> ();
			if (snap != null && snap.state == FAFVR.SnapTargetState.Locked)
				return;
			
			Vector3 mid = (left.hand.transform.position + right.hand.transform.position) / 2.0f;

			if (dhTraget == null) {
				dhTraget = left.hand.ongoingTriggers [0].gameObject;
				left.state = VirtualHandState.Holding;
				right.state = VirtualHandState.Holding;

				if (dhTraget.GetComponent<SnapTarget> () != null) {
					dhTraget.GetComponent<SnapTarget> ().setState (FAFVR.SnapTargetState.Holding);
				}

				Rigidbody rig = dhTraget.GetComponent<Rigidbody> ();
				rig.isKinematic = true;
				rig.useGravity = false;

				dhPivotOffset = mid - dhTraget.transform.position;
			}
			dhTraget.transform.position = mid - dhPivotOffset;

		} else if(isTwoHandTwisting()){

			if (dhTraget == null) {
				dhTraget = left.hand.ongoingTriggers [0].gameObject;
				resetHand (left);
				resetHand (right);
				left.state = VirtualHandState.Holding;
				right.state = VirtualHandState.Holding;
				Rigidbody rig = dhTraget.GetComponent<Rigidbody> ();
				rig.isKinematic = true;
				rig.useGravity = false;

				dhTraget.GetComponent<TireIron> ().SetStatus (TireIronStatus.TowHandHolding);
			}
			
		}else {
			if (dhTraget != null) {
				left.state = VirtualHandState.Open;
				right.state = VirtualHandState.Open;

				Rigidbody rig = dhTraget.GetComponent<Rigidbody> ();
				rig.isKinematic = false;
				rig.useGravity = true;

				if (dhTraget.GetComponent<SnapTarget> () != null) {
					dhTraget.GetComponent<SnapTarget> ().setState (FAFVR.SnapTargetState.Open);
				}

				if (dhTraget.GetComponent<TireIron> () != null) {
					dhTraget.GetComponent<TireIron> ().SetStatus (TireIronStatus.None);
				}

				dhTraget = null;
			}

			updateHand (left);
			updateHand (right);
		}
	}

	void updateHand(VirtualHandInfo hi){
		VirtualHandState state = hi.state;
		Affect hand = hi.hand;
		CommonButton button = hi.button;
		GameObject target = hi.target;

		if (state == VirtualHandState.Open) {
			if (hand.triggerOngoing) {
				state = VirtualHandState.Touching;
			} else {
					
			}
		} else if (hi.state == VirtualHandState.Touching) {
			if (!hand.triggerOngoing) {
				// Change state to open
				state = VirtualHandState.Open;
				target = null;
			} else {
				if (button.GetPress () && target == null && !hand.ongoingTriggers [0].GetComponent<Interactive>().isHeavy) {
					
					target = hand.ongoingTriggers [0].gameObject;

					JackHook jackHook = target.GetComponent<JackHook> ();
					if (jackHook == null) {
						SnapTarget snap = target.GetComponent<SnapTarget> ();

						if (snap != null && snap.state == FAFVR.SnapTargetState.Locked) {
							state = VirtualHandState.Open;
							target = null;
						} else {

							if (snap != null) {
								snap.setState (FAFVR.SnapTargetState.Holding);
							}

							TireIron ti = target.GetComponent<TireIron> ();
							if (ti != null) {
								ti.SetStatus (TireIronStatus.OneHandHolding);
							}
							Rigidbody rig = target.GetComponent<Rigidbody> ();
							rig.isKinematic = true;
							rig.useGravity = false;
							target.transform.parent = hand.gameObject.transform;
							state = VirtualHandState.Holding;	
						}
					} else {
						if (jackHook.state == JackHookState.Open) {
							Rigidbody rig = target.GetComponent<Rigidbody> ();
							rig.isKinematic = true;
							rig.useGravity = false;
							target.transform.parent = hand.gameObject.transform;
							state = VirtualHandState.Holding;
							jackHook.SetState (JackHookState.Holding);
							jackHook.isRotating = false;
						} else if (jackHook.state == JackHookState.Connecting) {
							jackHook.isRotating = true;
						} else {
							jackHook.isRotating = false;
						}
					}

				}
			}
		} else if (hi.state == VirtualHandState.Holding) {
				
			if (target == null) {
				state = VirtualHandState.Open;
			}else {
				SnapTarget snap = target.GetComponent<SnapTarget> ();
				if (snap != null && snap.state == FAFVR.SnapTargetState.Locked) {
					target.transform.parent = world;
					target = null;
					state = VirtualHandState.Open;
				}
				else if (!button.GetPress () && target != null) {
					Rigidbody rig = target.GetComponent<Rigidbody> ();
					rig.isKinematic = false;
					rig.useGravity = true;
					target.transform.parent = world;

					state = VirtualHandState.Open;

					snap = target.GetComponent<SnapTarget> ();

					if (snap != null) {
						snap.setState (FAFVR.SnapTargetState.Open);
					}

					if (target.GetComponent<JackHook> () != null) {
						target.GetComponent<JackHook> ().state = JackHookState.Open;
					}
					target = null;
				} 
			}
		}

		hi.state = state;
		hi.target = target;
	}
}