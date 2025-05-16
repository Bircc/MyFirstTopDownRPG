using Godot;
using System;


// unorganized code, because this is just a test world to use while making code
// real worlds will be randomly generated with noise
public partial class TestWorld : Node2D
{
    // temp game fullscreen so i can debug
    private bool IsFullscreen = true;

    public override void _Ready()
    {
    // clamp camera to world
        var PlayerCam = GetNode<Camera2D>("Player/PlayerCam");
        // limit camera depending on world size
        // playercamera size is the same as viewport size
        // zoom is altered in code



    // gonna temporarily set camera limit to world limit
        // limit top left to world origin index (0, 0)
        PlayerCam.LimitTop = 0;
        PlayerCam.LimitLeft = 0;
        // limit bottom right to world size
        // get viewport is node function that gets viewport
        // get visible rect is rect2 function that gets the viewport rect
        // then size is a rect2 function that gets the vector2 index of the rect
        // vector2 viewport size usable for future references
        Vector2 ViewPortSize = GetViewport().GetVisibleRect().Size;
        // now limit camera to world which was 1280x720 before but change to 1920x1080
        //1920 x 1080 y 
        PlayerCam.LimitRight = 1920;
        PlayerCam.LimitBottom = 1080;
    }

    public override void _Process(double delta)
    {
        // set screen size
        SetScreenSize();
    }

    // screen size setter (temp)
    private void SetScreenSize()
    {
        if (Input.IsActionJustPressed("toggle_fscreen") && !IsFullscreen)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
            IsFullscreen = true;
        }
        else if (Input.IsActionJustPressed("toggle_fscreen") && IsFullscreen)
        {
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            IsFullscreen = false;
        }
    }

}
