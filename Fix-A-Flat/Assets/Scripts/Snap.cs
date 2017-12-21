using System;

namespace FAFVR
{
	public enum SnapBaseState {
		Open,
		Closed,
		Locked
	};


	public enum SnapType {
		None,
		LugNut,
		FlatTire,
		SpareTire,
		TireIron,
		Jack,
		ToolTray,
		JackHook,
		Other
	};

	public enum SnapTargetState {
		Open,
		Holding,  
		Moving,
		Closed,
		Locked
	};


	public enum SnapDependencyType {
		All,
		One
	};
}

