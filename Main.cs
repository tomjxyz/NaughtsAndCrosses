using Godot;
using System;

public class Main : Node
{
    private CanvasLayer _mainMenu;
    private PackedScene _gameplayScene;
    
    public override void _Ready()
    {
        _mainMenu = GetNode<CanvasLayer>("MainMenu");        
        _mainMenu.Connect("start_game", this, "_startGame");

        _gameplayScene = GD.Load<PackedScene>("res://Gameplay.tscn");
    }

    private void _startGame(bool humanp1, bool humanp2) {
        Gameplay gameplayInstance = _gameplayScene.Instance<Gameplay>();

        gameplayInstance.HumanCross = humanp1;
        gameplayInstance.HumanNaught = humanp2;
        gameplayInstance.Connect("exit_to_menu", this, "_onBackSignal");

        AddChild(gameplayInstance);
    }

    private void _onBackSignal() {
        GetNode<CanvasLayer>("Gameplay").QueueFree();
    }

}
