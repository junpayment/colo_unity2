using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	//public int m_index;
	public Sprite[] blockSprites;
	public int m_nRow;
	public int m_nType;
	//public float m_fMoveY;

	// Use this for initialization
	void Start () {

		int nRand;
		nRand=Random.Range(0, 6);

		SpriteRenderer renderer = GetComponent<SpriteRenderer>(); 
		renderer.sprite = blockSprites[nRand];
		m_nType = nRand;
	}
	
	// Update is called once per frame
	void Update () {


	}

	public int getColor(){
		return m_nType;
	}


}
