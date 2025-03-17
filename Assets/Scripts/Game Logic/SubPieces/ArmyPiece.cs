using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game_Logic.SubPieces
{
    public class ArmyPiece : Piece
    {
        public List<ArmyFormation> formation = new List<ArmyFormation>();

        public ArmyPiece(Square s, Vector3Int lookDirection, PieceType t, Color team) : base(s, lookDirection, t, team) { }

        public override object getData()
        {
            return this;
        }

    }

    public class ArmyFormation
    {
        public PieceType piece;
        public Vector3Int position;
        public Vector3Int lookDirection;

        public ArmyFormation(PieceType piece, Vector3Int position, Vector3Int lookDirection)
        {
            this.piece = piece;
            this.position = position;
            this.lookDirection = lookDirection;
        }
    }
}