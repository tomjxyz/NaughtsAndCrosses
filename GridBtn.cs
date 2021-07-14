using Godot;
using System;

public class GridBtn : Control
{
    [Export]
    public Vector2 GridPos;
    public bool Empty;
    private bool _cross;

    public bool Disabled {
        set { this.GetChild<TextureButton>(0).Disabled = value; }
        get { return this.GetChild<TextureButton>(0).Disabled; } 
    }

    [Signal]
    public delegate void tapped(GridBtn g);
    public override void _Ready()
    {
        Empty = true;
        TextureButton b = this.GetChild<TextureButton>(0);
        b.Connect("pressed", this, "onBtnPressed");
    }

    public void onBtnPressed() {
        EmitSignal("tapped", this);
    }

    public void setTexture(Texture t, bool cross) {
        GetNode<TextureButton>("TextureButton").TextureNormal = t;
        _cross = cross;
    }

    public bool GetCross() {
        return _cross;
    }

}
