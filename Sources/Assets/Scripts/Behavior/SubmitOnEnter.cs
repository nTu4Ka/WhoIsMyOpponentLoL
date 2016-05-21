using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SubmitOnEnter : MonoBehaviour {

    public GameObject RegionDropdown;
    public GameObject NameInput;
    
    void OnGUI() {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) {
            MainController.Instance.PlayClickingSound(true);
            MainController.Instance.SetNewSummoner(RegionDropdown.GetComponent<Dropdown>().value, NameInput.GetComponent<InputField>().text);
        }
    }
}
