using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game_Logic.SubPieces
{
    public class MeleePiece : Piece
    {
        public MeleePiece(Square s, Vector3Int lookDirection, PieceType t, Color team) : base(s, lookDirection, t, team) { }
        public override List<Square> getAvaliableMoves(Square s)
        {
            return getMoveBox(s, 1);

        }
        public override List<Square> getAvaiableAttacks(Square s)
        {
            List<Square> result = new List<Square>();
            for (int x = -2; x <= 2; x++)
            {
                for (int z = -2; z <= 2; z++)
                {
                    if (x == 0 && z == 0) continue;
                    Vector3Int v = new Vector3Int(x, 0, z);
                    Square movesquare = s.getSquareOffset(v);
                    if (movesquare == null) continue;
                    if (!movesquare.hasPiece()) continue;
                    if (movesquare.piece.team == team) continue;
                    result.Add(movesquare);
                }
            }
            result.RemoveAll(item => item == null);
            return result;
        }

        public override void Attack(Piece attk)
        {
            Square moveTo = attk.square;
            base.Attack(attk);
            MoveTo(moveTo);
        }
    }
}