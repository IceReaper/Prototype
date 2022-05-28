namespace Prototype.Systems.Activities;

using Stride.Games;

public abstract class Activity
{
	private readonly List<Activity> children = new();
	public State State { get; private set; } = State.Queued;

	protected void Complete()
	{
		this.State = State.Completed;
	}

	protected void Add(Activity activity)
	{
		this.children.Add(activity);
	}

	public void Update(GameTime updateTime)
	{
		var first = this.State == State.Queued;

		if (first)
			this.State = State.Started;

		var child = this.children.FirstOrDefault();

		if (child != null)
		{
			child.Update(updateTime);

			if (child.State == State.Completed)
				this.children.Remove(child);
		}
		else
		{
			if (first)
				this.Start();

			this.UpdateInner(updateTime);
		}
	}

	protected virtual void Start()
	{
	}

	protected virtual void UpdateInner(GameTime updateTime)
	{
		this.Complete();
	}

	public void Cancel()
	{
		foreach (var child in this.children.ToArray())
		{
			child.Cancel();

			if (child.State == State.Completed)
				this.children.Remove(child);
		}

		switch (this.State)
		{
			case State.Queued:
				if (this.children.Any())
					this.Complete();

				break;

			case State.Started:
				this.State = State.Canceled;
				this.CancelInner();

				break;

			case State.Canceled:
			case State.Completed:
				break;

			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	protected virtual void CancelInner()
	{
		this.Complete();
	}
}
