using Assets.Scripts.Game_Logic.SubPieces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public class Board 
{
    public Square[,] squares;
    public int xsize, zsize;

    public List<Piece> pieces = new List<Piece>();

    public Board(int xsize, int zsize)
    {
        squares = new Square[xsize, zsize];
        this.xsize = xsize;
        this.zsize = zsize;
    }

    public Board(int xsize, int zsize, SquareType type)
    {
        squares = new Square[xsize, zsize];
        this.xsize = xsize;
        this.zsize = zsize;

        for (int x = 0; x < xsize; x++)
        {
            for (int z = 0; z < zsize; z++)
            {
                squares[x, z] = new Square(x, z, type, this);
            }
        }
    }

    public Board(int xsize, int zsize, Func<int, int, SquareType> getSquareType)
    {
        squares = new Square[xsize, zsize];
        this.xsize = xsize;
        this.zsize = zsize;

        for (int x = 0; x < xsize; x++)
        {
            for (int z = 0; z < zsize; z++)
            {
                squares[x, z] = new Square(x, z, getSquareType(x, z), this);
            }
        }
    }


    public Square getSquare(int x, int z)
    {
        if (x < 0 || x > xsize - 1) return null;
        if (z < 0 || z > zsize - 1) return null;
        return squares[x, z];
    }

    public Square getSquare(Vector3 v)
    {
        return getSquare((int)v.x, (int)v.z);
    }

    public Square getNeighbour(Square s, Vector3 dir)
    {
        return getSquare(s.position + dir);
    }

    public List<Square> getNeighbours(Square s)
    {
        List<Square> list = new List<Square>();
        foreach (Vector3 v in directions)
        {
            list.Add(getNeighbour(s, v));
        }
        list.RemoveAll(item => item == null);
        return list;
    }

    public object addPiece(Square s, PieceType type, Vector3Int lookDirection, Color team)
    {
        if (s.hasPiece()) return null;
        switch (type)
        {
            case PieceType.None:
                Debug.Log("No piece selected.");
                break;

            case PieceType.Mech:
                s.piece = new MeleePiece(s, lookDirection, type, team);
                break;

            case PieceType.Sniper:
                s.piece = new SniperPiece(s, lookDirection, type, team);
                break;

            case PieceType.Shield:
                s.piece = new ShieldPiece(s, lookDirection, type, team);
                break;

            case PieceType.Teleporter:
                s.piece = new TeleporterPiece(s, lookDirection, type, team);
                break;

            case PieceType.Support:
                s.piece = new SupportPiece(s, lookDirection, type, team);
                break;

            case PieceType.Battleship:
                s.piece = new MeleePiece(s, lookDirection, type, team);
                break;

            case PieceType.Destroyer:
                s.piece = new SniperPiece(s, lookDirection, type, team);
                break;

            case PieceType.ShieldShip:
                s.piece = new ShieldPiece(s, lookDirection, type, team);
                break;

            case PieceType.Submarine:
                s.piece = new TeleporterPiece(s, lookDirection, type, team);
                break;

            case PieceType.Carrier:
                s.piece = new SupportPiece(s, lookDirection, type, team);
                break;

            case PieceType.Army:
                s.piece = new ArmyPiece(s, lookDirection, type, team);
                break;

            default:
                Debug.LogWarning("Unknown piece type!");
                break;
        }
        pieces.Add(s.piece);
        return s.piece.getData();
    }

    public void MovePiece(Piece p, Square s, Vector3 rotation)
    {
        if (p.square != s)
        {
            p.MoveTo(s);
        }
        p.Rotate(Vector3Int.RoundToInt(Quaternion.Euler(rotation) * Vector3.forward));

    }

    public void AttackPiece(Piece p1, Piece p2)
    {
        p1.Attack(p2);
    }

    public void removePiece(Piece p)
    {
        if (p == null) return;
        p.remove();
    }

    public void updateLogic()
    {
        foreach (Square square in squares) square.isBlocked = false;

        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].isSupported = false;
            if (pieces[i].type == PieceType.Support)
            {
                pieces[i].onStartTurn();
            }
        }

        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].onStartTurn();
        }
    }

    public List<Square> getPath(Square start, Square end, SquareType[] walkableTypes)
    {
        if (!walkableTypes.Contains(end.type)) return null;

        foreach (Square s in squares) { s.resetPathfinding(); }
        List<Square> open = new List<Square>();
        List<Square> closed = new List<Square>();
        open.Add(start);

        while (open.Count > 0)
        {
            Square node = open[0];
            //node.GetComponent<MeshRenderer>().material.color = Color.blue;
            for (int i = 0; i < open.Count; i++)
            {
                if (open[i].fCost < node.fCost || open[i].fCost == node.fCost)
                {
                    if (open[i].hCost < node.hCost) node = open[i];
                }
            }

            open.Remove(node);
            closed.Add(node);

            if (node == end)
            {
                return getPathRetraced(start, end);
            }

            foreach (Square neighbour in node.getNeighbours())
            {
                if (neighbour.isBlocked || closed.Contains(neighbour)) continue;
                if (!walkableTypes.Contains(neighbour.type)) continue;

                float newCostToNeighbour = node.gCost + Vector3.Distance(node.position, neighbour.position);
                if (newCostToNeighbour < neighbour.gCost || !open.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = Vector3.Distance(neighbour.position, end.position);
                    neighbour.prev = node;

                    if (!open.Contains(neighbour)) open.Add(neighbour);
                }
            }
        }
        return null;
    }

    public List<Square> getStraightWithBackup(Square start, Square end, SquareType[] walkableTypes) 
    {
        List<Square> path = new List<Square>();
        Square current = start;
        Square AStart = null;
        Square AEnd = null;

        List<Square> normalPath = getPath(start, end, walkableTypes);
        if (normalPath == null) return null;

        while (current != end)
        {
            Square next = current;

            if (Mathf.Abs(end.x - start.x) > Mathf.Abs(end.z - start.z))
            {
                if (current.x != end.x)
                {
                    next = getSquare(current.x + (end.x > current.x ? 1 : -1), current.z);
                }

                else if (current.z != end.z)
                {
                    next = getSquare(current.x, current.z + (end.z > current.z ? 1 : -1));
                }
            }
            else
            {
                if (current.z != end.z)
                {
                    next = getSquare(current.x, current.z + (end.z > current.z ? 1 : -1));
                }

                else if (current.x != end.x)
                {
                    next = getSquare(current.x + (end.x > current.x ? 1 : -1), current.z);
                }
            }

            if (!walkableTypes.Contains(next.type))
            {
                if (walkableTypes.Contains(current.type))
                {
                    AStart = next;
                }

            }
            else
            {
                if (!walkableTypes.Contains(current.type))
                {
                    AEnd = next;
                    List<Square> list = getPath(AStart, AEnd, walkableTypes);
                    if (list == null)
                    {
                        return normalPath;
                    }
                    path.AddRange(list);
                }
                else
                {
                    path.Add(next);
                }

            }

            current = next;
        }

        if (path.Count > normalPath.Count + 10) return normalPath;
        return path;
    }

    private List<Square> getPathRetraced(Square start, Square end)
    {
        List<Square> path = new List<Square>();
        Square currentNode = end;


        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.prev;
        }
        path.Add(start);
        path.Reverse();

        return path;
    }


    public static readonly Vector3[] directions =
    {
        Vector3.forward,
        Vector3.back,
        Vector3.right,
        Vector3.left
        };
}
