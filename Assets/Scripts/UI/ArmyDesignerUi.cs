using Assets.Scripts.Game_Visuals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ArmyDesignerUi : MonoBehaviour
    {
        [SerializeField] private GameObject InfoPanelPrefab;
        [SerializeField] private GameObject InfoContent;

        [SerializeField] private RawImage avaiableMoveTexture;
        [SerializeField] private Color backgroundColor;
        public Texture2D selectionTexture;
        public PieceType currentPieceSelected = PieceType.Mech;

        private void Start()
        {
            createPieceInfoPiece();
        }

        void createPieceInfoPiece()
        {
            foreach (PiecePrefabData data in PrefabManager.instance.pieces)
            {
                GameObject g = Instantiate(InfoPanelPrefab, InfoContent.transform);
                Piece_Picker p = g.GetComponent<Piece_Picker>();
                p.Set(data);
            }

            setPieceInfo();
        }

        void setPieceInfo()
        {
            selectionTexture = new Texture2D(7, 7);
            selectionTexture.filterMode = FilterMode.Point;

            Board b = new Board(7, 7, SquareType.None);
            b.addPiece(b.getSquare(3, 3), currentPieceSelected, Vector3Int.forward, Color.green);
            Piece p = b.getSquare(3, 3).piece;
            for (int x = 0; x < b.xsize; x++)
            {
                for (int z = 0; z < b.zsize; z++)
                {
                    selectionTexture.SetPixel(x, z, backgroundColor);
                }
            }

            setSelectedSquares(p.getAvaliableMoves(p.square), Color.yellow);
            //avaiableMoveTexture.texture = selectionTexture;
            avaiableMoveTexture.material.SetVector("_GridSize", new Vector2(b.xsize, b.zsize));
            avaiableMoveTexture.material.SetTexture("_SelectionTexture", selectionTexture);
            selectionTexture.Apply();


        }

        public void setSelectedSquares(List<Square> squares, Color c)
        {
            foreach (Square square in squares)
            {
                setSelectedSquare(square, c);
            }
            selectionTexture.Apply();
        }

        public void setSelectedSquare(Square square, Color c)
        {
            selectionTexture.SetPixel(square.x, square.z, c);
            selectionTexture.Apply();
        }

        public void clearSelection()
        {
            for (int x = 0; x < selectionTexture.width; x++)
            {
                for (int z = 0; z < selectionTexture.height; z++)
                {
                    selectionTexture.SetPixel(x, z, new Color(0, 0, 0, 0));
                }
            }
            selectionTexture.Apply();
        }
    }
}