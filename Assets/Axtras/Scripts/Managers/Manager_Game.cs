using UnityEngine;
using System.Collections.Generic;
using GogoGaga.OptimizedRopesAndCables;
using System.Linq;

public class Manager_Game : MonoBehaviour 
{
    #region Vars
    public static Manager_Game Instance { get; private set; }
    
    private GameObject playerGO;
    private GameObject mainCameraGO;
    private Data_General generalData = new();

    [Header("Gen Settings")]
    [SerializeField] private List<Transform> hangingPointsTransformList;
    [SerializeField] private GameObject corpsePrefab;
    [SerializeField] private int maxCorpses = 3;

    [Header("Run Settings")]
    [SerializeField] private int run = 0;
    
    [Header("Score Settings")]
    [SerializeField] private int score = 0;
    [SerializeField] private int scoreInc = 10;
    [SerializeField] private int scoreDec = 15;
    #endregion

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        Init();
        DataLoad();
    }
    private void Init() {
        if (playerGO == null)
            playerGO = GameObject.FindGameObjectWithTag("Player");
        if (mainCameraGO == null)   
            mainCameraGO = Camera.main.gameObject;

        // Initialize the hangin points if not
        if (hangingPointsTransformList.Count == 0) {
            var tree = GameObject.FindGameObjectWithTag("Tree");
            if (tree != null) {
                var allChilds = tree.GetComponentsInChildren<Transform>();
                hangingPointsTransformList = allChilds.Skip(1).ToList();
                Debug.Log($"Assigned {hangingPointsTransformList.Count} hanging points!");
            }
            else {
                Debug.LogError($"Couldn't find the tree!");
            }
        }
    }
    private void DataLoad() {
        // Load general data from save file
        var loadedGeneralData = Manager_SaveLoad.Instance.LoadGeneral();
        if (loadedGeneralData == null) {
            Manager_SaveLoad.Instance.SaveGeneral();
        }
        
        // Set the run and score values
        SetRunCnt(generalData.runCnt);
        SetScore(generalData.scoreCnt);
    }

    #region Run Related
    public void StartRun() {
        Debug.Log("Game Run Started");

        // Clear corpses
        var corpses = GameObject.FindGameObjectsWithTag("Corpse");
        foreach (var corpse in corpses) {
            Destroy(corpse.transform.parent.gameObject);
        }

        // Spawn corpses
        var pointToPickFrom = hangingPointsTransformList;
        for (int i = 0; i < maxCorpses; i++) {
            var corpse = Instantiate(corpsePrefab);
            var rope = corpse.transform.GetComponentInChildren<Rope>();
            var randIndex = Random.Range(0, pointToPickFrom.Count);
            var randHangPoint = pointToPickFrom[randIndex];
            rope.startPoint.position = randHangPoint.position;
        }

        // Prewarm physics steps
        for (int i = 0; i < 100; i++) {
            Physics.simulationMode = SimulationMode.Script;
            Physics.Simulate(Time.fixedDeltaTime);
        }
        Physics.simulationMode = SimulationMode.FixedUpdate;

        // Set time to normal speed
        Time.timeScale = 1f;
        
        // Add run count
        run++;
    }
    public void EndRun() {
        Debug.Log("Game Run Ended");
        
        // Pick a random story from the list
        var story = Manager_Content.Instance.PickStory();
        Manager_UI.Instance.SetStoryText(story);

        // Pick and unlock a random rule from the list
        Manager_Content.Instance.PickRule();

        // Save the current set of stories and rules
        Manager_SaveLoad.Instance.SaveStories();
        Manager_SaveLoad.Instance.SaveRules();

        // Set time to frozen
        Time.timeScale = 0f;
    }
    #endregion

    #region Score Related
    public void IncScore() {
        Debug.Log($"IncScore | Score: {score} | ScoreInc: {scoreInc}");
        score += scoreInc;
    }
    public void DecScore() {
        Debug.Log($"DecScore | Score: {score} | ScoreInc: {scoreInc}");
        score -= scoreInc;
    }
    #endregion

    #region Save Load Related
    public Data_General GetGeneral() {
        return generalData;
    }

    private void SetRunCnt(int runcnt) {
        run = runcnt;
    }
    private void SetScore(int scoreval) {
        score = scoreval;
    }
    #endregion
}