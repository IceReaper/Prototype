namespace Prototype.Scripts.EntityComponents;

using Stride.Engine;
using Systems.Activities;

public sealed class ActivitySystemComponent : SyncScript
{
	private readonly List<Activity> queue = new();

	public void Add(Activity activity)
	{
		this.queue.Add(activity);
	}

	public override void Update()
	{
		var activity = this.queue.FirstOrDefault();

		activity?.Update(this.Game.UpdateTime);

		if (activity?.State == State.Completed)
			this.queue.Remove(activity);
	}

	public override void Cancel()
	{
		foreach (var activity in this.queue.ToArray())
		{
			activity.Cancel();

			if (activity.State == State.Completed)
				this.queue.Remove(activity);
		}
	}
}
