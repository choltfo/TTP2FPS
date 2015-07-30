/*
	Stores state of Enemy.
	
*/
public enum MovementStates {
	MovingToTarget,			// Is not currently in range, but is moving to be.
	HasLOS					// Has a line of sight. Thus, stop, aim, and fire.
}
