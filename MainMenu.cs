using Godot;
using System;

public class MainMenu : CanvasLayer
{

    private TextureButton _crossSelector;
    private TextureButton _naughtSelector;
    private Button _startBtn;

    [Signal]
    public delegate void start_game(bool crossp1, bool crossp2);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get required nodes
        _crossSelector = GetNode<TextureButton>("GridContainer/CrossSelector");
        _naughtSelector = GetNode<TextureButton>("GridContainer/NaughtSelector");
        _startBtn = GetNode<Button>("StartBtn");

        // Connect signals
        _crossSelector.Connect("pressed", this, "_toggleCross");
        _naughtSelector.Connect("pressed", this, "_toggleNaught");
        _startBtn.Connect("pressed", this, "_onStartPressed");
    }

    private void _toggleCross() {
    }
    private void _toggleNaught() {
    }

    private void _onStartPressed() {
    }
}
