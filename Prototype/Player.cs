namespace Prototype;

using Components;
using Irony.Parsing;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Extensions;
using Stride.Graphics.GeometricPrimitives;
using Stride.Input;
using Stride.Rendering;

public class Player : SyncScript
{
	private Model? playerModel;
	
	private Vector3 movementEnd;
	private WorldCursorComponent cameraWithVM;

	private float movespeed = 5f;
	private float elapsedTime = 0;

	private List<Vector3> path;

	public override void Start()
	{
		this.cameraWithVM = this.SceneSystem.SceneInstance.RootScene.Entities.SelectMany(entity => entity.Components.OfType<WorldCursorComponent>()).FirstOrDefault();
		this.CreatePlayerModel();
		this.Entity.Transform.Position = new(118, 3, 211); 
		this.movementEnd = this.Entity.Transform.Position;
		this.path = new();
	}

	public override void Update()
	{
		if (this.Input.IsMouseButtonPressed(MouseButton.Left))
		{
			//Console.WriteLine("Virtualmouse on: " + this.cameraWithVM.VirtualMousePosition); //TODO: Delete this
			this.movementEnd = this.cameraWithVM.VirtualMousePosition;
			this.path.Add(this.movementEnd);
		}
		
		if (this.Input.IsMouseButtonPressed(MouseButton.Right))
		{
			if (this.path != null)
				this.path.Clear();
			Console.WriteLine("Path has been cleared");
		}
		
		if (this.cameraWithVM.VirtualMousePosition != null)
		{
			//this.Movement(); 
			//Console.WriteLine("Entity is at: " + this.Entity.Transform.Position);
			this.UseWayPoints();
		}

	}
	
	public void CreatePlayerModel()
	{
		this.playerModel = new();
		this.Entity.GetOrCreate<ModelComponent>().Model = this.playerModel;		
		this.playerModel.Meshes.Add(new() {Draw = GeometricPrimitive.Capsule.New(this.GraphicsDevice,4f,1f,1).ToMeshDraw()});
	}

	private void Movement() //direct movement without waypoints
	{
		var deltaTime = (float)this.Game.UpdateTime.Elapsed.TotalSeconds;

		var start = this.Entity.Transform.Position;
		var end = this.movementEnd;

		var distance = (end - start).Length();
		var direction = Vector3.Normalize(end - start);

		this.Entity.Transform.Position += direction * Math.Min(this.movespeed * deltaTime, distance);
	}

	private void UseWayPoints()
	{
		if (this.path.Count == 0)
			return;

		var deltaTime = (float)this.Game.UpdateTime.Elapsed.TotalSeconds;

		var start = this.Entity.Transform.Position;
		var end = this.path[0];

		var distance = (end - start).Length();
		var direction = Vector3.Normalize(end - start);

		this.Entity.Transform.Position += direction * Math.Min(this.movespeed * deltaTime, distance);
            
		if (distance == 0)
			this.path.RemoveAt(0);
	}

	private void UseWayPointsWhile()
	{
		while (this.path.Count > 0)
		{
			var deltaTime = (float)this.Game.UpdateTime.Elapsed.TotalSeconds;

			var start = this.Entity.Transform.Position;
			var end = this.path[0];

			var distance = (end - start).Length();
			var direction = Vector3.Normalize(end - start);

			if (distance == 0)
				this.path.RemoveAt(0);
			else
			{
				this.Entity.Transform.Position += direction * Math.Min(this.movespeed * deltaTime, distance);
				break;
			}
		}
	}
}
