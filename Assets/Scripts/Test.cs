using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){

		if ( GUI.Button( new Rect(100, 100, 100, 20), "hoge" ) )
		{
			ActionController ac =  gameObject.GetComponent<ActionController>();
			ac.input(ActionController.command.hadoken);
		}
	}


}
