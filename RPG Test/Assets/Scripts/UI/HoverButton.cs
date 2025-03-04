using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject hoverButton;
    [SerializeField] private GameObject defaultButton;

    private bool isHovering = false;

    private void Start() {
        hoverButton.SetActive(false);
        defaultButton.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (isHovering) {
            UpdateButtonAppearance(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        isHovering = true;
        UpdateButtonAppearance();
    }

    public void OnPointerExit(PointerEventData eventData) {
        isHovering = false;
        UpdateButtonAppearance();
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (isHovering) {
            UpdateButtonAppearance(); 
        }
    }

    private void UpdateButtonAppearance(bool isPressed = false) {
        if (isPressed) {

        } else {
            hoverButton.SetActive(isHovering);
            defaultButton.SetActive(!isHovering);
        }
    }
   

    

}
