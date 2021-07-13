using Godot;
using System;
using System.Linq;

public class Gameplay : CanvasLayer
{
    // Boolean that determines whether crosses are currently playing
    // If not naughts must be
    private bool _crossTurn;
    private GridBtn[,] _buttons;

    private Texture _crosstexture;
    private Texture _naughttexture;
    private Line2D _winLine;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _crosstexture = (Texture)GD.Load("res://Art/cross.png");
        _naughttexture = (Texture)GD.Load("res://Art/naught.png");
        _buttons = new GridBtn[3,3];

        foreach (GridBtn b in GetNode<GridContainer>("GridContainer").GetChildren()) {
            b.Connect("tapped", this, "onTapped");

            // Assign each button to a 2d array of the relevant positions
            _buttons[(int)b.GridPos.x, (int)b.GridPos.y] = b;
        }
        _winLine = GetNode<Line2D>("WinLine");
        var restartBtn = GetNode<Button>("RestartBtn");
        restartBtn.Connect("pressed", this, "_onRestartBtn");
    }

    [Signal]
    public delegate void player_won(bool crosses);

    public void onTapped(GridBtn b) {
        if (b.Empty) {
            if (_crossTurn)
                b.setTexture(_crosstexture, true);
            else
                b.setTexture(_naughttexture, false);
            _crossTurn = !_crossTurn;
            b.Empty = false;
        }
        var winArray = checkWin();
        // If its not -1 then there was a win
        if( winArray[0].x != -1 ) {
            _winLine.Points = winArray;
            _winLine.Visible = true;
            GetNode<Button>("RestartBtn").Visible = true;
            EmitSignal("player_won", _crossTurn);
        }
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
        if (win) {
            for (int i = 0; i < positions.Length; i++){
                var btnSize = _buttons[0,0].RectSize;
                positions[i].x += btnSize.x / 2;
                positions[i].y += btnSize.y / 2;
            }
        }
        else {
            // Set an impossible value
            positions[0].x = -1;
        }
        
        // Otherwise return no win
        return positions;
    }

    private void _onRestartBtn() {
        _winLine.Visible = false;
        foreach (GridBtn b in _buttons) {
            b.Empty = true;
            b.setTexture(null, false);
        }
        GetNode<Button>("RestartBtn").Visible = false;
    }
}
