using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game_Logic.SubPieces
{
    public class SniperPiece : Piece
    {
        public SniperPiece(Square s, Vector3Int lookDirection, PieceType t, Color team) : base(s, lookDirection, t, team) { }
        public override List<Square> getAvaliableMoves(Square s)
        {
            List<Square> result = new List<Square>() { s };

            result.AddRange(getMoveDirection(square, new Vector3Int(0, 0, 1), false));
            result.AddRange(getMoveDirection(square, new Vector3Int(0, 0, -1), false));
            result.AddRange(getMoveDirection(square, new Vector3Int(1, 0, 0), false));
            result.AddRange(getMoveDirection(square, new Vector3Int(-1, 0, 0), false));

            result.RemoveAll(item => item == null);
            return result;
        }

        public override List<Square> getAvaiableAttacks(Square s)
        {
            List<Square> result = new List<Square>();

            result.AddRange(getAttackDirection(square, lookDirection, isSupported));

            return result;
        }

        public override void Attack(Piece attk)
        {
            if (isSupported)
            {
               foreach (Square s in getAttackDirection(square, lookDirection, true))
                {
                    if (!s.hasPiece()) return;
                    s.piece.remove();
                }
            }

            else
            {
                attk.remove();
            }
        }
    }
}