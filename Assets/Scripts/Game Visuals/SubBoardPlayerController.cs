using Assets.Scripts.UI;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Game_Visuals
{
    public class SubBoardPlayerController : MonoBehaviour
    {
        public SubBoardManager boardManager;

        public bool isAnimation = false;

        private int state = 0;
        private Square selectedMoveSquare;
        private Piece selectedPiece;
        private GameObject rotationMarker;
        private Square mouseSquare;

        private Plane mousePlane = new Plane(Vector3.up, 0);
        public Vector3 cameraHitPoint = Vector3.zero;
        public Action<SubBoardManager> onWin;
        public bool gameEnded = false;

        public List<Square> debugSquares = new List<Square>();

        private void Awake()
        {
            rotationMarker = Instantiate(PrefabManager.instance.rotationMarker);
            rotationMarker.SetActive(false);
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
            UIhud.instance.changeTeamColor(boardManager.battle.getCurrentTeam());
            boardManager.battle.startTurn();
        }

        private void Update()
        {
            mouseSquare = getMouseOverSquare(boardManager.board);
            if (mouseSquare != null && !isAnimation)
            {
                if (gameEnded) 
                {
                    isAnimation = true;
                    closeSubBoard();
                }

                boardManager.Viewer.clearSelection();

                if (Input.GetKeyDown(KeyCode.End))
                {
                    gameEnded = true;
                }

                if (Input.GetKeyDown(KeyCode.Home))
                {
                   print(boardManager.board.ToString());
                }

                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (Input.GetKeyDown(KeyCode.F1))
                    {
                        debugSquares.AddRange(mouseSquare.getNeighbours());
                    }

                    if (Input.GetKeyDown(KeyCode.F2))
                    {
                        foreach (Square s in boardManager.board.squares)
                        {
                            if (!s.hasPiece()) continue;
                            if (s.piece.isSupported) { debugSquares.Add(s); }
                        }
                    }
                }
                else
                {
                    debugSquares.Clear();
                }

                switch (state)
                {
                    case 0:
                        selectPiece();
                        break;
                    case 1:
                        selectMoveSquare();
                        break;
                    case 2:
                        selectRotation();
                        break;
                    default:
                        print("oh shit");
                        break;
                }


                boardManager.Viewer.setSelectedSquares(debugSquares, Color.magenta);
                boardManager.Viewer.setSelectedSquare(mouseSquare, new Color(1, 1, 1, 0.5f));

            }

        }

        private void selectPiece()
        {
            selectedPiece = null;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                print("test");
                if (!mouseSquare.hasPiece()) return;
                if (mouseSquare.piece.team != boardManager.battle.getCurrentTeam()) return;
                selectedPiece = mouseSquare.piece;
                state = 1;
            }
        }

        private void selectMoveSquare()
        {
            boardManager.Viewer.setSelectedSquares(selectedPiece.getAvaliableMoves(selectedPiece.square), boardManager.Viewer.MoveColor);
            boardManager.Viewer.setSelectedSquares(selectedPiece.getAvaiableAttacks(selectedPiece.square), boardManager.Viewer.AttackColor);

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                //move square
                if (selectedPiece.getAvaliableMoves(selectedPiece.square).Contains(mouseSquare))
                {
                    selectedMoveSquare = mouseSquare;
                    state = 2;
                }

                //attack square
                if (selectedPiece.getAvaiableAttacks(selectedPiece.square).Contains(mouseSquare))
                {
                    isAnimation = true;
                    Piece p = mouseSquare.piece;
                    boardManager.battle.makeAttack(selectedPiece, p);
                    boardManager.Viewer.PlayAttackPieceAnimation(selectedPiece, p, () =>
                    {
                        isAnimation = false;
                        boardManager.Viewer.PlayStartTurnAnimations();
                    });

                    boardManager.battle.startTurn();
                    boardManager.Viewer.clearSelection();
                    resetSelectionState();
                    boardManager.Viewer.UpdatePieceGameobjects();
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                state = 0;
                selectedMoveSquare = null;
            }
        }

        private void selectRotation()
        {
            rotationMarker.SetActive(true);
            rotationMarker.transform.position = selectedMoveSquare.position;

            boardManager.Viewer.setSelectedSquares(selectedPiece.getAvaliableMoves(selectedPiece.square), boardManager.Viewer.MoveColor);
            boardManager.Viewer.setSelectedSquares(selectedPiece.getAvaiableAttacks(selectedPiece.square), boardManager.Viewer.AttackColor);

            Vector3 dir = cameraHitPoint - selectedMoveSquare.position;
            if (dir != Vector3.zero)
            {
                float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

                float snap = Mathf.Round(angle / 90) * 90;

                rotationMarker.transform.rotation = Quaternion.Euler(0, snap, 0);

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    isAnimation = true;
                    boardManager.battle.makeMove(selectedPiece, selectedMoveSquare, rotationMarker.transform.rotation.eulerAngles);
                    boardManager.Viewer.PlayMovePieceAnimation(selectedPiece, selectedMoveSquare, rotationMarker.transform.rotation.eulerAngles, () =>
                    {
                        isAnimation = false;
                        boardManager.Viewer.PlayStartTurnAnimations();
                    });

                    boardManager.battle.startTurn();
                    boardManager.Viewer.clearSelection();
                    resetSelectionState();
                    boardManager.Viewer.UpdatePieceGameobjects();
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                state = 1;
                rotationMarker.SetActive(false);
            }
        }

        public void closeSubBoard()
        {
            //change
            Sequence seq = DOTween.Sequence();
            seq.Append(PrefabManager.instance.Camera.transform.DOMoveY(25, 1f).SetEase(Ease.InOutSine)).OnComplete(() =>
            {
                onWin.Invoke(boardManager);
                Destroy(gameObject);
            });
            seq.Play();
            
        }

    }
}