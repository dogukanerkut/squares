using UnityEngine;
using System.Collections;

public class SelectionManager : MonoBehaviour {

	// Use this for initialization
	private GameState gameState;
	void Start ()
	{
		gameState = GameState.Idle;
	}
	
	// Update is called once per frame
	public void OnPointerDown()
	{
		print(gameObject.name);
		
	}
	void Update ()
	{

		#if UNITY_EDITOR
		if (gameState == GameState.Idle)
		{
			if (Input.GetKey(KeyCode.Mouse0))
			{
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

				if (hit.collider != null && hit.collider.tag == Constants.Tag_Tile)
				{
					Tile tileGO = hit.collider.gameObject.GetComponent<Tile>();
					print(tileGO.Row + " " + tileGO.Column);

				}
            }	
        }

		#endif

	}
}
