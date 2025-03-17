using Assets.Scripts.Game_Logic.SubPieces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle
{
    public Board board;
    public bool currentPlayer = true;
    public Color[] teams = new Color[2];

    List<ArmyFormation> teamA, teamB;

    public Battle(Board b, List<ArmyFormation> teamA, List<ArmyFormation> teamB, Color teamAColor, Color teamBColor)
    {
        board = b;

        foreach (ArmyFormation f in teamA)
        { 
            board.addPiece(board.getSquare(f.position), f.piece, f.lookDirection, teamAColor);
        }

        foreach (ArmyFormation f in teamB)
        {
            board.addPiece(board.getSquare(board.xsize-1 - f.position.x, f.position.z), f.piece, -f.lookDirection, teamBColor);
        }

        teams[0] = teamAColor;
        teams[1] = teamBColor;

        this.teamA = teamA;
        this.teamB = teamB;

        startBattle();
        startTurn();

    }

    public void startBattle()
    {
        
    }

    public void stopBattle()
    {

    }

    public void startTurn()
    {
        foreach (Square square in board.squares) square.isBlocked = false;

        for (int i = 0; i < board.pieces.Count; i++)
        {
            board.pieces[i].isSupported = false;
            if (board.pieces[i].type == PieceType.Support)
            {
                board.pieces[i].onStartTurn();
            }
        }

        for (int i = 0; i < board.pieces.Count; i++)
        {
            if (board.pieces[i].type == PieceType.Shield)
            {
                board.pieces[i].onStartTurn();
            }
        }

    }

    public void makeMove(Piece p, Square s, Vector3 rotation)
    {
        board.MovePiece(p, s, Vector3Int.RoundToInt(rotation));
        nextTurn();
    }

    public void makeAttack(Piece attacker, Piece defender)
    {
        board.AttackPiece(attacker, defender);
        nextTurn();
    }

    public void nextTurn()
    {
        Debug.Log("next turn");
        currentPlayer = !currentPlayer;   
    }

    public Color getCurrentTeam()
    {
        return currentPlayer ? teams[0] : teams[1];
    }
}
