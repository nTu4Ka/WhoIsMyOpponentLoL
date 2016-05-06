using UnityEngine;
using UnityEngine.UI;

public class SummonerSelect : MonoBehaviour {

    public GameObject RegionDropdown;
    public GameObject NameInput;
    public GameObject RegionSelect;


    public void SubmitSummoner() {
        MainController.Instance.SetNewSummoner(RegionDropdown.GetComponent<Dropdown>().value, NameInput.GetComponent<InputField>().text);
    }
}
