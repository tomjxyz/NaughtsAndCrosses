using Godot;
using System;
using System.Linq;

public class Gameplay : CanvasLayer
{
    // These two booleans control wether naughts or crosses are humans
    // Passed in when the scene is instantiated
    public bool HumanCross { get; set; }
    public bool HumanNaught { get; set; }

    [Export]
    private int _lineExtension;

    // Boolean that determines whether crosses are currently playing
    // If not naughts must be
    private bool _crossTurn = true;
    private GridBtn[,] _buttons;
    private Drone _dronePlayer;

    private Texture _crosstexture;
    private Texture _naughttexture;

    private Line2D _winLine;
    private Timer _winTimer;
    private Vector2[] _winArray;
    private Label _turnLbl;
    private Timer _droneTimer;


    [Signal]
    public delegate void exit_to_menu();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<Button>("WinMenu/Backing/GridContainer/BackBtn").Connect("pressed", this, "_onBackBtn");
        GetNode<Button>("WinMenu/Backing/GridContainer/RestartBtn").Connect("pressed", this, "_onRestartBtn");

        _crosstexture = (Texture)GD.Load("res://Art/cross.png");
        _naughttexture = (Texture)GD.Load("res://Art/naught.png");
        _buttons = new GridBtn[3,3];
        _dronePlayer = new Drone();

        foreach (GridBtn b in GetNode<GridContainer>("GridTexture/GridContainer").GetChildren()) {
            b.Connect("tapped", this, "onTapped");

            // Assign each button to a 2d array of the relevant positions
            _buttons[(int)b.GridPos.x, (int)b.GridPos.y] = b;
        }
        _turnLbl = GetNode<Label>("TurnLbl");
        _updateTurnLbl();

        _winLine = GetNode<Line2D>("WinLine");
        _winTimer = GetNode<Timer>("WinTimer");
        _winTimer.Connect("timeout", this, "_showWinPopup");

        _droneTimer = GetNode<Timer>("DroneTimer");
        _droneTimer.Connect("timeout", this, "_onDroneTimeout");
        _droneTimer.Stop();
        if (!HumanCross)
            _droneTimer.Start();
    }

    public void onTapped(GridBtn b) {
        if (b.Empty) {
            if (_crossTurn)
                b.setTexture(_crosstexture, true);
            else 
                b.setTexture(_naughttexture, false);
            _afterTurn(b);
        }
        
        // If the user has tapped a valid space
        if (b.Empty && _droneTimer.IsStopped()) {
            if (_crossTurn)
                b.setTexture(_crosstexture, true);
            else 
                b.setTexture(_naughttexture, false);
            _afterTurn(b);
        }
    }

    private void _onDroneTimeout() {
        GridBtn b = _dronePlayer.findMove(_buttons, _crossTurn);
        if (_crossTurn && !HumanCross)
            b.setTexture(_crosstexture, true);
        if (!_crossTurn && !HumanNaught)
            b.setTexture(_naughttexture, false);
        _afterTurn(b);
    }

    private void _afterTurn(GridBtn b) {
        _crossTurn = !_crossTurn;
        b.Empty = false;
        _updateTurnLbl();

        if (_crossTurn && !HumanCross)
            _droneTimer.Start();
        if (!_crossTurn && !HumanNaught)
            _droneTimer.Start();

        _winArray = checkWin();
        // If its not -1 then there is a win or draw
        if( _winArray[0].x != -1 ) {
            // Stop the ai
            _droneTimer.Stop();

            // Disable all buttons
            foreach (GridBtn btn in _buttons) {
                btn.Disabled = true;
            }
            _turnLbl.Visible = false;
            _winLine.Points = _winArray;
            _winLine.Visible = true;
            // Start timer to delay win popup
            _winTimer.Start();
        }

        
    }

    private void _showWinPopup() {
        var winPopup = GetNode<Popup>("WinMenu");
        // Someone won
        if (_winArray[0].x != -2) {
            if (!_crossTurn)
                winPopup.GetNode<Label>("Backing/ResultLbl").Text = "Crosses win";
            else
                winPopup.GetNode<Label>("Backing/ResultLbl").Text = "Naughts win";
        }
        else {  // It was a draw
            winPopup.GetNode<Label>("Backing/ResultLbl").Text = "Draw";
        }
        winPopup.Popup_();
    }

    // Returns an array of vector 2 positions to draw the line of winning to
    public Vector2[] checkWin() {
        Vector2[] positions = new Vector2[3]; 
        bool win = false;

        // Check every row
        for (int x = 0; x < 3; x++) {
            if (_buttons[x,0].GetCross() == _buttons[x,1].GetCross() && _buttons[x,1].GetCross() == _buttons [x,2].GetCross() && !(_buttons[x,0].Empty || _buttons[x,1].Empty || _buttons[x,2].Empty))
            {
                positions[0] = _buttons[x,0].RectGlobalPosition;
                positions[1] = _buttons[x,1].RectGlobalPosition;
                positions[2] = _buttons[x,2].RectGlobalPosition;
                win = true;
            }
        }
        // Check every col
        for (int y = 0; y < 3; y++) {
            if (_buttons[0,y].GetCross() == _buttons[1,y].GetCross() && _buttons[1,y].GetCross() == _buttons [2,y].GetCross() && !(_buttons[0,y].Empty || _buttons[1,y].Empty || _buttons[2,y].Empty))
            {
                positions[0] = _buttons[0,y].RectGlobalPosition;
                positions[1] = _buttons[1,y].RectGlobalPosition;
                positions[2] = _buttons[2,y].RectGlobalPosition;
                win = true;
            }
        }
        // Check diagonal
        if (_buttons[0,0].GetCross() == _buttons[1,1].GetCross() && _buttons[1,1].GetCross() == _buttons[2,2].GetCross() && !(_buttons[0,0].Empty || _buttons[1,1].Empty || _buttons[2,2].Empty)) {
                positions[0] = _buttons[0,0].RectGlobalPosition;
                positions[1] = _buttons[1,1].RectGlobalPosition;
                positions[2] = _buttons[2,2].RectGlobalPosition;
                win = true;
        }
        if (_buttons[0,2].GetCross() == _buttons[1,1].GetCross() && _buttons[1,1].GetCross() == _buttons[2,0].GetCross() && !(_buttons[0,2].Empty || _buttons[1,1].Empty || _buttons[2,0].Empty)) {
                positions[0] = _buttons[0,2].RectGlobalPosition;
                positions[1] = _buttons[1,1].RectGlobalPosition;
                positions[2] = _buttons[2,0].RectGlobalPosition;
                win = true;
        }

        // Change line position to be in the centre of the buttons
        // and add some length to either end
        if (win) {
            for (int i = 0; i < positions.Length; i++){
                var btnSize = _buttons[0,0].RectSize;
                positions[i].x += btnSize.x / 2;
                positions[i].y += btnSize.y / 2;
            }

            // If it's a horizontal line
            if (positions[0].x == positions[1].x && positions [1].x == positions[2].x) {
                positions[0].y -= _lineExtension;
                positions[2].y += _lineExtension;
            }
            // If it's a vertical line
            if (positions[0].y == positions[1].y && positions[1].y == positions[2].y) {
                positions[0].x -= _lineExtension;
                positions[2].x += _lineExtension;
            }

            // If its a diagonal
            if (positions[0].x != positions[1].x && positions[0].y != positions[1].y) {
                positions[0].x -= _lineExtension;
                positions[0].y -= _lineExtension;
                positions[2].x += _lineExtension;
                positions[2].y += _lineExtension;

            }
        }
        else {
            // If all sqaures have a value but still no win
            bool full = true;
            foreach (GridBtn b in _buttons) {
                if (b.Empty) 
                    full = false;
            }
            if (full) {
                // Set value for draw
                positions[0].x = -2;
            }
            else {
                // Set value for no draw or win
                positions[0].x = -1;
            }
        }
        
        return positions;
    }

    private void _onRestartBtn() {
        // Reset game state
        _winLine.Visible = false;
        foreach (GridBtn b in _buttons) {
            b.Empty = true;
            b.setTexture(null, false);
            b.Disabled = false;
        }
        GetNode<Popup>("WinMenu").Visible = false;
        _turnLbl.Visible = true;
        if (_crossTurn && !HumanCross)
            _droneTimer.Start();
        if (!_crossTurn && !HumanNaught)
            _droneTimer.Start();
    }

    // Emits exit signal to be picked up by main node
    private void _onBackBtn() {
        EmitSignal("exit_to_menu");
    }

    private void _updateTurnLbl() {
        if (_crossTurn) {
            _turnLbl.Text = "Cross Turn";
            _turnLbl.Set("custom_colors/font_color", new Color("#d62411"));
        } 
        else {
            _turnLbl.Text = "Naught Turn";
            _turnLbl.Set("custom_colors/font_color", new Color("#68aed4"));
        }

    }
}
