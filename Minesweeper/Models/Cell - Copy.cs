namespace Minesweeper.Models;

public class Cell
{
    public bool IsMine { get; set; }
    public int AdjacentMines { get; set; }
    public CellState State { get; set; }
}
