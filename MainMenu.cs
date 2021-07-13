using Godot;
using System;

public class MainMenu : CanvasLayer
{

    private TextureButton _crossSelector;
    private TextureButton _naughtSelector;
    private Button _startBtn;

    // Size 2 array, index 0 cross, index 1 naught
    // value true = human, value false = cpu
    private bool[] _playerHuman;
    private bool _crossPressed, _naughtPressed;

    private Texture[] _selectorTextures;

    [Signal]
    public delegate void start_game(bool crossp1, bool crossp2);

    public MainMenu() {
        _crossPressed = false;
        _naughtPressed = false;

        _playerHuman = new bool[2];
        _playerHuman[0] = false;
        _playerHuman[1] = true;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get required nodes
        _crossSelector = GetNode<TextureButton>("GridContainer/CrossSelector");
        _naughtSelector = GetNode<TextureButton>("GridContainer/NaughtSelector");
        _startBtn = GetNode<Button>("StartBtn");

        // Load textures
        _selectorTextures = new Texture[2];
        _selectorTextures[0] = GD.Load<Texture>("res://Art/human.png");
        _selectorTextures[1] = GD.Load<Texture>("res://Art/cpu.png");

        // Connect signals
        _crossSelector.Connect("pressed", this, "_toggleCross");
        _naughtSelector.Connect("pressed", this, "_toggleNaught");
        _startBtn.Connect("pressed", this, "_onStartPressed");
    }

    // Toggle methods change button textures to user's selection and 
    // store selection ready to be passed into start signal
    private void _toggleCross() {
        _crossPressed = true;
        if (_naughtPressed)
            _startBtn.Disabled = false;

        _playerHuman[0] = !_playerHuman[0];
        if (_crossSelector.TextureNormal == _selectorTextures[0]) {
            _crossSelector.TextureNormal = _selectorTextures[1];
        }
        else if (_crossSelector.TextureNormal == _selectorTextures[1]) {
            _crossSelector.TextureNormal = _selectorTextures[0];
        }
        else {
            // Should be human on first run
            _crossSelector.TextureNormal = _selectorTextures[0];
        }
    }
    private void _toggleNaught() {
        _naughtPressed = true;
        if (_crossPressed)
            _startBtn.Disabled = false;

        _playerHuman[1] = !_playerHuman[1];
        if (_naughtSelector.TextureNormal == _selectorTextures[0]) {
            _naughtSelector.TextureNormal = _selectorTextures[1];
        }
        else if (_naughtSelector.TextureNormal == _selectorTextures[1]) {
            _naughtSelector.TextureNormal = _selectorTextures[0];
        }
        else {
            // Should be cpu on first run
            _naughtSelector.TextureNormal = _selectorTextures[1];
        }
    }


    private void _onStartPressed() {
        EmitSignal("start_game", _playerHuman[0], _playerHuman[1]);
    }
}
