using Assets.Scripts.Game_Logic.SubPieces;
using Assets.Scripts.Game_Visuals;
using Assets.Scripts.Game_Visuals.Visual_Sub_Pieces;
using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class MainBoardManager : BoardManager
    {
        [SerializeField] int xsize, zsize;

        [SerializeField] private List<float> noiseScales = new List<float>();
        [SerializeField] private List<float> Amplitides = new List<float>();

        [SerializeField] private float waterThreshold, grassThreshold, sandThreshold;

        private float randomOffset;

        [Header("Sub Board Parameters")]
        public int subBoardSize = 10;

        public List<MoveOrder> moveOrders = new List<MoveOrder>();
        public List<Square> Viewintersetions = new List<Square>();

        private void Awake()
        {
            Viewer = GetComponent<BoardViewer>();
        }

        public void addMoveOrder(ArmyPiece p, Square s, List<Square> path)
        {
            MoveOrder moveOrder = new MoveOrder(p, s, path);
            moveOrders.Add(moveOrder);
        }


        public List<MoveIntersection> finalIntersection = new List<MoveIntersection>();
        List<Piece> found = new List<Piece>();
        List<MoveOrder> finalOrders = new List<MoveOrder>();
        public (List<MoveOrder>, List<MoveIntersection>) moveAllPieces()
        {
            Viewintersetions.Clear();

            //foreach (Square s in board.squares) s.data = null;

            //int maxCount = 0;
            //foreach (MoveOrder order in moveOrders)
            //{
            //    if (order.path.Count > maxCount) maxCount = order.path.Count;
            //}

            //List<MoveOrder> orders = moveOrders;
            //print(orders.Count);

            //for (int i = 0; i < maxCount; i++)
            //{
            //    for (int j = 0; j < orders.Count; j++)
            //    {
            //        if (i >= orders[j].path.Count) continue;

            //        Square newSquare = orders[j].path[i];
            //        if (newSquare.data as Piece == null)
            //        {
            //            newSquare.data = orders[j].piece;
            //        }

            //    }
            //}

            //foreach (Square s in board.squares)
            //{
            //    Piece p = s.data as Piece;
            //    if (p == null) continue;
            //    print(p.team);
            //    Color c = p.team;
            //    c.a = 1;
            //    Debug.DrawLine(s.position, s.position + Vector3.up * 5f, c, 15f);     
            //}

            //List<Piece> found = new List<Piece>();
            //List<MoveOrder> newOrders = new List<MoveOrder>();

            //foreach (MoveOrder order in moveOrders)
            //{
            //    if (found.Contains(order.piece)) continue;
            //    int c = 0;
            //    foreach (Square s in order.path)
            //    {
            //        if (s.data != null)
            //        {
            //            if (s.data == order.piece || found.Contains(s.data)) continue;

            //            //Debug.DrawLine(s.position, s.position + Vector3.up * 10f, Color.magenta, 25f);
            //            intersetions.Add(s);
            //            found.Add(order.piece);
            //            found.Add((Piece)s.data);
            //            newOrders.Add(new MoveOrder(order.piece, s, order.path.GetRange(0, c)));
            //            break;

            //        }
            //        c++;
            //    }
            //}
            //print(intersetions.Count);
            found.Clear();
            finalOrders.Clear();
            finalIntersection.Clear();

            while(moveOrders.Count > 0)
            {
                runIntersectionfinded();
            }

            return (finalOrders, finalIntersection);

            //board.MovePiece(order.piece, order.targetPos, Vector3.forward);
            //Viewer.getVisualPiece(order.piece).PlayPathAnimation(order.path);
        }

        private void runIntersectionfinded()
        {
            List<MoveIntersection> intersections = new List<MoveIntersection>();
            foreach (MoveOrder order in moveOrders)
            {
                if (found.Contains(order.piece)) continue;
                foreach (MoveOrder rest in moveOrders)
                {
                    if (rest == order) continue;
                    for (int i = 0; i < rest.path.Count - 1; i++)
                    {
                        if (order.path.Contains(rest.path[i]) && order.piece.team != rest.piece.team)
                        {
                            int index = order.path.IndexOf(rest.path[i]);
                            print(string.Format("{0} & {1} > {2}", order.piece.team, rest.piece.team, index + i));
                            //Viewintersetions.Add(rest.path[i]);
                            Viewintersetions.Add(rest.path[i]);
                            intersections.Add(new MoveIntersection(order, rest, rest.path[i], index, i));
                            found.Add(rest.piece);
                            break;
                        }
                    }
                }
            }
            foreach (MoveOrder order in moveOrders)
            {
                bool t = true;
                foreach (MoveIntersection i in intersections)
                {
                    if (order == i.A || order == i.B)
                    {
                        t = false;
                    }
                }
                if (t) { finalOrders.Add(order); }
            }


            moveOrders.RemoveAll(x => finalOrders.Contains(x));
            if (intersections.Count > 0)
            {
                MoveIntersection minI = intersections[0];
                foreach (MoveIntersection i in intersections)
                {
                    if (i.Tdistance < minI.Tdistance) minI = i;
                }
                finalOrders.Add(new MoveOrder(minI.A.piece, minI.square, minI.A.path.GetRange(0, minI.Adistance)));
                finalOrders.Add(new MoveOrder(minI.B.piece, minI.square, minI.B.path.GetRange(0, minI.Bdistance)));
                finalIntersection.Add(minI);

                moveOrders.RemoveAll(x => x == minI.A || x == minI.B);
            }
        }


        private void Start()
        {
            randomOffset = Random.Range(0, 100000f);

            createMainBoard();
        }

        public void createMainBoard()
        {
            board = new Board(xsize, zsize, getSquareType);

            Viewer.Set(this);
            Viewer.UpdateView();
        }

        private SquareType getSquareType(int x, int z)
        {
            float noise = 0;
            int i = 0;
            foreach (float s in noiseScales)
            {
                noise += Mathf.PerlinNoise((x / s) + randomOffset, (z / s) + randomOffset) * Amplitides[i];
                i++;
            }
            noise /= Amplitides.Sum();
            if (noise < waterThreshold) { return SquareType.Water; }
            if (noise < sandThreshold) { return SquareType.Sand; }
            if (noise < grassThreshold) { return SquareType.Grass; }
            return SquareType.Grass;
        }

    }

    public class MoveOrder
    {
        public ArmyPiece piece;
        public Square targetPos;
        public List<Square> path;

        public MoveOrder(ArmyPiece piece, Square targetPos, List<Square> path)
        {
            this.piece = piece;
            this.targetPos = targetPos;
            this.path = path;
        }
    }

    public class MoveIntersection
    {
        public MoveOrder A;
        public MoveOrder B;
        public Square square;
        public int Adistance, Bdistance, Tdistance;

        public MoveIntersection(MoveOrder a, MoveOrder b, Square square, int adistance, int bdistance)
        {
            A = a;
            B = b;
            this.square = square;
            Adistance = adistance;
            Bdistance = bdistance;
            Tdistance = Adistance + Bdistance;
        }
    }
}