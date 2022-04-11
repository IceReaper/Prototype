namespace Prototype.Scripts.EntityComponents;

using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Systems.Entities;

public class ReserveCellComponent : SyncScript
{
	private Vector3 reservedPosition = new(-1);
	private int reservedX = -1;
	private int reservedY = -1;

	public override void Update()
	{
		var currentX = (int)this.Entity.Transform.Position.X;
		var currentY = (int)this.Entity.Transform.Position.Z;

		if (this.reservedX == currentX && this.reservedY == currentY)
			return;

		var grid = this.Entity.Scene.Entities.FirstOrDefault(nameof(WorldGrid))?.Components.FirstOrDefault<GridComponent>();

		grid?.UnreserveCell(this.reservedPosition, this.Entity);
		grid?.ReserveCell(this.Entity.Transform.Position, this.Entity);

		this.reservedPosition = this.Entity.Transform.Position;
		this.reservedX = currentX;
		this.reservedY = currentY;
	}
}
