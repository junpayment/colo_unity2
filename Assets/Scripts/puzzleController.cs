using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class puzzleController : MonoBehaviour {
	public GameObject m_obj;

	private float BLOCKAREASPACE = 0.9f;
	private int MAX_ROW = 6;
	private int MAX_CLM = 6;
	private static int EMIT_Y = 7;


	[SerializeField]
	private List<GameObject> m_BlockList= new List<GameObject>();

	private enum state{
		STOP,
		MOVE,
		CHECK_TRICK,
	};

	private state m_nState;

	//-----------------------------------------------------------------------------
	void Start () {
		m_nState = state.STOP;

		for(int nRow=0;nRow<MAX_ROW;nRow++)
		{
			for(int nClm=0;nClm<MAX_CLM;nClm++)
			{

				createBrock(nRow,nClm);
//				GameObject instance = (GameObject)Instantiate(m_obj);
//				Vector3 tmp = gameObject.transform.position;
//				tmp.x += BLOCKAREASPACE*nRow;
//				tmp.y += BLOCKAREASPACE*nClm;
//				instance.transform.position = tmp;
//  
//				instance.GetComponent<Block>().m_nRow = nRow; 
			}
		}
		BlockSort ();
	}
	//-----------------------------------------------------------------------------
	void fire(){
		int nRand;
		nRand=Random.Range(0, 3);

		switch (nRand) {
		case 0:
			GameObject.Find ("ActionController").GetComponent<ActionController> ().input (ActionController.command.hadoken);
			break;
		case 1:
			GameObject.Find ("ActionController").GetComponent<ActionController> ().input (ActionController.command.shoryuken);
			break;
		case 2:
			GameObject.Find ("ActionController").GetComponent<ActionController> ().input (ActionController.command.tasumaki);
			break;
		}

	}

	//-----------------------------------------------------------------------------
	void Update () {

		// レイキャスト取得
		//Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		if (Input.GetMouseButtonDown(0)){ // Input.GetMouseButtonDown(0)でマウスクリック取得
			Vector2 tapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			RaycastHit2D hitobj = Physics2D.Raycast(tapPoint,-Vector2.up);
			Debug.Log(hitobj.collider.gameObject.name);

			int nWhichRowObjDestroy = hitobj.collider.gameObject.GetComponent<Block>().m_nRow;

			m_BlockList.Remove(hitobj.collider.gameObject);
			GameObject.Destroy(hitobj.collider.gameObject);

			createBrock(nWhichRowObjDestroy,EMIT_Y);
			m_nState = state.MOVE;
			fire ();
		}

		
		if (m_nState == state.MOVE) {
			if (isBlocksStoping ()) {
				Debug.Log (" stopping... ");
				m_nState = state.CHECK_TRICK;
			}
		} else if (m_nState == state.CHECK_TRICK) {
			//Debug.Log (" CHECK_TRICK... ");
			m_nState = state.STOP;
			//checkAndRemoveBlocksOnlyRow();
		}
		BlockSort ();

	}
	//-----------------------------------------------------------------------------
	private void BlockSort(){
		Debug.Log ("Block Sort");
		m_BlockList = m_BlockList
			.OrderBy( item => item.transform.position.y)
				.ThenBy( item2=> item2.transform.position.x).ToList();
		for (int nCount = 0; nCount < m_BlockList.Count; nCount++){
			
			m_BlockList[nCount].name = nCount.ToString();
		}
	}
	//-----------------------------------------------------------------------------
	private void checkAndRemoveBlocksOnlyRow(){
		Debug.Log ("checkAndRemoveBlocksOnlyRow()");

		List<List<int>> groups = new List<List<int>> ();
		for(int nClm=0; nClm < MAX_CLM; nClm++){
			groups.Add(new List<int> ());
		}

		for (int nCount = 0; nCount < m_BlockList.Count; nCount++) {
			int nRow = nCount % MAX_ROW;
			int nClm = nCount / MAX_CLM;

			for (int n = nRow; n < MAX_ROW; n++) {
				int nCurr = nRow + nClm*MAX_CLM;
				int nTgt = nRow+n + nClm*MAX_CLM;
				Debug.Log("r"+nRow+"c"+nClm);
				int nColor = m_BlockList[nCurr].GetComponent<Block>().getColor();
				if(nColor != m_BlockList[nTgt].GetComponent<Block>().getColor()){
					break;
				}
				nCount = nCount + n;
				groups[nClm].Add(nTgt);
			}
		}

		foreach(List<int> list in groups){
			bool bisremoved = false;
			foreach(int nObj in list){
				GameObject obj = GameObject.Find(nObj.ToString());
				if(obj == null) break;
				
				int nEmitRow = GameObject.Find(nObj.ToString()).GetComponent<Block>().m_nRow;
				createBrock(nEmitRow,EMIT_Y);
				m_BlockList.Remove(GameObject.Find(nObj.ToString()));
				GameObject.Destroy(GameObject.Find(nObj.ToString()));
				bisremoved = true;
			}
			if(bisremoved){
				fire();
			}
		}
		BlockSort ();
	}

	//-----------------------------------------------------------------------------
	/*
	private void checkAndRemoveBlocks(){
		Debug.Log ("checkAndRemoveBlocks()");

		List<List<int>> groups = new List<List<int>> ();
//		for (int n = 0; n<6; n++) {
//			groups.Add(new List<int> ());
//		}

		for (int nCount = 0; nCount < m_BlockList.Count; nCount++){
			int nTop = nCount - MAX_CLM;
			int nLeft = nCount - 1;
			int nRight = nCount + 1;
			int nBottom = nCount + MAX_CLM;
			//Debug.Log("t"+nTop+",l"+nLeft+",r"+nRight+",b"+nBottom);

			int nCurrType = m_BlockList[nCount].GetComponent<Block>().getColor();

//			for(int n = 0; n<groups.Count; n++){
//				if(groups[n].Contains(nCount)){
//					break;
//				}
//			}

			int nGroupNum;
			if(isBlockExist(nTop) && isBlockExist(nLeft) && isBlockExist(nRight) && isBlockExist(nBottom)){
				groups.Add(new List<int> ());
				nGroupNum = groups.Count - 1;
			}else{
				break;
			}

			if(isBlockExist(nTop)){
				if(nCurrType == m_BlockList[nTop].GetComponent<Block>().getColor()){
					if(!groups[nGroupNum].Contains(nTop)){
						groups[nGroupNum].Add(nTop);
					}
				}
			}
			if(isBlockExist(nLeft)){
				if(nCurrType == m_BlockList[nLeft].GetComponent<Block>().getColor()){
					if(!groups[nGroupNum].Contains(nLeft)){
						groups[nGroupNum].Add(nLeft);
					}
				}
			}
			if(isBlockExist(nRight)){
				if(nCurrType == m_BlockList[nRight].GetComponent<Block>().getColor()){
					if(!groups[nGroupNum].Contains(nBottom)){
						groups[nGroupNum].Add(nBottom);
					}
				}
			}
			if(isBlockExist(nBottom)){
				if(nCurrType == m_BlockList[nBottom].GetComponent<Block>().getColor()){
					if(!groups[nGroupNum].Contains(nRight)){
						groups[nGroupNum].Add(nRight);
					}
				}
			}

		}

		foreach(List<int> list in groups){
			foreach(int nObj in list){
				GameObject obj = GameObject.Find(nObj.ToString());
				if(obj == null) break;

				int nEmitRow = GameObject.Find(nObj.ToString()).GetComponent<Block>().m_nRow;
				createBrock(nEmitRow,EMIT_Y);
				m_BlockList.Remove(GameObject.Find(nObj.ToString()));
				GameObject.Destroy(GameObject.Find(nObj.ToString()));

			}
		}
		BlockSort ();
	}
	*/

	//-----------------------------------------------------------------------------
	private bool isBlockExist(int nPosition){
		if (nPosition < 0) {
			return false;
		} else if (nPosition >= MAX_ROW * MAX_CLM) {
			return false;
		}
		return true;
	}
	//-----------------------------------------------------------------------------

	//-----------------------------------------------------------------------------
	private bool isBlocksStoping(){
		Block[] blocks = FindObjectsOfType (typeof(Block)) as Block[];
		foreach (Block block in blocks) {
			if(block.rigidbody2D.velocity.magnitude != 0){
				return false;
			}
		}
		return true;
	}

	//-----------------------------------------------------------------------------
	private void createBrock(int nRow, int nClm){
		Debug.Log ("createBrock >>> " + nRow);

		GameObject instance = (GameObject)Instantiate(m_obj);
		Vector3 tmp = gameObject.transform.position;
		tmp.x += BLOCKAREASPACE*nRow;
		tmp.y += BLOCKAREASPACE*nClm;
		instance.transform.position = tmp;
		instance.GetComponent<Block>().m_nRow = nRow;
		m_BlockList.Add (instance);

	}
	//-----------------------------------------------------------------------------
}
