using UnityEngine;
using System.Collections;

public class LoadingBG : MonoBehaviour {

    public GameObject Background;
    private Vector3 CurrentPos;
    private float StartingPosition;
	
	void Start () {
        StartingPosition = Background.transform.localPosition.x;
    }	
	
	void FixedUpdate () {

        CurrentPos = Background.transform.localPosition;
        CurrentPos.x += 5;
        if (CurrentPos.x > StartingPosition + 250) CurrentPos.x = StartingPosition - 260;
        Background.transform.localPosition = CurrentPos;
    }
}
