using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// init variables
	// Speed var
	public int Speed = 100;
	// init Animatedsprites var
	private AnimatedSprite2D AnimSprite;
	// init the normalizedvel value
	private Vector2 NormalizedVel;
	// init camera
	private Camera2D PlayerCam;

	// function for sprite control (parameters are the animated sprites and velocity)
	private void PlaySpriteAnim(AnimatedSprite2D Sprite, Vector2 velocity)
	{
		// first check if not moving
		if (velocity == Vector2.Zero)
		{
			// this part works by checking what last animation was being played
			// check if walk down was last anim
			if (AnimSprite.Animation == "walk_down")
			{
				// then play idle down
				AnimSprite.Play("idle_down");
			}
			// check if walk up was last anim
			if (AnimSprite.Animation == "walk_up")
			{
				// then play idle up
				AnimSprite.Play("idle_up");
			}
			// check if walk side r was last anim and if sprite was not flipped horizontally
			if (AnimSprite.Animation == "walk_side_r" && !AnimSprite.FlipH)
			{
				// then play idle right but not flipped
				AnimSprite.Play("idle_side_r");
				AnimSprite.FlipH = false;
			}
			// check if walk side r but flipped was last anim
			if (AnimSprite.Animation == "walk_side_r" && AnimSprite.FlipH)
			{
				// then play idle right but stay flipped
				AnimSprite.Play("idle_side_r");
				AnimSprite.FlipH = true;
			}
		}
		
		// if moving diagonal
		if (NormalizedVel.IsEqualApprox(new Vector2(-1, 1).Normalized())
		|| NormalizedVel.IsEqualApprox(new Vector2(1, -1).Normalized())
		|| NormalizedVel.IsEqualApprox(new Vector2(-1, -1).Normalized())
		|| NormalizedVel.IsEqualApprox(new Vector2(1, 1).Normalized()))
		{
			// seperated diagonal sprite control for future case 
			// when new sprite for diags
			// if moving top right
			if (NormalizedVel.IsEqualApprox(new Vector2(1, -1).Normalized()))
			{
				// if moving top right dont flip sprite
				AnimSprite.FlipH = false;
				// play sprite
				AnimSprite.Play("walk_side_r");
			}
			// if moving top left
			else if (NormalizedVel.IsEqualApprox(new Vector2(-1, -1).Normalized()))
			{
				// if moving top left flip sprite
				AnimSprite.FlipH = true;
				// play sprite
				AnimSprite.Play("walk_side_r");
			}
			// if moving bottom right
			else if (NormalizedVel.IsEqualApprox(new Vector2(1, 1).Normalized()))
			{
				// if bottom top right dont flip sprite
				AnimSprite.FlipH = false;
				// play sprite
				AnimSprite.Play("walk_side_r");
			}
			// if moving bottom left
			else if (NormalizedVel.IsEqualApprox(new Vector2(-1, 1).Normalized()))
			{
				// if moving bottom left flip sprite
				AnimSprite.FlipH = true;
				// play sprite
				AnimSprite.Play("walk_side_r");
			} 
		}

		// check if we are moving only the 4 main directions (cardinal)
		if (NormalizedVel.IsEqualApprox(new Vector2(1, 0).Normalized())
		|| NormalizedVel.IsEqualApprox(new Vector2(0, 1).Normalized())
		|| NormalizedVel.IsEqualApprox(new Vector2(-1, 0).Normalized())
		|| NormalizedVel.IsEqualApprox(new Vector2(0, -1).Normalized()))
		{
			// if moving left and right
			if (Math.Abs(velocity.X) > Math.Abs(velocity.Y))
			{
				// if moving right
				if (velocity.X > 0)
				{
					// if moving right dont flip sprite
					AnimSprite.FlipH = false;
					// play sprite
					AnimSprite.Play("walk_side_r");
				} 
				else if (velocity.X < 0) { // if moving left	
					// flip animated sprite if moving left
					AnimSprite.FlipH = true;
					// play walk right sprite but fipped
					AnimSprite.Play("walk_side_r");
				}
			} else { // else if moving up or down and not left or right
				// if moving up
				if (velocity.Y < 0) // if moving up
				{
					AnimSprite.Play("walk_up");
				}
				else if (velocity.Y > 0) // if moving down
				{
					AnimSprite.Play("walk_down");
				}
			}
		}
	}

	// init stuff with ready
	public override void _Ready()
	{
	// sprite stuff
		// assign animsprite the node
		AnimSprite = GetNode<AnimatedSprite2D>("Sprites");

	// startpos stuff
		// assign node startpos as a vector2
		Vector2 StartPos = GetNode<Marker2D>("StartPos").Position;
		// make another vector2 to store position
		Vector2 position = Vector2.Zero;
		// assign position vector 2 to something in this case startpos
		position = StartPos;
		// assign actual Position to position vector2
		Position = position;

	// assign an initial sprite
		// dont flip horizontally or vertically
		AnimSprite.FlipH = false;
		AnimSprite.FlipV = false;
		// assign idle down as init sprite
		AnimSprite.Play("idle_down");

	// camera stuff
		// assign the node to camera var
		PlayerCam = GetNode<Camera2D>("PlayerCam");
		// enable camera smoothing only after player position has been set
		PlayerCam.SetDeferred("position_smoothing_enabled", true);
	}
	
	// per frame
	public override void _Process(double delta)
	{
		// init velocity and reset every frame
		var velocity = Vector2.Zero;

		// movement of x and y
		if (Input.IsActionPressed("mov_up")) { velocity.Y -= 1; }
		if (Input.IsActionPressed("mov_down")) { velocity.Y += 1; }
		if (Input.IsActionPressed("mov_right")) { velocity.X += 1; }
		if (Input.IsActionPressed("mov_left")) { velocity.X -= 1; }

		// (debug code) GD.Print(velocity);

		// if moving
		if (velocity.Length() != 0)
		{
			// assign normalized vel so we can grab direction
			NormalizedVel = velocity.Normalized();
			// assign the change with speed
			velocity = NormalizedVel * Speed;
			// play animation sprite
			PlaySpriteAnim(AnimSprite, velocity);
		}

		// if not moving
		if (velocity == Vector2.Zero)
		{
			// set normalized vel to zero to avoid bugs
			NormalizedVel = Vector2.Zero;
			// do sprite control
			PlaySpriteAnim(AnimSprite, velocity);
		}

		// change position based on velocity and frame rate
		Position += velocity * (float)delta;
	}
}
