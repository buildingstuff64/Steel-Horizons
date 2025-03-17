using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game_Logic.SubPieces
{
    public class TeleporterPiece : Piece
    {
        public TeleporterPiece(Square s, Vector3Int lookDirection, PieceType t, Color team) : base(s, lookDirection, t, team) { }

        public override List<Square> getAvaliableMoves(Square s)
        {
            List<Square> result = new List<Square>();
            result.AddRange(getMoveBox(s, 1));

            result.AddRange(getMoveDirection(square, new Vector3Int(1, 0, 1), true));
            result.AddRange(getMoveDirection(square, new Vector3Int(-1, 0, 1), true));
            result.AddRange(getMoveDirection(square, new Vector3Int(1, 0, -1), true));
            result.AddRange(getMoveDirection(square, new Vector3Int(-1, 0, -1), true));

            if (isSupported)
            {
                result.AddRange(getMoveDirection(square, new Vector3Int(0, 0, 1), true));
                result.AddRange(getMoveDirection(square, new Vector3Int(0, 0, -1), true));
                result.AddRange(getMoveDirection(square, new Vector3Int(1, 0, 0), true));
                result.AddRange(getMoveDirection(square, new Vector3Int(-1, 0, 0), true));
            }
            result.RemoveAll(item => item == null);

            return result;
        }

        public override List<Square> getAvaiableAttacks(Square s)
        {
            List<Square> result = new List<Square>();

            for (int i = 1; i <= 3; i++)
            {
                Square newsquare = square.getSquareOffset(lookDirection * i);
                if (newsquare == null || newsquare.isBlocked) break;
                if (newsquare.hasPiece())
                {
                    if (newsquare.piece.team != team)
                    {
                        result.Add(newsquare);
                    }
                    else break;
                }
            }

            return result;
        }
    }
}