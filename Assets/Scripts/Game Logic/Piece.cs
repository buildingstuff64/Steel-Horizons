using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Piece
{
    public Square square;
    public PieceType type;
    public Vector3Int lookDirection;
    public Color team;
    public bool isSupported = false;

    public Piece(Square s, Vector3Int lookDirection, PieceType t, Color team)
    {
        this.square = s;
        this.type = t;
        this.team = team;
        this.lookDirection = lookDirection;

    }

    public void remove()
    {
        square.board.pieces.Remove(this);
        square.piece = null;
    }

    protected List<Square> getMoveDirection(Square from, Vector3Int dir, bool ignorePiece)
    {
        List<Square> result = new List<Square>();
        int i = 1;
        while (true)
        {
            Square newsquare = from.getSquareOffset(i * dir);
            i++;
            if (newsquare == null) break;
            if (newsquare.isBlocked || newsquare.hasPiece())
            {
                if (!ignorePiece) { break; }
                else { continue; }
            }
            result.Add(newsquare);
        }
        return result;
    }

    public List<Square> getAttackDirection(Square from, Vector3Int dir, bool piercing)
    {
        List<Square> result = new List<Square>();
        int i = 1;
        while (true)
        {
            Square newsquare = from.getSquareOffset(i * dir);
            i++;
            if (newsquare == null) break;
            if (newsquare.isBlocked)
            {
                if (!piercing) { break; }
                else continue;
            }
            if (newsquare.hasPiece())
            {
                if (newsquare.piece.team != team)
                {
                    result.Add(newsquare);

                    if (!piercing) break;
                    else continue;
                }
                break;
            }
        }
        return result;
    }

    protected List<Square> getMoveBox(Square from, int i)
    {
        List<Square> result = new List<Square>();
        for (int x = -i; x <= i; x++)
        {
            for (int z = -i; z <= i; z++)
            {
                //if (x == 0 && z == 0) continue;
                Vector3Int v = new Vector3Int(x, 0, z);
                Square movesquare = from.getSquareOffset(v);
                if (movesquare == null || movesquare.isBlocked) continue;
                if (movesquare.hasPiece() && movesquare != square) continue;
                result.Add(movesquare);
            }
        }
        result.RemoveAll(item => item == null);
        return result;
    }

    virtual public List<Square> getAvaliableMoves(Square s) { return new List<Square>(); }
    virtual public List<Square> getAvaiableAttacks(Square s) { return new List<Square>(); }

    virtual public object getData() { return null; }

    virtual public void MoveTo(Square s)
    {
        s.piece = this;
        square.piece = null;
        square = s;
    }

    virtual public void Rotate(Vector3Int r)
    {
        lookDirection = r;
    }

    virtual public void Attack(Piece attk)
    {
        attk.remove();
    }

    virtual public void onStartTurn() { }
 
}

public enum PieceType { None, Mech, Sniper, Shield, Teleporter, Support, Battleship, Destroyer, ShieldShip, Submarine, Carrier, Army }
