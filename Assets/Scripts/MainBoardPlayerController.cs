using Assets.Scripts.Game_Visuals;
using System.Collections;
using UnityEngine;
using Assets.Scripts.Game_Logic.SubPieces;
using Unity.VisualScripting;
using DG.Tweening;
using Sequence = DG.Tweening.Sequence;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game_Visuals.Visual_Sub_Pieces;
using Assets.Scripts.UI;

namespace Assets.Scripts
{
    public class MainBoardPlayerController : MonoBehaviour
    {
        public MainBoardManager boardManager;
        private GameObject CameraObj;

        public bool isEnabled = true;

        private int state = 0;
        private Square selectedMoveSquare;
        private Piece selectedPiece;
        private GameObject rotationMarker;
        private Square mouseSquare;

        private Plane mousePlane = new Plane(Vector3.up, 0);
        public Vector3 cameraHitPoint = Vector3.zero;

        private Vector3 targetPos = Vector3.zero;
        public float moveSpeed = 5;
        public float lerpSpeed = 5;

        public Color DebugTeamColor;

        [SerializeField] private int armyRange = 25;
        Stack<MoveIntersection> battles = new Stack<MoveIntersection> ();

        List<List<Square>> selectedPaths = new List<List<Square>> ();


        private void Start()
        {
            CameraObj = PrefabManager.instance.Camera;
        }

        public Square getMouseOverSquare(Board b)
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (mousePlane.Raycast(r, out float enter))
            {
                cameraHitPoint = r.GetPoint(enter);
                return b.getSquare(Vector3Int.RoundToInt(cameraHitPoint));
            }
            return null;

        }

        public void resetSelectionState()
        {
            selectedMoveSquare = null;
            selectedPiece = null;
            rotationMarker?.SetActive(false);
            state = 0;
            selectedPaths.Clear();
        }

        private void Update()
        {
            if (!isEnabled) return;

            Vector3 dir = Quaternion.Euler(0, 45, 0) * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetPos += (dir * moveSpeed * Time.deltaTime);
            CameraObj.transform.position = Vector3.Lerp(CameraObj.transform.position, targetPos, Time.deltaTime * lerpSpeed);

            mouseSquare = getMouseOverSquare(boardManager.board);
            if (mouseSquare != null)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    boardManager.Viewer.viewTerritory();
                }

                if (Input.GetKey(KeyCode.LeftShift)) return;

                boardManager.Viewer.clearSelection();

                switch (state)
                {
                    case 0:
                        selectPiece();
                        boardManager.Viewer.setSelectedSquare(mouseSquare, new Color(1, 1, 1, 0.5f));
                        if (mouseSquare.hasPiece())
                        {
                            if (mouseSquare.piece.type == PieceType.Structure)
                            {
                                boardManager.Viewer.viewTerritory(mouseSquare.piece as StructurePiece);
                            }
                        }
                        break;
                    case 1:
                        SelectPiecePath();
                        break;
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    ArmyPiece a1 = (ArmyPiece)boardManager.board.addPiece(mouseSquare.getSquareOffset(new Vector3Int(2, 0, 0)), PieceType.Army, Vector3Int.forward, Color.red);
                    ArmyPiece a2 = (ArmyPiece)boardManager.board.addPiece(mouseSquare.getSquareOffset(new Vector3Int(0, 0, 2)), PieceType.Army, Vector3Int.forward, Color.blue);


                    //for (int i = 0; i < 5; i++)
                    //{
                    //    a1.formation.Add(new ArmyFormation((PieceType)i+1, new Vector3Int(0, 0, i*2), Vector3Int.right));
                    //}

                    //ArmyPiece a2 = (ArmyPiece)boardManager.board.addPiece(mouseSquare.getSquareOffset(new Vector3Int(0, 0, 1)), PieceType.Army, Vector3Int.forward, Color.blue);

                    //for (int i = 0; i < 5; i++)
                    //{
                    //    a2.formation.Add(new ArmyFormation((PieceType)i+1, new Vector3Int(0, 0, i*2), Vector3Int.right));
                    //}
                    a1.formation.Add(new ArmyFormation(PieceType.Sniper, new Vector3Int(0, 0, 1), Vector3Int.right));
                    a2.formation.Add(new ArmyFormation(PieceType.Sniper, new Vector3Int(0, 0, 1), Vector3Int.right));
                    a1.formation.Add(new ArmyFormation(PieceType.Sniper, new Vector3Int(0, 0, 2), Vector3Int.right));
                    a2.formation.Add(new ArmyFormation(PieceType.Sniper, new Vector3Int(0, 0, 2), Vector3Int.right));


                    boardManager.Viewer.UpdatePieceGameobjects();

                    //createSubBoard(mouseSquare);
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    ArmyPiece a1 = (ArmyPiece)boardManager.board.addPiece(mouseSquare, PieceType.Army, Vector3Int.forward, DebugTeamColor);

                    for (int i = 0; i < 5; i++)
                    {
                        a1.formation.Add(new ArmyFormation((PieceType)i + 1, new Vector3Int(0, 0, i * 2), Vector3Int.right));
                    }

                    boardManager.Viewer.UpdatePieceGameobjects();
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    var (orders, intersections) = boardManager.moveAllPieces();

                    foreach (MoveOrder order in orders)
                    {
                        boardManager.board.MovePiece(order.piece, order.path[^1], Vector3.forward);
                        boardManager.Viewer.getVisualPiece(order.piece).PlayPathAnimation(order.path, () => { });
                        
                    }

                    intersections.Reverse();
                    foreach (MoveIntersection item in intersections)
                    {
                        battles.Push(item);
                    }
                    transform.DOMove(transform.position, 2f).OnComplete(() =>
                    {
                        startBattle();
                    });

                }

                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.I))
                {
                    print(mouseSquare);
                }

                boardManager.Viewer.setSelectedSquares(boardManager.Viewintersetions, Color.magenta);
                //boardManager.Viewer.setSelectedSquare(mouseSquare, new Color(1, 1, 1, 0.5f));
            }

        }

        public void startBattle()
        {
            if (battles.Count < 1)
            {
                Debug.Log("updateing territory");
                foreach (Piece p in boardManager.board.pieces)
                {
                    if (p.type != PieceType.Structure) continue;

                    StructurePiece sp = p as StructurePiece;
                    List<Piece> pieces = new List<Piece>();
                    foreach (Square s in sp.territory)
                    {
                        if (!s.hasPiece()) continue;
                        if (s.piece == p) continue;
                        pieces.Add(s.piece);
                    }

                    Color Team = Color.grey;
                    Debug.Log(pieces.Count);
                    foreach (Piece piece in pieces)
                    {
                        if (Team == Color.grey) { Team = piece.team; }

                        if (Team != piece.team) { Team = Color.grey; break; }
                    }
                    if (Team != Color.grey) { sp.team = Team; }
                    boardManager.Viewer.getVisualPiece(p).setVisuals();
                }
                
                return;
            };
            MoveIntersection i = battles.Pop();
            createSubBoard(i);

        }

        public void selectPiece()
        {
            selectedPiece = null;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!mouseSquare.hasPiece()) return;
                if (mouseSquare.hasStructure()) return;
                selectedPiece = mouseSquare.piece;
                MoveOrder existingOrder = boardManager.getPieceMoveOrder(selectedPiece as ArmyPiece);
                if (existingOrder != null)
                {
                    boardManager.moveOrders.Remove(existingOrder);
                    existingOrder.path.Remove(mouseSquare);
                    selectedPaths.Add(new List<Square>() { mouseSquare });
                    selectedPaths.Add(existingOrder.path);
                }
                else
                {
                    selectedPaths.Add(new List<Square>() { mouseSquare });
                }
                state = 1;
            }
        }

        public void SelectPiecePath()
        {
            foreach (MoveOrder i in boardManager.moveOrders)
            {
                if (i.piece.team != selectedPiece.team)
                {
                    boardManager.Viewer.setSelectedSquares(i.path, new Color(1, 0, 0, 0.5f));
                }
                if (i.piece.team == selectedPiece.team && i.piece != selectedPiece)
                {
                    boardManager.Viewer.setSelectedSquares(i.path, new Color(0, 1, 0, 0.5f));
                }
            }

            List<Square> flatPath = selectedPaths.SelectMany(x => x).ToList();

            List<Square> newPath = new List<Square>();
            newPath = boardManager.board.getStraightWithBackup(flatPath[^1], mouseSquare, new SquareType[3] { SquareType.Water, SquareType.Grass, SquareType.Sand });
            if (newPath == null) return;
            
            boardManager.Viewer.setSelectedSquares(flatPath, new Color(1, 0.92f, 0.016f, 0.25f));
            if (flatPath.Count + newPath.Count > armyRange)
            {
                boardManager.Viewer.setSelectedSquares(newPath.GetRange(0, armyRange - flatPath.Count), Color.yellow);
                boardManager.Viewer.setSelectedSquares(newPath.Skip(armyRange - flatPath.Count).ToList(), Color.red);
            }
            else
            {
                boardManager.Viewer.setSelectedSquares(newPath, Color.yellow);


                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (newPath.Count > 0)
                    {
                        selectedPaths.Add(newPath);
                    }
                    else
                    {
                        boardManager.addMoveOrder(selectedPiece as ArmyPiece, mouseSquare, flatPath);
                        state = 0;
                        selectedPiece = null;
                        selectedPaths.Clear();
                        return;
                    }
                }

            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (selectedPaths.Count > 1)
                {
                    selectedPaths.RemoveAt(selectedPaths.Count-1);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                resetSelectionState();
            }
        }

        public void createSubBoard(MoveIntersection intersection)
        {
            Square s = intersection.square;
            ArmyPiece pieceA = intersection.A.piece;
            ArmyPiece pieceB = intersection.B.piece;
            VisualPiece visA = boardManager.Viewer.getVisualPiece(pieceA);
            VisualPiece visB = boardManager.Viewer.getVisualPiece(pieceB);
            isEnabled = false;
            Color og = Camera.main.backgroundColor;
            Sequence seq = DOTween.Sequence();
            targetPos = s.position;
            seq.Append(CameraObj.transform.DOMove(s.position, 1f).SetEase(Ease.InOutSine));
            seq.Append(visA.transform.DOMove(s.position, 1f))
                .Join(visB.transform.DOMove(s.position, 1f))
                .Join(visA.transform.DOScale(Vector3.zero, 0.75f))
                .Join(visB.transform.DOScale(Vector3.zero, 0.75f));
                
            seq.Append(Camera.main.DOOrthoSize(0.1f, 1f).SetEase(Ease.InOutSine));
            seq.OnComplete(() =>
            {
                Camera.main.backgroundColor = MeshGenerator.Instance.materials[((int)s.type) - 1].color;

                GameObject currentSubBoardGobj = Instantiate(PrefabManager.instance.subBoardPrefab);
                currentSubBoardGobj.transform.position = Vector3.zero;
                SubBoardManager subBoardManager = currentSubBoardGobj.GetComponent<SubBoardManager>();
                subBoardManager.Set(boardManager.subBoardSize,
                    boardManager.subBoardSize,
                    pieceA,
                    pieceB,
                    s,
                    closeSubBoard);

                CameraObj.transform.position = new Vector3(boardManager.subBoardSize / 2 - 0.5f, 25, boardManager.subBoardSize / 2 - 0.5f);
                Camera.main.orthographicSize = boardManager.subBoardSize / 2;

                //animation
                Camera.main.DOColor(og, 2f);
                CameraObj.transform.DOMoveY(0, 2f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    UIhud.instance.show(true);

                });
                boardManager.gameObject.SetActive(false);

            });
            seq.Play();
        }

        public void closeSubBoard(SubBoardManager manager)
        {
            GameObject cam = PrefabManager.instance.Camera;
            Sequence seq = DOTween.Sequence();
            Color ogColor = Camera.main.backgroundColor;
            UIhud.instance.show(false);
            seq.Append(Camera.main.DOColor(MeshGenerator.Instance.materials[((int)manager.mainBoardSquare.type) - 1].color, 0.5f).OnComplete(() =>
            {
                Camera.main.backgroundColor = ogColor;
                Camera.main.orthographicSize = 0.1f;
                gameObject.SetActive(true);
                cam.transform.position = manager.mainBoardSquare.position;
                targetPos = cam.transform.position;
                cam.transform.rotation = Quaternion.Euler(0, 0, 0);
                Camera.main.DOOrthoSize(10f, 1f).OnComplete(() =>
                {
                    boardManager.Viewer.getVisualPiece(manager.winner).transform.DOScale(Vector3.one, 1f);
                    boardManager.board.MovePiece(manager.winner, manager.mainBoardSquare, manager.winner.lookDirection);
                    boardManager.board.removePiece(manager.loser);
                    //boardManager.Viewer.getVisualPiece(manager.winner).PlayMoveAnimation(manager.winner.square, manager.mainBoardSquare, manager.winner.lookDirection, () => { });
                    boardManager.Viewer.removePiece(manager.loser);

                    boardManager.board.removePiece(manager.loser);
                    startBattle();
                    if (battles.Count == 0) { isEnabled = true; }
                });
            }));     
        }
    }
}