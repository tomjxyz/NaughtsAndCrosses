using Godot;
using System;
using System.Collections.Generic;

// This class makes all the decisions for the CPU player
class Drone {

    private GridBtn[,] _buttons;
    private bool _crossTurn;

    private Random _rand = new Random();

    public GridBtn findMove(GridBtn[,] buttons, bool crossTurn) {
        _buttons = buttons;
        _crossTurn = crossTurn;

        var moveResult = moveToWin(crossTurn);
        if (moveResult != null) {
            GD.Print("MoveToWin");
            return moveResult;
        }
        else {
            GD.Print("Random Move");
            return randomMove();
        }
    }

    // This method returns a grid button if there is a move for crosses if parameter = true 
    // Or if there is a move for naughts if param = false
    // Otherwise returns null
    public GridBtn moveToWin(bool forCrosses) {
        GridBtn empty;
        int amountFilled;

        // Check rows
        for (int y = 0; y < 3; y++) {
            amountFilled = 0;
            empty = null;
            for (int x = 0; x < 3; x++) {
                if (_buttons[x, y].Empty)
                    empty = _buttons[x,y];
                else {
                    if (_buttons[x,y].GetCross()  && forCrosses)
                        amountFilled++;
                    if (!_buttons[x,y].GetCross()  && !forCrosses)
                        amountFilled++;
                }
            }
            if (amountFilled == 2)
                return empty;
        }
        
        // Check columns
        for (int y = 0; y < 3; y++) {
            empty = null;
            amountFilled = 0;
            for (int x = 0; x < 3; x++) {
                if (_buttons[y, x].Empty)
                    empty = _buttons[y,x];
                else {
                    if (_buttons[y,x].GetCross()  && forCrosses || !_buttons[y,x].GetCross()  && !forCrosses)
                        amountFilled++;
                }
            }
            if (amountFilled == 2)
                return empty;
        }

        // Check diagonal
        GridBtn[] rightDiag = {_buttons[0,0], _buttons[1,1], _buttons[2,2]};
        GridBtn[] leftDiag = {_buttons[0,2], _buttons[1,1], _buttons[2,0]};

        empty = null;
        amountFilled = 0;
        foreach (GridBtn b in rightDiag) {
            if (b.Empty)
                empty = b;
            else {
                if (b.GetCross() && forCrosses || !b.GetCross() && !forCrosses)
                    amountFilled++;
            }
        }
        if (amountFilled == 2)
            return empty;

        empty = null;
        amountFilled = 0;
        foreach (GridBtn b in leftDiag) {
            if (b.Empty)
                empty = b;
            else {
                if (b.GetCross() && forCrosses || !b.GetCross() && !forCrosses)
                    amountFilled++;
            }
        }
        if (amountFilled == 2)
            return empty;

        return null;
    }
    

    // This move returns a random move near the most consecutive filled buttons
    public GridBtn nearMove() {

        return null;
    }

    // This move picks a random empty space to go
    public GridBtn randomMove() {
        List<GridBtn> emptyBtns = new List<GridBtn>();

        // Get all empty spaces
        foreach (GridBtn b in _buttons) {
            if (b.Empty)
                emptyBtns.Add(b);
        }
        if (emptyBtns.Count > 0)
            return emptyBtns[_rand.Next(0, emptyBtns.Count)];
        else
            return null;
    }

}