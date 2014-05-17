using UnityEngine;
using System.Collections;

public class TouchManager : MonoBehaviour
{
	// 自身のインスタンス
	private static TouchManager instance = null;
	// Rayを飛ばすカメラ
	private Camera myCamera;
	// Ray
	private Ray ray;
	// Rayがヒットしたものの情報 
	private RaycastHit hit;
	
	// RayがヒットしたGameObjectを格納
	public static GameObject selectedGameObject;
	
	void Awake ()
	{
		// TouchManagerの唯一のインスタンスを生成
		if (instance == null) {
			instance = this;
		} else {
			Destroy (gameObject);
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		myCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update ()
	{
		#if UNITY_EDITOR
		// UnityEditorの時のみ（クリック）
		if(Input.GetMouseButtonDown(0)){
			Debug.Log("Clicked");
			Vector3 clickDeltaPosition = Input.mousePosition;
			ray = myCamera.ScreenPointToRay(clickDeltaPosition);
			
			if(Physics.Raycast(ray, out hit)){
				// RayがヒットしたGameObjectをstaticなクラス変数に格納
				selectedGameObject = hit.collider.gameObject;
			}if(Input.GetMouseButtonUp(0)){
				selectedGameObject = null;
			}
			Debug.Log("hit" + hit);
		}
		#elif UNITY_ANDROID
		// タッチされている
		if (Input.touchCount == 1) {
			Debug.Log ("touch");
			
			Touch t = Input.touches [0];
			
			// タッチの位置からRayを発射
			ray = myCamera.ScreenPointToRay (Input.touches [0].position);
			
			if (t.phase == TouchPhase.Ended && Physics.Raycast (ray.origin, ray.direction, out hit, Mathf.Infinity)) {
				// RayがヒットしたGameObjectをstaticなクラス変数に格納
				selectedGameObject = hit.collider.gameObject;	
			} else if (t.phase == TouchPhase.Ended && !Physics.Raycast (ray.origin, ray.direction, out hit, Mathf.Infinity)) {
				selectedGameObject = null;	
			}
		}
		#endif
	}
}