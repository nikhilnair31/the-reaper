using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;

public class Controller_Diary : MonoBehaviour 
{
    #region Vars
    public static Controller_Diary Instance { get; private set; }
    
    private int currentPage = 0;
    private int totalPages = 0;
    private bool isFlipping = false;
    private Vector3 startDragPosition;

    [Header("Page Content")]
    [SerializeField] private GameObject pageParentGO;
    [SerializeField] private GameObject pagePrefab;
    private List<GameObject> pageObjectsList = new ();
    private List<TextMeshProUGUI> textList = new ();

    [Header("Page Content")]
    [SerializeField] private GameObject blotPrefab;
    [SerializeField] private float blotChances = 0.1f;
    [SerializeField] private Vector2 randBlotPos = new (0f, 0f);
    [SerializeField] private Vector2 randBlotRot = new (0f, 360f);
    [SerializeField] private Vector2 randBlotScale = new (0.7f, 1.3f);
    
    [Header("Flip Settings")]
    [SerializeField] private float flipDuration = 0.5f;
    [SerializeField] private float dragThreshold = 50f;
    #endregion
    
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void InitDiaryPages(List<string> diaryRulesList) {
        if (pageParentGO == null || pagePrefab == null) return;
        
        // Clear previous pages
        foreach (GameObject obj in pageObjectsList) {
            Destroy(obj);
        }
        pageObjectsList.Clear();
        
        totalPages = diaryRulesList.Count;
        for (int i = 0; i < totalPages; i++) {
            GameObject page = Instantiate(pagePrefab, pageParentGO.transform);
            page.transform.localPosition = new Vector3(0, 145f, 0);
            
            var text = page.GetComponentInChildren<TextMeshProUGUI>();
            text.text = diaryRulesList[i];
            
            pageObjectsList.Add(page);
            textList.Add(text);

            // Start all pages on the right side, hide them except for the first one
            page.SetActive(i == 0);
            // page.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        currentPage = 0;
    }
    
    private void Update() {
        if (!isFlipping) {
            if (Input.GetMouseButtonDown(0)) {
                startDragPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0)) {
                float dragDistance = Input.mousePosition.y - startDragPosition.y;
                Debug.Log($"Drag Distance: {dragDistance} | currentPage: {currentPage} | totalPages: {totalPages}");
                
                if (Mathf.Abs(dragDistance) > dragThreshold) {
                    if (dragDistance > 0 && currentPage >= 0) {
                        FlipPage(1);
                    }
                    else if (dragDistance < 0 && currentPage <= totalPages - 1) {
                        FlipPage(-1);
                    }
                }
            }
        }
    }
    
    private void FlipPage(int direction) {
        if (isFlipping) return;

        int newPage = currentPage + direction;
        if (newPage < 0 || newPage >= totalPages) return;

        isFlipping = true;

        GameObject currentPageObj = pageObjectsList[currentPage];
        GameObject nextPageObj = pageObjectsList[newPage];

        nextPageObj.SetActive(true);

        StartCoroutine(
            SimplePageFlip(currentPageObj, nextPageObj, direction)
        );
    }
    
    private IEnumerator SimplePageFlip(GameObject currentPageObj, GameObject nextPageObj, int direction) {
        currentPageObj.SetActive(false);
        currentPage = pageObjectsList.IndexOf(nextPageObj);
        isFlipping = false;
        yield return null;
    }
    private IEnumerator AnimatePageFlip(GameObject currentPageObj, GameObject nextPageObj, int direction) {
        float endX = (direction > 0) ? 270f : 180f;
        
        Vector3 currentRotation = currentPageObj.transform.rotation.eulerAngles;
        
        isFlipping = true;
        
        currentPageObj.transform
            .DOLocalRotate(
                new (endX, currentRotation.y, currentRotation.z), 
                flipDuration
            ).OnComplete(() => {
                currentPageObj.SetActive(false);
                currentPage = pageObjectsList.IndexOf(nextPageObj);
                isFlipping = false;
            });
        
        return null;
    }

    public void SpawnBlot() {
        if (blotPrefab == null) return;

        Debug.Log($"textList: {textList.Count}");
        foreach (var text in textList) {
            Debug.Log($"text: {text.text}");
            if (Random.value < blotChances) {
                Debug.Log("Spawn Blot");
                GameObject blot = Instantiate(blotPrefab, text.transform.parent);
                blot.transform.SetLocalPositionAndRotation(
                    new (
                        Random.Range(randBlotPos.x, randBlotPos.y), 
                        0f, 
                        0f
                    ),
                    Quaternion.Euler(
                        0f, 
                        0f, 
                        Random.Range(randBlotRot.x, randBlotRot.y)
                    )
                );
                blot.transform.localScale = Vector3.one * Random.Range(randBlotScale.x, randBlotScale.y);
            }
        }
    }
}