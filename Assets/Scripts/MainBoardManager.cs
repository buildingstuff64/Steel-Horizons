using Assets.Scripts.Game_Logic.SubPieces;
using Assets.Scripts.Game_Visuals;
using Assets.Scripts.Game_Visuals.Visual_Sub_Pieces;
using Assets.Scripts.UI;
using DG.Tweening;
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
        [SerializeField] private bool create_on_start = true;
        [SerializeField] private bool loadFromDisk = true;
        [SerializeField] public ProceduralData data;
        [SerializeField] public MainBoardPlayerController controller;

        int xsize, zsize;

        [Header("Sub Board Parameters")]
        public int subBoardSize = 10;

        public List<MoveOrder> moveOrders = new List<MoveOrder>();
        public List<Square> Viewintersetions = new List<Square>();

        private void Awake()
        {
            Viewer = GetComponent<BoardViewer>();
            xsize = data.xsize;
            zsize = data.zsize;
        }

        public void addMoveOrder(ArmyPiece p, Square s, List<Square> path)
        {
            MoveOrder moveOrder = new MoveOrder(p, s, path);
            moveOrders.Add(moveOrder);
        }

        public MoveOrder getPieceMoveOrder(ArmyPiece p)
        {
            foreach (var order in moveOrders)
            {
                if (order.piece == p)
                {
                    return order;
                }
            }
            return null;
        }


        public List<MoveIntersection> finalIntersection = new List<MoveIntersection>();
        List<Piece> found = new List<Piece>();
        List<MoveOrder> finalOrders = new List<MoveOrder>();
        public (List<MoveOrder>, List<MoveIntersection>) moveAllPieces()
        {
            Viewintersetions.Clear();

            found.Clear();
            finalOrders.Clear();
            finalIntersection.Clear();

            while (moveOrders.Count > 0)
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
                            //print(string.Format("{0} & {1} > {2}", order.piece.team, rest.piece.team, index + i));
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
            if (!create_on_start) return;
            if (loadFromDisk) SaveSystem.LoadProcedural(data);
            createMainBoard();
            setupPieces();

        }

        public void createMainBoard()
        {
            if (!loadFromDisk) data.seed = Random.Range(0, 1000000f);
            board = new Board(xsize, zsize, getSquareType);
            board.createStructures(16);

            Viewer.Set(this);
            Viewer.UpdateView(true);
        }

        public void createPrettyBoard()
        {

            if (!loadFromDisk) data.seed = Random.Range(0, 1000000f);
            board = new Board(xsize, zsize, getSquareType);

            Viewer.Set(this);
            Viewer.UpdateView(false);
        }

        private SquareType getSquareType(int x, int z)
        {
            float noise = 0;
            int i = 0;
            foreach (float s in data.noiseScales)
            {
                noise += Mathf.PerlinNoise((x / s) + data.seed, (z / s) + data.seed) * data.Amplitides[i];
                i++;
            }
            noise /= data.Amplitides.Sum();

            noise *= getGradient(x, z);

            if (noise < data.waterThreshold) { return SquareType.Water; }
            if (noise < data.sandThreshold) { return SquareType.Sand; }
            if (noise < data.grassThreshold) { return SquareType.Grass; }
            return SquareType.Grass;
        }

        private float getGradient(int x, int z)
        {
            float maxrad = Vector2.Distance(new Vector2(0, 0), new Vector2(xsize * .5f, zsize * .5f));
            float distance = Vector2.Distance(new Vector2(x, z), new Vector2(xsize * .5f, zsize * .5f));
            float norm = Mathf.Clamp(distance / maxrad, 0, 1);
            return Mathf.Pow(1f - norm, data.strength) * data.amp;
        }

        public Texture2D getImageTex()
        {
            return Viewer.getBoardTexture();
        }

        public void setupPieces()
        {
            MenuController.StartSettings sp = SaveSystem.Load<MenuController.StartSettings>("StartBattleSettings") as MenuController.StartSettings;
            controller.teamA = sp.TeamA;
            controller.teamB = sp.TeamB;

            foreach (ArmyFormation f in sp.formationA) createArmy(f, controller.teamA);

            foreach (ArmyFormation f in sp.formationB) createArmy(f, controller.teamB);

            Viewer.UpdatePieceGameobjects();

            controller.turnsleft = sp.turnCount;

        }

        private void createArmy(ArmyFormation data, Color team)
        {
            print(data.piece);
            ArmyPiece p = (ArmyPiece)board.addPiece(board.getSquare(data.position), PieceType.Army, data.lookDirection, team);
            int[] pt = { 1, 2, 3, 4, 5 };
            pt = pt.OrderBy(x => Random.value).ToArray();
            for (int i = 0; i < 5; i++)
            {
                p.formation.Add(new ArmyFormation((PieceType)pt[i], new Vector3Int(Random.Range(0, 3), 0, (i * 2) + Random.Range(0,1)), Vector3Int.right));
            }
        }

        public void endGame()
        {
            transform.DOScale(0, 1f).SetEase(Ease.InExpo);
            transform.DOMoveY(-500, 1f).SetEase(Ease.InExpo).OnComplete(() => { transform.gameObject.SetActive(false); });
            int Atcount = 0;
            int Btcount = 0;
            foreach (Piece p in board.pieces)
            {
                if (p.type == PieceType.Structure)
                {
                    StructurePiece sp = p as StructurePiece;
                    if (sp.team == controller.teamA) Atcount++;
                    if (sp.team == controller.teamB) Btcount++;
                }
            }
            if (Atcount > Btcount) UIhud.instance.showWinner("Team A Wins", controller.teamA);
            if (Atcount < Btcount) UIhud.instance.showWinner("Team B Wins", controller.teamB);
            if (Atcount == Btcount) UIhud.instance.showWinner("Draw", Color.grey);

        }

        public void checkend()
        {
            bool a = false, b = false;

            foreach (Piece p in board.pieces)
            {
                if (p.square.hasStructure()) continue;
                if (p.team == controller.teamA) { a = true; continue; }
                if (p.team == controller.teamB) { b = true; continue; }

            }
            if (!a || !b) {
                transform.DOScale(0, 1f).SetEase(Ease.InExpo);
                transform.DOMoveY(-500, 1f).SetEase(Ease.InExpo).OnComplete(() => { transform.gameObject.SetActive(false); });
            }
            if (!b) UIhud.instance.showWinner("Team A Wins", controller.teamA);
            if (!a) UIhud.instance.showWinner("Team B Wins", controller.teamB);



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