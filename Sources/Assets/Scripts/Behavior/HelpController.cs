using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HelpController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameObject HelpPanel;
    public Sprite SpriteInactive;
    public Sprite SpriteHovered;

    public void OnPointerEnter(PointerEventData eventData) {
        HelpPanel.SetActive(true);
        GetComponent<Image>().sprite = SpriteHovered;
    }



    public void OnPointerExit(PointerEventData eventData) {
        HelpPanel.SetActive(false);
        GetComponent<Image>().sprite = SpriteInactive;
    }

}
