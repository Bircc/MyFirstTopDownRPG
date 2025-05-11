using Godot;
using System;

public partial class Player : CharacterBody2D
{
	// init variables
	public int Speed = 100;
	
	
	// init
	public override void _Ready()
	{
		

	}
	
	// per frame
	public override void _Process(double delta)
	{
		// Movement code is here
		// reset velocity per frame
		
		var velocity = Vector2.Zero;
		
		// up
		if (Input.IsActionPressed("mov_up"))
		{
			velocity.Y -= 1;
		}
		// down
		if (Input.IsActionPressed("mov_down"))
		{
			velocity.Y += 1;
		}
		// right
		if (Input.IsActionPressed("mov_right"))
		{
			velocity.X += 1;
		}
		// left
		if (Input.IsActionPressed("mov_left"))
		{
			velocity.X -= 1;
		}
		// if any of the buttons are pressed
		if (velocity.Length() != 0)
		{
			velocity = velocity.Normalized() * Speed;
			var IdleSprite = GetNode<AnimatedSprite2D>("Sprites");
			IdleSprite.Play("idle_down");
		}
		// change position based on velocity and frame rate
		Position += velocity * (float)delta;
	}
}
