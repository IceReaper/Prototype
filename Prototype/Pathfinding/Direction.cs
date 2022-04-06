namespace Prototype.Pathfinding;

public enum Direction : byte
{
	// @formatter:off
	None      = 0b00000000,
	Up        = 0b00000001,
	UpRight   = 0b00000010,
	Right     = 0b00000100,
	DownRight = 0b00001000,
	Down      = 0b00010000,
	DownLeft  = 0b00100000,
	Left      = 0b01000000,
	UpLeft    = 0b10000000
	// @formatter:on
}
