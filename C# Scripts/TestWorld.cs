using Godot;
using System;


// unorganized code, because this is just a test world to use while making code
// real worlds will be randomly generated with noise
public partial class TestWorld : Node2D
{
    public override void _Ready()
    {
    // clamp camera to world
        var PlayerCam = GetNode<Camera2D>("Player/PlayerCam");
        // limit camera depending on world size
        // playercamera size is the same as viewport size
        // zoom is altered in code

        // limit top left to world origin index (0, 0)
        PlayerCam.LimitTop = 0;
        PlayerCam.LimitLeft = 0;
        // limit bottom right to world size
        // get viewport is node function that gets viewport
        // get visible rect is rect2 function that gets the viewport rect
        // then size is a rect2 function that gets the vector2 index of the rect
        Vector2 ViewPortSize = GetViewport().GetVisibleRect().Size;
        // now limit camera to world which is 1280x720
        //1280 x and 720 y
        PlayerCam.LimitRight = (int)ViewPortSize.X;
        PlayerCam.LimitBottom = (int)ViewPortSize.Y;
    }

}
