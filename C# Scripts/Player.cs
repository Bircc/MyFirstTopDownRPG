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
	private bool IsDropping;
	private List<TileData> ListPlayerTiles;
	private bool InWater;
	private bool InGround;
	private bool CanSwim;


	private Vector2 Movement()
	{
	// normal movement
		// assign direction as zero each time the function is called
		Direction = Vector2.Zero;
		// set new params before you can move like if you are dropping, etc.
		if (!IsDropping)
		{
		// handle movement for each cardinal direction
			if (Input.IsActionPressed("mov_up")) { Direction.Y -= 1; }
			if (Input.IsActionPressed("mov_down")) { Direction.Y += 1; }
			if (Input.IsActionPressed("mov_right")) { Direction.X += 1; }
			if (Input.IsActionPressed("mov_left")) { Direction.X -= 1; }
		}
	// drop down to  water platform movement
		// if pressed and colliding with edgeplat
		if (Input.IsActionPressed("drop_down_or_climb")
		&& RaycastObjCollide?.Name == "GroundnWater")
		{
			// debug code GD.Print("should print when i press q on edge");
		// actual drop down
			// direction check first to play right animation
			if (LastDirFace == Vector2.Up)
			{
				GrabLerpPos(LastDirFace);
				SpriteContol(LastDirFace, AnimatedSprites);
			}
			if (LastDirFace == Vector2.Down)
			{
				GrabLerpPos(LastDirFace);
				SpriteContol(LastDirFace, AnimatedSprites);
			}
			if (LastDirFace == Vector2.Right)
			{
				GrabLerpPos(LastDirFace);
				SpriteContol(LastDirFace, AnimatedSprites);
			}
			if (LastDirFace == Vector2.Left)
			{
				GrabLerpPos(LastDirFace);
				SpriteContol(LastDirFace, AnimatedSprites);
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


	// start direction and position of etc. player stuff
		// enable y sort
		YSortEnabled = true;
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
		GD.Print("Last Direction Facing was: " + LastDirFace);
	// handle sprite control
		// pass direction and animatedsprites node
		SpriteContol(Direction, AnimatedSprites);
	// raycast movement
		RaycastMove();
	// raycast object grabbing
		RaycastObjCollide = RaycastCollide();
		// debug code GD.Print("Raycast checking for collisions colliding with: " + RaycastObjCollide?.Name);
	// grab list of player tiles
		ListPlayerTiles = PlayerTiles();
	// If in certain tiles do certain things
		UseTileType();
	}

	// physics process every frame
	public override void _PhysicsProcess(double delta)
	{
	// dropping to water

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
		// use dictionaries for shorter not moving checker
		Dictionary<string, string> IdleChecker = new Dictionary<string, string>
		{
			{"walk_up", "idle_up"},
			{"walk_down", "idle_down"},
			{"walk_side_r", "idle_side_r"}
		};

		// if not moving and not dropping, cant swim
		if (direction == Vector2.Zero && !IsDropping && !CanSwim)
		{
			// get key value
			if (IdleChecker.TryGetValue(sprite.Animation, out String IdleAnimation))
			{
				sprite.Play(IdleAnimation);
			}
		}

		// if moving and not dropping, cant swim
		if (direction != Vector2.Zero && !IsDropping && !CanSwim)
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
		if (IsDropping) // if dropping check for sprite to play based on lastdirface
		{	
			// use dictionaries for the checking of sprite to play again
			Dictionary<Vector2, String> DropAnimCheck= new Dictionary<Vector2, String>
			{
				// make key values
				{Vector2.Up, "jump_water_up"},
				{Vector2.Down, "jump_water_down"},
				{Vector2.Right, "jump_water_side_r"},
				{Vector2.Left, "jump_water_side_r"}
			};
			// check if correlated
			if (DropAnimCheck.TryGetValue(direction, out String DropAnimToPlay))
			{
				sprite.Play(DropAnimToPlay);
			}
		}

		// if you can swim use swim anim
		if (CanSwim)
		{
			sprite.Play("in_water");
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
// function for grabbing list of playertiles which tile the player is standing on
	private List<TileData> PlayerTiles()
	{
		// make new list
		var TileDataList = new List<TileData>();
		// get parent node for all tilemaplayers
		var TileMapLayParents = GetNode<Node2D>("../WorldObjects");
		// loop through parent for all child and put it in var TileMapLay
		foreach (var TileMapLay in TileMapLayParents.GetChildren())
		{
			// if TileMapLay is of TileMapLayer origin, call it Tilemap to use it
			if (TileMapLay is TileMapLayer Tilemap)
			{
				// we are now looping through each TileMapLayer Node
				// first grab player node
				Vector2 PlayerGlobalPos = GlobalPosition;
				// use GlobalPosition to grab the localpos of TileMapLayer node
				// since tilemaplayers are children of main node, this won't matter
				// but its good practice?
				Vector2 TileMapPos = Tilemap.ToLocal(GlobalPosition);
				// convert that local basis for converting it to tilemap coordinates
				Vector2I TileCoords = Tilemap.LocalToMap(TileMapPos);
				// use those coordinates to get the tilemapdata
				TileData TileMapData = Tilemap.GetCellTileData(TileCoords);
				if (TileMapData != null)
				{
					TileDataList.Add(TileMapData);
				}
			}
		}
		return TileDataList;
	}

	private void UseTileType()
	{
		    // Reset flags at the start of each call
		InWater = false;
		InGround = false;
		CanSwim = false;
		
		// iterate through tiles and grab tile type
		foreach (TileData Tile in ListPlayerTiles)
		{
			// extract as string because C# can't do it itself
			var GrabType = (String)Tile.GetCustomData("TileType");
			
			// if its water
			if (GrabType == "water")
			{
				InWater = true;
			}
			// if its ground
			if (GrabType == "ground")
			{
				InGround = true;
			}
		}

		// lazy state machine
		// check if both water n ground
		if (InWater && InGround)
		{
			// cant swim
			CanSwim = false;
		}
		// if only water
		else if (InWater)
		{
			// can swim
			CanSwim = true;
		}
	}
	private void GrabLerpPos(Vector2 DirectionOfDrop) 
	{
		// disable the collision mask for now
		CollisionShape2D CollisionMask = GetNode<CollisionShape2D>("CollisionMask");
		CollisionMask.Disabled = true;

		// set is dropping to true for movement disabler
		IsDropping = true;

		// also set variable to be used
		Vector2 StartPos = Position;
		// end is just adding the direction multiplied by some value for the jump space
		Vector2 EndPos = StartPos + DirectionOfDrop * 20;
	
		// use tweening for easier transition
		var Tween = CreateTween();
		Tween.TweenProperty(this, "position", EndPos, 1).SetEase(Tween.EaseType.Out);
		// tween auto destroys itself

		// this just insta calls the finish signal here without making new object and calling it
		Tween.Finished += () => {
			CollisionMask.Disabled = false;
			IsDropping = false;
		};
	}
}
