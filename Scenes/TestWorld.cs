using Godot;
using System;
using System.Drawing;

public partial class TestWorld : Node2D
{
    public override void _Ready()
    {
    // stuff to make world collision for world edge based on the screen size    
        // init var for MapArea Node
        var MapArea = GetNode<Area2D>("MapArea");
        // init var for MapZone node
        var MapZone = GetNode<CollisionShape2D>("MapArea/MapZone");
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
    }
}



