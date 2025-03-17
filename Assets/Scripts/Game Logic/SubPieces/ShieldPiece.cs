using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Game_Logic.SubPieces
{
    public class ShieldPiece : Piece
    {
        public ShieldPiece(Square s, Vector3Int lookDirection, PieceType t, Color team) : base(s, lookDirection, t, team) { }

        List<Square> blockedSpaces = new List<Square>();
        public int turnsSinceMoved = 1;
        public override List<Square> getAvaliableMoves(Square s)
        {
            List<Square> result = new List<Square>();
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    //if (x == 0 && z == 0) continue;
                    Vector3Int v = new Vector3Int(x, 0, z);
                    Square movesquare = s.getSquareOffset(v);
                    if (movesquare == null) continue;
                    if (movesquare.hasPiece() && movesquare != square) continue;
                    result.Add(movesquare);
                }
            }
            result.RemoveAll(item => item == null);
            return result;
        }

        public override void MoveTo(Square s)
        {
            turnsSinceMoved = 0;
            base.MoveTo(s);
        }

        public override void onStartTurn()
        {
            base.onStartTurn();

            turnsSinceMoved++;
            blockedSpaces.Clear();

            Vector3Int right = Vector3Int.RoundToInt(Vector3.Cross(Vector3.up, lookDirection).normalized);

            blockedSpaces.AddRange(new List<Square>()
            {
                square.getSquareOffset(lookDirection - right),
                square.getSquareOffset(lookDirection),
                square.getSquareOffset(lookDirection + right)

            });
            if (isSupported)
            {
                blockedSpaces.AddRange(new List<Square>()
                {
                    square.getSquareOffset(lookDirection - right - right),
                    square.getSquareOffset(lookDirection + right + right)

                });      
            } 

            blockedSpaces.RemoveAll(item => item == null);

            if (turnsSinceMoved > 1)
            {
                foreach (Square square in blockedSpaces)
                {
                    if (square.hasPiece())
                    {
                        square.piece.remove();
                        square.piece = null;
                    }
                    square.isBlocked = true;
                }
            }
            
        }
    }
}