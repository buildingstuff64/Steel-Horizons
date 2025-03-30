using Assets.Scripts;
using Assets.Scripts.Game_Logic.SubPieces;
using Assets.Scripts.Game_Visuals;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private float rotateSpeed = 1f;
    [SerializeField] BoardViewer Viewer;
    [SerializeField] MainBoardManager manager;
    [SerializeField] GameObject createGame;
    [SerializeField] Image teamAColor;
    [SerializeField] Image teamBColor;
    [SerializeField] RawImage boardimg;
    [SerializeField] TMP_InputField seed;

    [SerializeField] Sprite teamColorPrefab;
    public int pieceCount = 1;

    public Color[] teams = { Color.red, Color.blue, Color.green, Color.magenta };

    private float timer;

    private void Update()
    {
        if (!createGame.activeSelf) timer += Time.deltaTime;
        cam.transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            manager.createPrettyBoard();
        }

        if (timer > 10f)
        {
            timer = 0;
            manager.createPrettyBoard();
        }
    }

    private void Start()
    {
        manager.createPrettyBoard();
        teamAColor.color = teams[0];
        teamBColor.color = teams[1];
    }

    public void startGame()
    {
        SaveSystem.SaveProcedural(manager.data);

        StartSettings sp = new StartSettings();
        sp.TeamA = teamAColor.color;
        sp.TeamB = teamBColor.color;
        sp.turnCount = 15;
        for (int i = 0; i < pieceCount; i++)
        {
            sp.formationA.Add(new ArmyFormation(
                (PieceType)i+1,
                new Vector3Int(Random.Range(0, manager.data.xsize), 0, Random.Range(0, 3)),
                Vector3Int.forward));
        }


        for (int i = 0; i < pieceCount; i++)
        {
            sp.formationB.Add(new ArmyFormation(
                (PieceType)i + 1,
                new Vector3Int(Random.Range(0, manager.data.xsize), 0, Random.Range(manager.data.zsize-4, manager.data.zsize-1)),
                Vector3Int.back));
        }

        SaveSystem.Save(sp, "StartBattleSettings");

        SceneManager.LoadScene("MainGame");
    }

    public void RefreshBoard()
    {
        manager.createPrettyBoard();
        seed.text = manager.data.seed.ToString();
        updateView();
    }

    public void updateView()
    {
        boardimg.texture = manager.getImageTex();
        setSeed();
    }

    public void setSeed()
    {
        seed.text = manager.data.seed.ToString();
    }


    [System.Serializable]
    public class StartSettings
    {
        public Color TeamA;
        public Color TeamB;
        public List<ArmyFormation> formationA = new List<ArmyFormation>();
        public List<ArmyFormation> formationB = new List<ArmyFormation>();
        public int turnCount;
    }



}
