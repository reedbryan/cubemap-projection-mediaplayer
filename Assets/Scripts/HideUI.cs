using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HideUI : MonoBehaviour
{
    public GameObject canvas;
    public Button hideButton;  
    [SerializeField]private bool UI_hidden = false;

    void Start()
    {
        hideButton.onClick.AddListener(OnHideButton);
    }

    void Update(){
        // Only toggle with Tab, or any key if UI is hidden (but not Tab twice)
        if (Input.GetKeyDown(KeyCode.Tab)){
            OnHideButton();
        }
        else if (Input.anyKeyDown && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) && UI_hidden){
            OnHideButton();
        }
    }

    public void OnHideButton(){
        Debug.Log("Hide UI toggle");

        if (UI_hidden){
            UI_hidden = false;
            canvas.SetActive(true);
        }
        else{
            UI_hidden = true;
            canvas.SetActive(false);
        }
    }
}
