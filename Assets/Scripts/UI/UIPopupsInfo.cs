using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game_Logic;
using Assets.Scripts.Game_Visuals;
using static Assets.Scripts.Game_Visuals.PrefabManager;
using System;
using Assets.Scripts;

// string name, image movement, image attack pattern, string support buff, int cost
public static class UIPopupsInfo
{
 
    static public string getPieceName(PieceType t){
        switch(t){
        case PieceType.Mech: return "Mech";
        case PieceType.Sniper: return "Sniper";
        case PieceType.Shield: return "Defender";
        case PieceType.Teleporter: return "Assassin";
        case PieceType.Support: return "Supporter";
        case PieceType.Battleship: return "Battleship";
        case PieceType.Destroyer: return "Destroyer";
        case PieceType.ShieldShip: return "Juggernaut";
        case PieceType.Submarine: return "Submarine";
        case PieceType.Carrier: return "Aircraft Carrier";
        default: return "Piece name not found!1!!1!!";
        }
    }

    static public string getPieceDesc(PieceType t){
        switch(t){
        case PieceType.Mech: return "Melee Attacker: The Mech moves quickly and attacks immediately from close range.";
        case PieceType.Sniper: return "Ranged Attacker: The Sniper moves carefully, taking a turn to set up a long-range attack.";
        case PieceType.Shield: return "Bulwark: The Defender deploys a shield, blocking all movement and attacks.";
        case PieceType.Teleporter: return "Teleporter: The Assassin instantly appears across the board in diagonal lines.";
        case PieceType.Support: return "Support Buff: The Supporter gives buffs to your other adjacent pieces.";
        case PieceType.Battleship: return "Melee Attacker: The Battleship moves quickly and attacks immediately from close range.";
        case PieceType.Destroyer: return "Ranged Attacker: The Destroyer moves carefully, taking a turn to set up a long-range attack.";
        case PieceType.ShieldShip: return "Bulwark: The Juggernaut deploys a shield, blocking all movement and attacks.";
        case PieceType.Submarine: return "Teleporter: The Submarine instantly appears across the board in diagonal lines.";
        case PieceType.Carrier: return "Support Buff: The Aircraft Carrier gives buffs to your other adjacent pieces.";
        default: return "Piece description not found!1!!1!!";
        }
    }

    static public string getPieceBuff(PieceType t){
        switch(t){
        case PieceType.Mech: return "This piece cannot be buffed.";
        case PieceType.Sniper: return "Piercing Shots: Allows the Sniper to shoot through enemies and obstacles.";
        case PieceType.Shield: return "Extra Shield: Widens the Defender's shield from 3 to 5 squares.";
        case PieceType.Teleporter: return "Movement Boost: The Assassin can move across the board in straight lines.";
        case PieceType.Support: return "This piece cannot be buffed.";
        case PieceType.Battleship: return "This piece cannot be buffed.";
        case PieceType.Destroyer: return "Piercing Shots: Allows the Destroyer to shoot through enemies and obstacles.";
        case PieceType.ShieldShip: return "Extra Shield: Widens the Juggernaut's shield from 3 to 5 squares.";
        case PieceType.Submarine: return "Movement Boost: The Submarine can move across the board in straight lines.";
        case PieceType.Carrier: return "This piece cannot be buffed.";
        default: return "Piece buff not found!1!!1!!";
        }
    }

    static public int getPieceCost(PieceType t){

        switch(t){
        case PieceType.Mech: return 1;
        case PieceType.Sniper: return 3;
        case PieceType.Shield: return 3;
        case PieceType.Teleporter: return 2;
        case PieceType.Support: return 2;
        case PieceType.Battleship: return 1;
        case PieceType.Destroyer: return 3;
        case PieceType.ShieldShip: return 3;
        case PieceType.Submarine: return 2;
        case PieceType.Carrier: return 2;
        default: return 0;
        }
    }

    static public PieceType getRealType(PieceType p, SquareType t)
    {
        if (t == SquareType.Water)
        {
            switch (p)
            {
                case PieceType.Mech:
                    return PieceType.Battleship;
                case PieceType.Sniper:
                    return PieceType.Destroyer;
                case PieceType.Shield:
                    return PieceType.ShieldShip;
                case PieceType.Teleporter:
                    return PieceType.Submarine;
                case PieceType.Support:
                    return PieceType.Carrier;
            }
        }
        return p;
    }

    static public Texture2D selectionTexture = new Texture2D(7, 7);
    static public Color backgroundColor = new Color(0.25f, 0.25f, 0.25f, 0.25f);
    static public Texture2D getAttacksMoves(PieceType t, bool AM, bool isSupported){
        selectionTexture.filterMode = FilterMode.Point;
        Board b = new Board(7, 7, SquareType.None);
        b.addPiece(b.getSquare(3, 3), t, Vector3Int.forward, Color.green);
        Piece p = b.getSquare(3, 3).piece;

        if (AM) 
        { 
            foreach (Square s in b.squares) 
            {
                if (s.hasPiece()) continue;
                b.addPiece(s, PieceType.Mech, Vector3Int.forward, Color.red); 
            } 
        }

        p.isSupported = isSupported;
        for (int x = 0; x < b.xsize; x++){
            for (int z = 0; z < b.zsize; z++){
                selectionTexture.SetPixel(x, z, backgroundColor);
            }
        }

        if (AM) { setSelectedSquares(p.getAvaiableAttacks(p.square), Color.red); }
        else { setSelectedSquares(p.getAvaliableMoves(p.square), Color.yellow); }
        selectionTexture.SetPixel(3, 3, Color.green);
        selectionTexture.Apply();
        return selectionTexture;
    }

    static public void setSelectedSquares(List<Square> squares, Color c){
        foreach (Square square in squares){
            setSelectedSquare(square, c);
        }
    }

    static public void setSelectedSquare(Square square, Color c){
        selectionTexture.SetPixel(square.x, square.z, c);
    }

    static public void clearSelection(){
        for (int x = 0; x < selectionTexture.width; x++){
            for (int z = 0; z < selectionTexture.height; z++){
                selectionTexture.SetPixel(x, z, new Color(0, 0, 0, 0));
            }
        }
        selectionTexture.Apply();
    }

    static public void saveInfoasjson()
    {
        foreach (PieceType t in Enum.GetValues(typeof(PieceType)))
        {
            Debug.Log(t);
            if (PieceType.None == t) continue;
            PieceUIInfo i = new PieceUIInfo(t, getPieceName(t), getPieceDesc(t), getPieceBuff(t), getPieceCost(t));
        }
    }

    public class PieceUIInfo
    {
        public PieceType pieceType;
        public string Name;
        public string Description;
        public string Buff;
        public int Cost;

        public PieceUIInfo(PieceType type, string Name, string Description, string Buff, int Cost)
        {
            this.pieceType = type;
            this.Name = Name;
            this.Description = Description;
            this.Buff = Buff;
            this.Cost = Cost;
        }

        public void SaveJson()
        {
            SaveSystem.Save(this, string.Format("PieceInfo/{0}_Info", pieceType.ToString()));
        }

    }
}
