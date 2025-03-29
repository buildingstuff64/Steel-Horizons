using Assets.Scripts.Game_Logic.SubPieces;
using Assets.Scripts.Game_Visuals.Visual_Sub_Pieces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game_Visuals
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class BoardViewer : MonoBehaviour
    {

        public BoardManager boardManager;
        private Mesh mesh;

        private GameObject selectGrid;
        Texture2D selectionTexture;
        Material material;

        public Color MoveColor;
        public Color AttackColor;
        public Color SpecialColor;

        public Dictionary<Piece, VisualPiece> pieceObjects = new Dictionary<Piece, VisualPiece>();

        private void Awake()
        {
            GetComponent<MeshFilter>().mesh = mesh;
        }

        public void Set(BoardManager b)
        {
            boardManager = b;
        }

        public VisualPiece getVisualPiece(Piece piece)
        {
            return pieceObjects[piece];
        }

        public void UpdatePieceGameobjects()
        {
            foreach (Piece p in boardManager.board.pieces)
            {
                if (pieceObjects.ContainsKey(p)) continue;
                GameObject g = Instantiate(PrefabManager.instance.getPiecePrefab(p), this.transform);
                Type T = VisualPiece.getVisualPieceType(p.type, p.square);
                g.AddComponent(T);
                VisualPiece visP = g.GetComponent<VisualPiece>();
                visP.piece = p;
                visP.data = p.getData();
                visP.viewer = this;
                visP.transform.position = p.square.position;
                visP.transform.rotation = Quaternion.LookRotation(p.lookDirection, Vector3.up);
                visP.setVisuals();
                pieceObjects.Add(p, visP);
            }
        }

        public void removePiece(Piece p)
        {
            VisualPiece visp = pieceObjects[p];
            Destroy(visp.gameObject);
            pieceObjects.Remove(p);
        }

        public void PlayAttackPieceAnimation(Piece from, Piece to, Action onComplete)
        {
            pieceObjects[from].PlayAttackAnimation(pieceObjects[from], pieceObjects[to], onComplete);
        }

        public void PlayMovePieceAnimation(Piece p, Square to, Vector3 roation, Action onComplete)
        {
            pieceObjects[p].PlayMoveAnimation(p.square, to, roation, onComplete);
        }

        public void PlayStartTurnAnimations()
        {
            foreach (VisualPiece vis in pieceObjects.Values)
            {
                vis.PlayTurnStartAnimation(() => { });
            }
        }

        public void UpdateView(bool x)
        {
            CreateMesh();
            if (x) { createSelectionGrid(); }
            UpdatePieceGameobjects();
        }

        public void CreateMesh()
        {
            GetComponent<MeshFilter>().mesh = MeshGenerator.Instance.getSubBoardMesh(boardManager.board);
            GetComponent<MeshRenderer>().materials = MeshGenerator.Instance.materials.ToArray();
        }

        public void createSelectionGrid()
        {
            selectGrid = Instantiate(PrefabManager.instance.gridViewPrefab, this.transform);
            selectGrid.transform.position = new Vector3((boardManager.board.xsize / 2) - 0.5f, 0.001f, (boardManager.board.zsize / 2) - 0.5f);
            selectGrid.transform.localScale = new Vector3(boardManager.board.xsize, boardManager.board.zsize, 1);
            selectionTexture = new Texture2D(boardManager.board.xsize, boardManager.board.zsize);
            selectionTexture.filterMode = FilterMode.Point;
            for (int x = 0; x < boardManager.board.xsize; x++)
            {
                for (int z = 0; z < boardManager.board.zsize; z++)
                {
                    selectionTexture.SetPixel(x, z, new Color(0, 0, 0, 0));
                }
            }
            selectionTexture.Apply();
            material = selectGrid.GetComponent<MeshRenderer>().material;
            material.SetVector("_GridSize", new Vector2(boardManager.board.xsize, boardManager.board.zsize));
            material.SetTexture("_SelectionTexture", selectionTexture);
            selectGrid.GetComponent<MeshRenderer>().material = material;
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

        public void viewTerritory()
        {
            foreach (Piece piece in boardManager.board.pieces)
            {
                if (piece.type == PieceType.Structure)
                {
                    StructurePiece sp = piece as StructurePiece;
                    setSelectedSquares(sp.territory, sp.team * new Color(1, 1, 1, 0.25f));
                }
            }
        }

        public void viewTerritory(StructurePiece sp)
        {
            setSelectedSquares(sp.territory, new Color(1,1,1,0.25f));
        }
    }
}