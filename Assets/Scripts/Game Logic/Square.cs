using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square
{
    public int x, z;
    public Vector3Int position;
    public SquareType type;
    public Piece piece;
    public bool isBlocked = false;
    public Board board;

    public float gCost, fCost, hCost;
    public Square prev;
    public object data;

    public Square(int x, int z, SquareType type, Board board)
    {
        this.x = x;
        this.z = z;
        this.type = type;
        position = new Vector3Int(x, 0, z);
        this.board = board;
    }

    public List<Square> getNeighbours()
    {
        return board.getNeighbours(this);
    }

    public Square getNeighbour(Vector3 dir)
    {
        return board.getNeighbour(this, dir);
    }

    public Square getSquareOffset(Vector3Int offset)
    {
        return board.getSquare(x + (int)offset.x, z + (int)offset.z);
    }

    public bool hasPiece()
    {
        return piece != null;
    }

    public override string ToString()
    {
        return string.Format("{0} - ({1},{2})", type, x, z);
    }

    public void resetPathfinding()
    {
        hCost = 0;
        fCost = 0;
        gCost = 0;
        prev = null;
    }
}
public enum SquareType { None, Water, Grass, Sand }
