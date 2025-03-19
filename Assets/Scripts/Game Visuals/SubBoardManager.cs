using Assets.Scripts;
using Assets.Scripts.Game_Logic.SubPieces;
using Assets.Scripts.Game_Visuals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubBoardManager : BoardManager
{
    public Battle battle;
    public Square mainBoardSquare;
    public ArmyPiece TeamA, TeamB;
    public ArmyPiece winner;
    public ArmyPiece loser;
    public Action<SubBoardManager> onWin;

    private void Awake()
    {
        Viewer = GetComponent<BoardViewer>();
    }

    public void Set(int xsize, int zsize, ArmyPiece teamA, ArmyPiece teamB, Square mainboardsquare
        , Action<SubBoardManager> onWin)
    {
        print(teamA.formation.Count);
        print(teamB.formation.Count);

        board = new Board(xsize, zsize, mainboardsquare.type);
        battle = new Battle(board, teamA.formation, teamB.formation, teamA.team, teamB.team, () =>
        {
            if (battle.winner == teamA.team){ winner = teamA; loser = teamB; }
            if (battle.winner == teamB.team) { winner = teamB; loser = teamA; }

            GetComponent<SubBoardPlayerController>().gameEnded = true;
        });

        TeamA = teamA;
        TeamB = teamB;

        this.mainBoardSquare = mainboardsquare;
        GetComponent<SubBoardPlayerController>().onWin = onWin;
        GetComponent<SubBoardPlayerController>().resetSelectionState();
        Viewer.Set(this);
        Viewer.UpdateView(true);
    }

    public ArmyPiece getWinner()
    {
        return winner;
    }




}
