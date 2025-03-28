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
    private List<GameObject> pageObjectsList = new List<GameObject>();
    
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
            page.GetComponentInChildren<TextMeshProUGUI>().text = diaryRulesList[i];
            pageObjectsList.Add(page);

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
                // Debug.Log($"Drag Distance: {dragDistance} | currentPage: {currentPage} | totalPages: {totalPages}");
                
                if (Mathf.Abs(dragDistance) > dragThreshold) {
                    if (dragDistance > 0 && currentPage >= 0) {
                        FlipPage(1);
                    }
                    else if (dragDistance < 0 && currentPage < totalPages - 1) {
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
}
