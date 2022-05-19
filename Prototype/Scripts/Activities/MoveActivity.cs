namespace Prototype.Scripts.Activities;

using Entities;
using EntityComponents;
using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Systems.Activities;
using Systems.Navigation;

public sealed class MoveActivity : Activity
{
	private const int UnstuckTries = 20;
	private const double UnstuckDelay = .25;

	private readonly Entity entity;
	private readonly Vector3 target;
	private readonly GridComponent? gridComponent;
	private readonly ReserveCellComponent? reserveCellComponent;

	private readonly List<Cell> path = new();
	private int unstuckTry;
	private double unstuckDelay;

	public MoveActivity(Entity entity, Vector3 target)
	{
		this.entity = entity;
		this.target = target;

		this.gridComponent = this.entity.Scene.Entities.FirstOrDefault(nameof(WorldGrid))?.Components.FirstOrDefault<GridComponent>();
		this.reserveCellComponent = this.entity.Components.FirstOrDefault<ReserveCellComponent>();
	}

	protected override void UpdateInner(GameTime updateTime)
	{
		if (this.gridComponent == null)
		{
			this.Complete();

			return;
		}

		if (this.unstuckDelay > 0)
		{
			this.unstuckDelay -= Math.Min(updateTime.Elapsed.TotalSeconds, this.unstuckDelay);

			if (this.unstuckDelay > 0)
				return;
		}

		if (this.path.Count == 0)
		{
			if (this.State != State.Canceled)
				this.path.AddRange(this.gridComponent.FindPath(this.entity.Transform.Position, this.target));

			if (this.path.Count == 0)
				this.Complete();
		}

		var remainingDistance = (float)(CharacterComponent.MoveSpeed * updateTime.Elapsed.TotalSeconds);

		while (remainingDistance > 0 && this.path.Count > 0)
		{
			var cell = this.path[0];
			var cellPosition = new Vector3(cell.X, cell.Y, cell.Z);

			if (!cell.Occupiers.Contains(this.entity))
			{
				if (cell.Occupiers.Count == 0)
				{
					cell.Occupiers.Add(this.entity);
					this.unstuckTry = 0;
				}
				else
				{
					this.path.Clear();

					this.unstuckTry++;
					this.unstuckDelay = MoveActivity.UnstuckDelay;

					if (this.unstuckTry == MoveActivity.UnstuckTries)
						this.Complete();

					return;
				}
			}

			var distanceToTarget = (cellPosition - this.entity.Transform.Position).Length();
			var direction = Vector3.Normalize(cellPosition - this.entity.Transform.Position);
			var moveDistance = remainingDistance;

			if (remainingDistance >= distanceToTarget)
			{
				moveDistance = distanceToTarget;
				this.path.RemoveAt(0);

				if (this.State == State.Canceled)
					this.path.Clear();
			}

			this.entity.Transform.Position += direction * moveDistance;
			remainingDistance -= moveDistance;
			this.reserveCellComponent?.Update();
		}
	}
}
