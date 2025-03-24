using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Assets.Scripts.Game_Logic.SubPieces
{
    public class StructurePiece : Piece
    {
        public List<Square> territory = new List<Square>();

        public StructurePiece(Square s, Vector3Int lookDirection, PieceType t, Color team) : base(s, lookDirection, t, team)
        {
        }


    }
}