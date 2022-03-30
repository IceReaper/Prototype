﻿namespace Prototype.Entities.Components;

using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;
using Vortice.Mathematics;

public class CameraControllerComponent : SyncScript
{
	private const int Angle = -60;
	private const int ZoomMin = 10;
	private const int ZoomMax = 60;
	private const int RotateDistance = 8;
	private const int PanSpeed = 15;
	private const int PanFastFactor = 2;
	private const int PanBorder = 10;
	private const int RotateSpeed = 2;
	private const int MousePanFactor = 100;
	private const int MouseRotateFactor = 100;

	private CameraComponent? cameraComponent;
	private float rotation;

	public override void Start()
	{
		this.cameraComponent = this.Entity.Components.OfType<CameraComponent>().FirstOrDefault();
		base.Start();
	}

	public override void Update()
	{
		if (!this.Game.IsActive)
			return;

		var deltaTime = (float)this.Game.UpdateTime.Elapsed.TotalSeconds;
		var movement = Vector2.Zero;
		var rotation = 0f;
		var zoom = 0f;

		if (this.Input.HasGamePad)
		{
			var padState = this.Input.DefaultGamePad.State;

			var gamePadDir = padState.LeftThumb;

			if ((padState.Buttons & GamePadButton.PadUp) != 0)
				zoom -= 1;

			if ((padState.Buttons & GamePadButton.PadDown) != 0)
				zoom += 1;

			if ((padState.Buttons & GamePadButton.PadLeft) != 0)
				rotation -= 1;

			if ((padState.Buttons & GamePadButton.PadRight) != 0)
				rotation += 1;

			if ((padState.Buttons & GamePadButton.LeftThumb) != 0)
				gamePadDir *= CameraControllerComponent.PanFastFactor;

			movement += gamePadDir;
		}

		if (this.Input.HasKeyboard)
		{
			var keyboardDir = new Vector2();

			if (this.Input.IsKeyDown(Keys.W) || this.Input.IsKeyDown(Keys.Up))
				keyboardDir.Y -= 1;

			if (this.Input.IsKeyDown(Keys.S) || this.Input.IsKeyDown(Keys.Down))
				keyboardDir.Y += 1;

			if (this.Input.IsKeyDown(Keys.A) || this.Input.IsKeyDown(Keys.Left))
				keyboardDir.X -= 1;

			if (this.Input.IsKeyDown(Keys.D) || this.Input.IsKeyDown(Keys.Right))
				keyboardDir.X += 1;

			if (this.Input.IsKeyDown(Keys.R))
				zoom += 1;

			if (this.Input.IsKeyDown(Keys.F))
				zoom -= 1;

			if (this.Input.IsKeyDown(Keys.Q))
				rotation -= 1;

			if (this.Input.IsKeyDown(Keys.E))
				rotation += 1;

			if (this.Input.IsKeyDown(Keys.LeftShift) || this.Input.IsKeyDown(Keys.RightShift))
				keyboardDir *= CameraControllerComponent.PanFastFactor;

			movement += keyboardDir;
		}

		if (this.Input.HasMouse)
		{
			var mouseDir = new Vector2();

			if (this.Input.IsMouseButtonDown(MouseButton.Right))
			{
				this.Input.LockMousePosition();
				this.Game.IsMouseVisible = false;

				mouseDir += this.Input.MouseDelta * CameraControllerComponent.MousePanFactor;
			}
			else
			{
				this.Input.UnlockMousePosition();
				this.Game.IsMouseVisible = true;

				var mousePosition = new Vector2(
					this.Game.Window.ClientBounds.Size.Width * this.Input.Mouse.Position.X,
					this.Game.Window.ClientBounds.Size.Height * this.Input.Mouse.Position.Y
				);

				if (mousePosition.X < CameraControllerComponent.PanBorder)
					mouseDir.X -= 1;

				if (mousePosition.X > this.Game.Window.ClientBounds.Size.Width - CameraControllerComponent.PanBorder)
					mouseDir.X += 1;

				if (mousePosition.Y < CameraControllerComponent.PanBorder)
					mouseDir.Y -= 1;

				if (mousePosition.Y > this.Game.Window.ClientBounds.Size.Height - CameraControllerComponent.PanBorder)
					mouseDir.Y += 1;
			}

			zoom += this.Input.MouseWheelDelta;

			if (this.Input.IsMouseButtonDown(MouseButton.Middle))
				rotation += this.Input.MouseDelta.X * CameraControllerComponent.MouseRotateFactor;

			movement += mouseDir;
		}

		var finalRotation = rotation * deltaTime * CameraControllerComponent.RotateSpeed;
		var finalMovement = new Vector3(movement.X, 0, movement.Y) * deltaTime * CameraControllerComponent.PanSpeed;

		this.Entity.Transform.Position -= Vector3.Transform(
			new(0, 0, CameraControllerComponent.RotateDistance),
			Quaternion.RotationYawPitchRoll(this.rotation, 0, 0)
		);

		this.rotation -= finalRotation;

		this.Entity.Transform.Position += Vector3.Transform(
			new(0, 0, CameraControllerComponent.RotateDistance),
			Quaternion.RotationYawPitchRoll(this.rotation, 0, 0)
		);

		this.Entity.Transform.Position += Vector3.Transform(finalMovement, Quaternion.RotationYawPitchRoll(this.rotation, 0, 0));
		this.Entity.Transform.Rotation = Quaternion.RotationYawPitchRoll(this.rotation, MathHelper.ToRadians(CameraControllerComponent.Angle), 0);

		if (this.cameraComponent != null)
		{
			this.cameraComponent.VerticalFieldOfView = Math.Clamp(
				this.cameraComponent.VerticalFieldOfView - zoom,
				CameraControllerComponent.ZoomMin,
				CameraControllerComponent.ZoomMax
			);
		}
	}
}
