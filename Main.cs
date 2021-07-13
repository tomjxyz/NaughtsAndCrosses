using Godot;
using System;

public class Main : Node
{
    private CanvasLayer _mainMenu;
    
    public override void _Ready()
    {
        _mainMenu = GetNode<CanvasLayer>("MainMenu");        
        _mainMenu.Connect("start_game", this, "_startGame");
    }

    private void _startGame(bool crossp1, bool crossp2) {
        // For testing 
        GD.Print("Start game");
    }

}
