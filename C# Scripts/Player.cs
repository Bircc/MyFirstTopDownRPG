using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	// init variables
	private int Speed = 100;
	// if we need to store velocity temporarily
	private Vector2 velocity;
	private Vector2 Direction;
	private Camera2D PlayerCam;
	private AnimatedSprite2D AnimatedSprites;
	private Marker2D StartPos;
	private RayCast2D FrontDirChecker;
	private Area2D AreaChecker;
	private Node2D RaycastObjCollide;
	private Vector2 LastDirFace;

	private Vector2 Movement()
	{
	// normal movement
		// assign direction as zero each time the function is called
		Direction = Vector2.Zero;
		// handle movement for each cardinal direction
		if (Input.IsActionPressed("mov_up")) { Direction.Y -= 1; }
		if (Input.IsActionPressed("mov_down")) { Direction.Y += 1; }
		if (Input.IsActionPressed("mov_right")) { Direction.X += 1; }
		if (Input.IsActionPressed("mov_left")) { Direction.X -= 1; }

	// drop down to  water platform movement
		// if pressed and colliding with edgeplat
		if (Input.IsActionPressed("drop_down_or_climb")
		&& RaycastObjCollide?.Name == "GroundnWater")
		{
			// debug code GD.Print("should print when i press shift of edge");
		// actual drop down
			// first disable moving so no bugs
			Direction = Vector2.Zero;
			// direction check first to play right animation
			if (LastDirFace == Vector2.Up)
			{

			}
		}

		// return direction
		return Direction;
	}

	// init process
	public override void _Ready()
	{
	// assign vars to nodes
		// assign sprite node
		AnimatedSprites = GetNode<AnimatedSprite2D>("Sprites");
		// assign area2d node
		AreaChecker = GetNode<Area2D>("AreaChecker");
		// assign raycast node
		FrontDirChecker = GetNode<RayCast2D>("FrontDirChecker");
		// assign marker node
		StartPos = GetNode<Marker2D>("StartPos");
		// assign camera node
		PlayerCam = GetNode<Camera2D>("PlayerCam");

	// connect signal to new functions
		// for player area2d
		AreaChecker.Connect("body_entered", new Callable(this, nameof(PlayerEnter)));

	// camera stuff
		// assign camera zoom
		PlayerCam.Zoom = new Vector2(2.5f, 2.5f);
		// enable smoothing
		PlayerCam.PositionSmoothingEnabled = true;
		// adjust smoothing speed
		PlayerCam.PositionSmoothingSpeed = 5f;

		// turn on horizontal and vertical drag
		PlayerCam.DragHorizontalEnabled = true;
		PlayerCam.DragVerticalEnabled = true;
		// set horizontal and drag conditions
		PlayerCam.DragHorizontalOffset = 0f;
		PlayerCam.DragVerticalOffset = 0f;
		// margins for the drag
		PlayerCam.DragTopMargin = 0.001f;
		PlayerCam.DragBottomMargin = 0.001f;
		PlayerCam.DragLeftMargin = 0.001f;
		PlayerCam.DragRightMargin = 0.001f;

	// raycast stuff
		// set position to check more back
		FrontDirChecker = GetNode<RayCast2D>("FrontDirChecker");
		// set length to 20px whatever direction
		FrontDirChecker.TargetPosition = new Vector2(0, 12);
		// set offset to feet area
		FrontDirChecker.Position = new Vector2(0, 13);
		// init raycastobjcollide
		RaycastObjCollide = RaycastCollide();


	// start direction and position of player
		// assign whatever position of the marker
		Vector2 WantPos = Vector2.Zero;
		WantPos.X = 100;
		WantPos.Y = 60;
		StartPos.Position = WantPos;
		// assign position of player to startpos
		Position = StartPos.Position;
		// init facing direction
		AnimatedSprites.Play("idle_down");
	}

	// normal frame process
	public override void _Process(double delta)
	{
	// function to grab last direction facing and return variable vector2 for last direction
		// use return lastdir function to grab LastDirFace Vector2
		LastDirFace = GrabLastDirFace(AnimatedSprites);
		// debug to print last dir just in case its wrong
		GD.Print("Last Direction was: " + LastDirFace);
	// handle sprite control
		// pass direction and animatedsprites node
		SpriteContol(Direction, AnimatedSprites);
	// raycast movement
		RaycastMove();
	// raycast object grabbing
		RaycastObjCollide = RaycastCollide();
		// debug code GD.Print("Raycast checking for collisions colliding with: " + RaycastObjCollide?.Name);
	}

	// physics process every frame
	public override void _PhysicsProcess(double delta)
	{
	// movement
		// direction is equal to Vector2(0, 0) that determines 1, -1 or 0
		Movement(); // returns a Direction var
		// assign velocity as whatever direction multiplied by speed value
		Velocity = Direction.Normalized() * Speed;
		// assign Velocity as a tempvar for checking
		velocity = Velocity;
		// move and slide doesn't take parameters it just bases it on the
		// internal value of Velocity and Vector.Up
		MoveAndSlide();
	// end of movement
		// debug code GD.Print("Velocity:" + Velocity);

	}

	// only accepts two parameters velocity and an animatedsprite2d that is called sprite inside the function
	private void SpriteContol(Vector2 direction, AnimatedSprite2D sprite)
	{
		// if not moving
		if (direction == Vector2.Zero)
		{
		// this sprite contol works by checking last sprite
			// check last sprite
			// up
			if (sprite.Animation == "walk_up")
			{
				sprite.Play("idle_up");
			}
			// down
			if (sprite.Animation == "walk_down")
			{
				sprite.Play("idle_down");
			}
			// left and right same idle sprite
			// just flipped
			if (sprite.Animation == "walk_side_r")
			{
				sprite.Play("idle_side_r");
			}
		}

		// if moving
		if (direction != Vector2.Zero)
		{
			// first check if moving left or right
			if (Math.Abs(direction.X) > Math.Abs(direction.Y))
			{
				// if moving right
				if (direction.X == 1)
				{
					sprite.FlipH = false;
					sprite.Play("walk_side_r");
				}
				else // if moving left
				{
					sprite.FlipH = true;
					sprite.Play("walk_side_r");
				}
			}
			else // moving down or up, overrides left and right
			{
				if (direction.Y == 1)
				{
					sprite.Play("walk_down");
				}
				else
				{
					sprite.Play("walk_up");
				}
			}
		}
	}

// raycast functions
	// function for raycast movement
	private void RaycastMove()
	{
		// while moving do raycast movement
		if (velocity != Vector2.Zero)
		{
			// change angle if moving
			// .Angle returns radians despite its name, subtract 90 degrees for the offset
			FrontDirChecker.Rotation = velocity.Angle() - Mathf.Pi / 2;
		}
	}

	private Node2D RaycastCollide()
	{
		// make local Node2D var collidingobj
		Node2D CollidingObj = null;
		// first check if raycast is colliding
		if (FrontDirChecker.IsColliding())
		{
			// if colliding get that object
			// make colliderobj to store object temporarily for checks
			var ColliderObj = FrontDirChecker.GetCollider();
			// check if it of of Node2D class or subclass and assign it to body
			if (ColliderObj is Node2D body)
			{
				// if it is assign it to the local var
				CollidingObj = body; 
			}
		} // and return to be used
		return CollidingObj;
	}
// player area functions
	// function for player area collision signal
	private void PlayerEnter(Node2D body)
	{
		GD.Print("PlayerArea collision test, Collided with a: " + body.Name);
	}

// grab last dir using sprite only works if standing still because:
	// direction will be 0, 0 and i need a value for last direction
	private Vector2 GrabLastDirFace(AnimatedSprite2D sprite)
	{
		// init direction var
		var LastDirection = Vector2.Zero;
	// make dictionary for sprite and value it returns
		Dictionary<string, Vector2> AnimationDir = new Dictionary
		<string, Vector2> // dictionary containing string of animation for key
		// and vector2 as the direction of that animation
		{
			// make three cases for animation
			{"idle_down", new Vector2(0, 1)},
			{"idle_up", new Vector2(0, -1)},
			{"idle_side_r" , new Vector2(1, 0)}
		};
		// if the animation is that specific animation return the last direction
		if (AnimationDir.TryGetValue(sprite.Animation, out Vector2 value))
		{
			// first check if left
			if (sprite.FlipH)
			{
				// reverse x
				value.X = -value.X;
			}
			LastDirection = value;
		}
		return LastDirection;
	}
}
