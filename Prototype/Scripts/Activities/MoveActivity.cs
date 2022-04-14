namespace Prototype.Scripts.Activities;

using EntityComponents;
using Extensions;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Systems.Activities;
using Systems.Entities;

public class MoveActivity : Activity
{
	private const int UnstuckTries = 20;
	private const double UnstuckDelay = .25;

	private readonly Entity entity;
	private readonly Vector3 target;
	private readonly GridComponent? gridComponent;
	private readonly ReserveCellComponent? reserveCellComponent;

	private readonly List<Vector3> path = new();
	private int unstuckTry;
	private double unstuckDelay;
	private bool reserved;

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

		var remainingDistance = (float)(CharacterComponent.Movespeed * updateTime.Elapsed.TotalSeconds);

		while (remainingDistance > 0 && this.path.Count > 0)
		{
			var target = this.path[0];

			if (!this.reserved)
			{
				if (this.gridComponent.CanTransitionToCell(target, this.entity))
				{
					this.gridComponent.ReserveCell(target, this.entity);
					this.reserved = true;
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

			var distanceToTarget = (target - this.entity.Transform.Position).Length();
			var direction = Vector3.Normalize(target - this.entity.Transform.Position);
			var moveDistance = remainingDistance;

			if (remainingDistance >= distanceToTarget)
			{
				moveDistance = distanceToTarget;
				this.path.RemoveAt(0);
				this.reserved = false;

				if (this.State == State.Canceled)
					this.path.Clear();
			}

			this.entity.Transform.Position += direction * moveDistance;
			remainingDistance -= moveDistance;
			this.reserveCellComponent?.Update();
		}
	}
}
