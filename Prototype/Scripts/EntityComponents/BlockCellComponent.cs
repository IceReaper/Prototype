namespace Prototype.Scripts.EntityComponents;

using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Systems.Entities;

public class BlockCellComponent : SyncScript
{
	private Vector3 blockedPosition = new(-1);
	private int blockedX = -1;
	private int blockedY = -1;

	public override void Update()
	{
		var currentX = (int)this.Entity.Transform.Position.X;
		var currentY = (int)this.Entity.Transform.Position.Z;

		if (this.blockedX == currentX && this.blockedY == currentY)
			return;

		var grid = this.Entity.Scene.Entities.FirstOrDefault(nameof(WorldGrid))?.Components.FirstOrDefault<GridComponent>();

		grid?.UnblockCell(this.blockedPosition, this.Entity);
		grid?.BlockCell(this.Entity.Transform.Position, this.Entity);

		this.blockedPosition = this.Entity.Transform.Position;
		this.blockedX = currentX;
		this.blockedY = currentY;
	}
}
