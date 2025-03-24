using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "Piece", menuName = "Data/Pieces")]
    public class PiecePrefabData : ScriptableObject
    {
        [SerializeField] public PieceType pieceType;
        [SerializeField] public GameObject[] prefab;
        [SerializeField] public Sprite icon;
        [SerializeField] public int cost;


        public GameObject getPrefabBasedOnTile(Piece piece)
        {
            switch (piece.square.type)
            {
                case SquareType.Water:
                    return prefab[0];
                case SquareType.Grass:
                    return prefab[1];
                case SquareType.Sand:
                    return prefab[1];
                default:
                    return prefab[0];
            }
        }
    }
}