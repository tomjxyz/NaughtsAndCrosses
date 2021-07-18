using Godot;

// This class makes all the decisions for the CPU player
class Drone {

    public GridBtn findMove(GridBtn[,] buttons, bool crossTurn) {
        foreach (GridBtn b in buttons) {
            if (b.Empty)
                return b;
        }
        return null;
    }

}