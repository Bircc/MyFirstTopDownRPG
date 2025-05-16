using Godot;
using System;

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

    // drop down platform movement
        // if pressed and colliding with edgeplat
        if (Input.IsActionPressed("drop_down_or_climb"))
        {
        }

        // return direction
        return Direction;

    }

    // init process
    public override void _Ready()
    {
    // camera stuff
        // assign camera node
        PlayerCam = GetNode<Camera2D>("PlayerCam");
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
        FrontDirChecker.TargetPosition = new Vector2(0, 20);

    // start position of player
        // assign var to node
        StartPos = GetNode<Marker2D>("StartPos");
        // assign whatever position of the marker
        Vector2 WantPos = Vector2.Zero;
        WantPos.X = 100;
        WantPos.Y = 60;
        StartPos.Position = WantPos;
        // assign position of player to startpos
        Position = StartPos.Position;

    // assign vars to nodes
        // assign sprite node
        AnimatedSprites = GetNode<AnimatedSprite2D>("Sprites");
    }

    // normal frame process
    public override void _Process(double delta)
    {
        // handle sprite control
        // pass direction and animatedsprites node
        SpriteContol(Direction, AnimatedSprites);

    }

    // physics process every frame
    public override void _PhysicsProcess(double delta)
    {
    // returns a Direction var
        // direction is equal to Vector2(0, 0) that determines 1, -1 or 0
        Movement();
        // assign velocity as whatever direction multiplied by speed value
        Velocity = Direction.Normalized() * Speed;
        // assign Velocity as a tempvar for checking
        velocity = Velocity;
        // move and slide doesn't take parameters it just bases it on the
        // internal value of Velocity and Vector.Up
        MoveAndSlide();
        // debug // code GD.Print("Velocity:" + Velocity);
    }

    // only accepts two parameters velocity and an animatedsprite2d
    // that is called sprite inside the function
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

}
