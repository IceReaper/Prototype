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

	public void Add(Activity activity)
	{
		this.children.Add(activity);
	}

	public void Update(GameTime updateTime)
	{
		if (this.State == State.Queued)
			this.State = State.Started;

		var child = this.children.FirstOrDefault();

		if (child != null)
		{
			child.Update(updateTime);

			if (child.State == State.Completed)
				this.children.Remove(child);
		}
		else
			this.UpdateInner(updateTime);
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

		if (this.State == State.Started)
		{
			this.State = State.Canceled;
			this.CancelInner();
		}
		else
			this.Complete();
	}

	protected virtual void CancelInner()
	{
	}
}
