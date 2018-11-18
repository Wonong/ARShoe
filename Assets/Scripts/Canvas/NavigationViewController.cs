using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class NavigationViewController : ViewController {

    //private Stack<ViewController> viewStack = new Stack<ViewController>();
    //private ViewController currentView = null;

    [SerializeField] private Text title;

    // 사용자 입력 허가 여부 설정
    private void EnableInteraction(bool isEnabled) {
        GetComponent<CanvasGroup>().blocksRaycasts = isEnabled;
    }

    // Push newView to stack & set currentView
    public void Push(ViewController newView) {

        float rectWidth = this.CachedRectTransform.rect.width;

        if(UIManager.Instance.currentView == null){
            newView.gameObject.SetActive(true);
            UIManager.Instance.currentView = newView;
            return;
        }

        // disable user interaction while animating
        EnableInteraction(false);

        // move currentView to left of screen
        ViewController lastView = UIManager.Instance.currentView;
        UIManager.Instance.viewStack.Push(lastView);
        Vector2 lastViewPos = lastView.CachedRectTransform.anchoredPosition;
        lastViewPos.x = -rectWidth;
        lastView.CachedRectTransform.MoveTo(
            lastViewPos, 0.3f, 0.0f, iTween.EaseType.easeOutSine, () => {
                // set inactive after moving
                lastView.gameObject.SetActive(false);
        });

        // move new view to centor of screen
        newView.gameObject.SetActive(true);
        Vector2 newViewPos = newView.CachedRectTransform.anchoredPosition;
        newView.CachedRectTransform.anchoredPosition = new Vector2(rectWidth, newViewPos.y);
        newViewPos.x = 0.0f;
        newView.CachedRectTransform.MoveTo(
            newViewPos, 0.3f, 0.0f, iTween.EaseType.easeOutSine, () => {
            EnableInteraction(true);
        });

        // set new view to current view & activate back button
        UIManager.Instance.currentView = newView;
        //UIManager.Instance.toolbar.back.gameObject.SetActive(true);

        //UIManager.Instance.toolbar.ChangeActivation(UIManager.Instance.currentView.name);

    }

    public void Pop() {

        Debug.Log("Pop");
        string toPrint = UIManager.Instance.viewStack.ToArray().ToString();
        Debug.Log(toPrint);
        
        float rectWidth = this.CachedRectTransform.rect.width;

        if (UIManager.Instance.viewStack.Count < 1) return;

        // disable user interaction while animating
        EnableInteraction(false);

        // move currentView to right of screen
        ViewController lastView = UIManager.Instance.currentView;
        Vector2 lastViewPos = lastView.CachedRectTransform.anchoredPosition;
        lastViewPos.x = -rectWidth;
        lastView.CachedRectTransform.MoveTo(
            lastViewPos, 0.3f, 0.0f, iTween.EaseType.easeOutSine, () => {
                // set inactive after moving
                lastView.gameObject.SetActive(false);
        });

        ViewController poppedView = UIManager.Instance.viewStack.Pop();
        poppedView.gameObject.SetActive(true);
        Vector2 poppedViewPos = poppedView.CachedRectTransform.anchoredPosition;
        poppedViewPos.x = 0.0f;
        Debug.Log("Popped view move to");
        poppedView.CachedRectTransform.MoveTo(
            poppedViewPos, 0.3f, 0.0f, iTween.EaseType.easeOutSine, () => {
                EnableInteraction(true);
        });

        // set view from stack to currentView
        UIManager.Instance.currentView = poppedView;

        //if (UIManager.Instance.viewStack.Count >= 1) UIManager.Instance.toolbar.back.gameObject.SetActive(true);
        //else UIManager.Instance.toolbar.back.gameObject.SetActive(false);

        //UIManager.Instance.toolbar.ChangeActivation(UIManager.Instance.currentView.name);

    }

}
