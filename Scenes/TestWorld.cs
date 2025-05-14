using Godot;
using System;
using System.Drawing;
using System.Threading.Tasks;


// just a test world for writing code functions before making open world system
// will mostly have spaghetti code because im new (pls forgive me)
public partial class TestWorld : Node2D
{
    private Area2D MapArea;
    private CollisionShape2D MapZone;
    private CharacterBody2D Player;


    public override void _Ready()
    {
    // player stuff
        // init player var into noded
        Player = GetNode<CharacterBody2D>("Player");    

    // stuff to make world collision for world edge based on the screen size    
        // init var for MapArea Node
        MapArea = GetNode<Area2D>("MapCenterArea");
        // init var for MapZone node
        MapZone = GetNode<CollisionShape2D>("MapCenterArea/MapZone");
        // get window size here so its dynamic and changes
        var WindowSize = DisplayServer.WindowGetSize();
        // put MapArea node in the center of the window
        MapArea.Position = WindowSize / 2;
        // assign size of MapZone
        // first make a rectangle shape with same size as window
        var RectZoneShape = new RectangleShape2D();
        RectZoneShape.Size = WindowSize;
        // assign shape of MapZone to the made rectshape
        MapZone.Shape = RectZoneShape;

    // make new onbodyentered signal for map
    MapArea.Connect("body_entered", new Callable(this, "MapOnBodyEntered"));
    // make new onbodyexit signal for map
    MapArea.Connect("body_exited", new Callable(this, "MapOnBodyExited"));

    // collision for player and map
        // 
    }

    // way to detect that the player exited or entered the map once

    public override void _Process(double delta)
    {
        /*
        // way to detect that the player is constantly in the map or not
        // another way is to use signals which is better in some scenarios
        if (MapArea.OverlapsBody(Player))
        {
            GD.Print("Player is currently in the map.");
        } else {
            GD.Print("Player is no longer in the map.");
        }
        */
    }
}



