/*
	Stores state of Enemy.
	
*/
public enum MovementStates {
	Unaware,				// Will need expansion in the future....
	MovingToTarget,			// Is not currently in range, but is moving to be.
	HasLOS,					// Has a line of sight. Take cover now.
	InCover,				// Is in cover, waiting to fire or reloading.
	OutOfCover				// Is out of cover, firing.
}
