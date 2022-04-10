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
	private const int RetryCount = 5;
	private const double RetryAfter = 1;
	private const double MaxUnstuckTime = .25;

	private readonly Entity entity;
	private readonly Vector3 target;
	private readonly List<Vector3> path = new();
	private double retryTime;
	private int retryCount;

	public MoveActivity(Entity entity, Vector3 target)
	{
		this.entity = entity;
		this.target = target;
	}

	protected override void UpdateInner(GameTime updateTime)
	{
		if (this.State == State.Canceled)
		{
			this.Complete();

			return;
		}

		var gridComponent = this.entity.Scene.Entities.FirstOrDefault(nameof(WorldGrid))?.Components.FirstOrDefault<GridComponent>();

		if (gridComponent == null)
		{
			this.Complete();

			return;
		}

		if (this.path.Count > 0 && !gridComponent.CanTransitionToCell(this.entity.Transform.Position, this.path[0]))
		{
			this.retryTime = new Random().NextDouble() * MoveActivity.MaxUnstuckTime;
			this.path.Clear();
		}

		if (this.path.Count == 0)
		{
			if (this.retryTime > 0)
			{
				this.retryTime -= Math.Min(updateTime.Elapsed.TotalSeconds, this.retryTime);

				if (this.retryTime > 0)
					return;
			}

			this.path.AddRange(gridComponent.FindPath(this.entity.Transform.Position, this.target));

			if (this.path.Count == 0)
			{
				if (this.retryCount == MoveActivity.RetryCount)
					this.Complete();
				else
				{
					this.retryTime = MoveActivity.RetryAfter;
					this.retryCount++;
				}

				return;
			}

			this.retryCount = 0;
		}

		var nextCell = this.path[0];
		this.path.RemoveAt(0);

		var transitionActivity = new TransitionCellActivity(this.entity, nextCell);
		this.Add(transitionActivity);

		transitionActivity.Update(updateTime);
	}
}
