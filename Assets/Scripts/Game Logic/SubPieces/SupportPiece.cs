using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game_Logic.SubPieces
{
    public class SupportPiece : Piece
    {

        public SupportPiece(Square s, Vector3Int lookDirection, PieceType t, Color team) : base(s, lookDirection, t, team) { }
        public override List<Square> getAvaliableMoves(Square s)
        {
            List<Square> result = new List<Square>();
            result.AddRange(getMoveBox(s, 1));

            result.Add(getSquareWithOffsetChecks(new Vector3Int(0, 0, 2)));
            result.Add(getSquareWithOffsetChecks(new Vector3Int(0, 0, -2)));
            result.Add(getSquareWithOffsetChecks(new Vector3Int(2, 0, 0)));
            result.Add(getSquareWithOffsetChecks(new Vector3Int(-2, 0, 0)));

            result.RemoveAll(item => item == null);
            return result;
        }

        public override List<Square> getAvaiableAttacks(Square s)
        {
            return base.getAvaiableAttacks(s);
        }

        public override void onStartTurn()
        {
            List<Square> squares = new List<Square>
            {
                square.getSquareOffset(Vector3Int.forward),
                square.getSquareOffset(Vector3Int.left),
                square.getSquareOffset(Vector3Int.right),
                square.getSquareOffset(Vector3Int.back)
            };
            foreach (Square square in squares)
            {
                if (square == null) continue;
                if (square.hasPiece()) square.piece.isSupported = true;
            }
        }

        private Square getSquareWithOffsetChecks(Vector3Int v)
        {
            Square s = this.square.getSquareOffset(v);
            if (s == null) return null;
            if (s.hasPiece()) return null;
            if (s.isBlocked) return null;
            return s;
        }
    }
}