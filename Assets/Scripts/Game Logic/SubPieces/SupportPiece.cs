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
            base.onStartTurn();
            foreach (Square s in square.getNeighbours())
            {
                if (s.hasPiece())
                {
                    s.piece.isSupported = true;
                }
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