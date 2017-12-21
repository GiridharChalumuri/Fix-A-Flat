using UnityEngine;
using System.Collections;
using FAFVR;

public class SnapBase: MonoBehaviour
{

	public SnapBaseState state = SnapBaseState.Open;
	public SnapType targetType = SnapType.None;

	public Transform position = null;
	public GameObject targetObj = null;
	public HighlighterHelper highlighter;
	public SnapDependencyType dependencyType = SnapDependencyType.All;
	public SnapBase[] dependencies;
	void OnTriggerStay(Collider collider){
		// check type of collision object

		if (state != SnapBaseState.Open)
			return;
		
		SnapTarget target = collider.gameObject.GetComponent<SnapTarget> ();
		if (target == null || target.targetType != targetType)
			return;

		if (!target.isFree ()) {
			return;
		}

		if (checkDependency ()) {
			targetObj = collider.gameObject;
			target.snapTo (position);
			state = SnapBaseState.Closed;
		}
	}

	void OnTriggerExit(Collider collider){

		if (state != SnapBaseState.Closed || targetObj == null || (targetObj!= null && targetObj.GetComponent<SnapTarget>().state == SnapTargetState.Locked))
			return;

		if (collider.gameObject != targetObj)
			return;

		SnapTarget target = targetObj.GetComponent<SnapTarget> ();
		if (target == null || target.targetType != targetType)
			return;

		target.disconnect ();

		targetObj = null;

		state = SnapBaseState.Open;
	}

	bool checkDependency(){
		//1--> 3,4 --> 2,5
		int count = 0;
		for (int i = 0; i < dependencies.Length; i++) {
			count += dependencies [i].state == SnapBaseState.Closed ? 1 : 0;

			if (dependencyType == SnapDependencyType.All && count <= i) {
				return false;
			} 
			else if(dependencyType == SnapDependencyType.One && count > 0) {
				break;
			}
		}

		if (dependencies.Length > 0 && count <= 0)
			return false;

		return true;
	}

	void Start(){
		if(highlighter == null)
			highlighter = gameObject.GetComponent<HighlighterHelper> ();
	}

	void Update(){
//		if (highlighter == null)
//			return;
//		if (checkDependency ()) {
//			highlighter.gameObject.SetActive (true);
//			highlighter.lightOn ();
//		} else {
//			
//			highlighter.lightOff ();
//			highlighter.gameObject.SetActive (false);
//		}
	}
}