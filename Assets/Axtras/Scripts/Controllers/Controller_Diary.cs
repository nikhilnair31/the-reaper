using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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
        
        totalPages = diaryRulesList.Count;
        for (int i = 0; i < totalPages; i++) {
            GameObject page = Instantiate(pagePrefab, pageParentGO.transform);
            page.GetComponentInChildren<TextMeshProUGUI>().text = diaryRulesList[i];
            pageObjectsList.Add(page);
        }
    }
    
    private void Update() {
        if (!isFlipping) {
            if (Input.GetMouseButtonDown(0)) {
                startDragPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0)) {
                float dragDistance = Input.mousePosition.x - startDragPosition.x;
                Debug.Log($"Current Pos: {Input.mousePosition.x}, startDragPosition.x: {startDragPosition.x}, Drag Distance: {dragDistance}");
                
                if (Mathf.Abs(dragDistance) > dragThreshold) {
                    if (dragDistance > 0 && currentPage > 0) {
                        FlipPage(-1);
                    }
                    else if (dragDistance < 0 && currentPage < totalPages - 1) {
                        FlipPage(1);
                    }
                }
            }
        }
    }
    private void FlipPage(int direction) {
        isFlipping = true;
        
        int newPage = currentPage + direction;
        
        if (newPage >= 0 && newPage < totalPages) {
            int oldLeftIndex = currentPage * 2;
            int oldRightIndex = currentPage * 2 + 1;
            int newLeftIndex = newPage * 2;
            int newRightIndex = newPage * 2 + 1;
            
            StartCoroutine(
                AnimatePageFlip(
                    pageObjectsList[direction > 0 ? oldRightIndex : newLeftIndex], 
                    flipDuration, 
                    direction > 0
                )
            );
            
            for (int i = 0; i < pageObjectsList.Count; i++) {
                if (pageObjectsList[i] != null) {
                    bool shouldBeActive = (i == newLeftIndex || i == newRightIndex);
                    pageObjectsList[i].gameObject.SetActive(shouldBeActive);
                }
            }
            
            currentPage = newPage;
        }
        
        isFlipping = false;
    }
    private IEnumerator AnimatePageFlip(GameObject page, float duration, bool flipForward) {
        float time = 0f;
        float startY = flipForward ? 0f : 180f;
        float endY = flipForward ? 180f : 0f;
        
        while (time < duration) {
            time += Time.deltaTime;
            float t = time / duration;
            
            Vector3 rotation = page.transform.eulerAngles;
            rotation.y = Mathf.Lerp(startY, endY, t);
            page.transform.eulerAngles = rotation;
            
            yield return null;
        }
        
        Vector3 finalRotation = page.transform.eulerAngles;
        finalRotation.y = endY;
        page.transform.eulerAngles = finalRotation;
    }
}