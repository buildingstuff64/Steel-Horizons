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
                boardManager.Viewer.clearSelection();

                switch (state)
                {
                    case 0:
                        selectPiece();
                        break;
                    case 1:
                        SelectMove();
                        break;
                }

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    ArmyPiece a1 = (ArmyPiece)boardManager.board.addPiece(mouseSquare.getSquareOffset(new Vector3Int(1, 0, 0)), PieceType.Army, Vector3Int.forward, Color.red);

                    for (int i = 0; i < 5; i++)
                    {
                        a1.formation.Add(new ArmyFormation((PieceType)i+1, new Vector3Int(0, 0, i*2), Vector3Int.right));
                    }

                    ArmyPiece a2 = (ArmyPiece)boardManager.board.addPiece(mouseSquare.getSquareOffset(new Vector3Int(0, 0, 1)), PieceType.Army, Vector3Int.forward, Color.blue);

                    for (int i = 0; i < 5; i++)
                    {
                        a2.formation.Add(new ArmyFormation((PieceType)i+1, new Vector3Int(0, 0, i*2), Vector3Int.right));
                    }

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
                        boardManager.board.MovePiece(order.piece, order.targetPos, Vector3.forward);
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


                boardManager.Viewer.setSelectedSquares(boardManager.Viewintersetions, Color.magenta);
                boardManager.Viewer.setSelectedSquare(mouseSquare, new Color(1, 1, 1, 0.5f));
            }

        }

        public void startBattle()
        {

            MoveIntersection i = battles.Pop();
            createSubBoard(i);

        }

        public void closeSubBoard(SubBoardManager manager)
        {
            GameObject cam = PrefabManager.instance.Camera;
            Sequence seq = DOTween.Sequence();
            Color ogColor = Camera.main.backgroundColor;
            seq.Append(Camera.main.DOColor(MeshGenerator.Instance.materials[((int)manager.mainBoardSquare.type) - 1].color, 0.5f).OnComplete(() =>
            {
                Camera.main.backgroundColor = ogColor;
                Camera.main.orthographicSize = 0.1f;
                gameObject.SetActive(true);
                cam.transform.position = manager.mainBoardSquare.position;
                cam.transform.rotation = Quaternion.Euler(0, 0, 0);
                Camera.main.DOOrthoSize(10f, 1f).OnComplete(() =>
                {
                    if (battles.Count > 0)
                    {
                        startBattle();
                    }
                    else
                    {
                        isEnabled = true;
                    }
                });
            }));

        }

        public void selectPiece()
        {
            selectedPiece = null;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (!mouseSquare.hasPiece()) return;
                selectedPiece = mouseSquare.piece;
                state = 1;
            }
        }

        public void SelectMove()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                state = 0;
                selectedPiece = null;
                return;
            }

            List<Square> path = boardManager.board.getStraightWithBackup(selectedPiece.square, mouseSquare, new SquareType[] {SquareType.Water});
            if (path == null) return;
            if (path.Count > armyRange)
            {
                boardManager.Viewer.setSelectedSquares(path.GetRange(0, armyRange), Color.yellow);
                boardManager.Viewer.setSelectedSquares(path.Skip(armyRange).ToList(), Color.red);
            }
            else
            {
                boardManager.Viewer.setSelectedSquares(path, Color.yellow);
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    boardManager.addMoveOrder(selectedPiece as ArmyPiece, mouseSquare, path);
                    state = 0;
                    selectedPiece = null;
                    return;
                }
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


                });
                boardManager.gameObject.SetActive(false);

            });
            seq.Play();
        }
    }
}