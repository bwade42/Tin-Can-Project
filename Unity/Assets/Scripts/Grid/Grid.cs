using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grid class.
/// </summary>
public class Grid : MonoBehaviour {

	public List<Cell> cells = new List<Cell>(); // a list of all the cells in the grid

	public int rows; // Grid Height
	public int cols; // Grid Width

	public int cell_id = 1; // cell identifier; used for debugging

	private ClientManager clientManager; //gives access to sending and recieving messages via network

	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start()
	{
		InitializeGrid ();
		clientManager = GameObject.FindGameObjectWithTag ("network").GetComponent<ClientManager> ();

	}

	/// <summary>
	/// Update this instance every frame.
	/// </summary>
	public void Update()
	{
		CheckForUserInput();
		UpdateGrid ();
	}

	/// <summary>
	/// Initializes the grid with cells.
	/// </summary>
	public void InitializeGrid()
	{
		for (int x = 0; x < cols; x++) {
			for (int z = 0; z < rows; z++) {     

				Vector3 cell_position = new Vector3 (x * 2, 0, z * 2); // position of cells be double the length from each neighbor
				Cell tempCell = new Cell (cell_id,cell_position); // create a new cell with specified position 
				tempCell.SetCellID (cell_id);

				cells.Add (tempCell); // add the newly created cell to a list 
		
				cell_id++;
			}
		}

		Vector3 rotationVector = transform.rotation.eulerAngles; // reference to current rotation of parent
		rotationVector.x = 90; // set x component to 90 degrees
		transform.rotation = Quaternion.Euler(rotationVector); // set the current parents rotation to 90 degrees

	}
	/// <summary>
	/// Updates the grid by changing the color of each cell based on its state
	/// </summary>
	public void UpdateGrid()
	{
		foreach (Cell c in cells) {
			if (c.isAlive) {
				ChangeCellColor (c, Color.red);
			}

			else if (!c.isAlive) {
				ChangeCellColor (c, Color.black);
			}
		}
	}


	/// <summary>
	/// Checks for user input.
	/// </summary>
	public void CheckForUserInput()
	{
		if (Input.GetMouseButtonDown(0)) {
			SendOutRaycastHit();
		}
	}
	/// <summary>
	/// Sends the out raycast hit. On hit this function will send the server a message
	/// containing its Grid information
	/// </summary>
	void SendOutRaycastHit()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit rc;

		if (Physics.Raycast(ray, out rc))
		{
			EnableCell ((int)rc.transform.position.x, (int)rc.transform.position.y);
			if(clientManager.IsClientAvailable())
			{
				string message = "<GRID_INFORMATION>" + "+" + GetGridInfo () + "-!";
				string message2 = "<LIGHT_INFORMATION_REQUEST>" + "!";

				clientManager.SendMessage (message);
				clientManager.SendMessage (message2);
				clientManager.GetMessage();
			}


		}
	}

	/// <summary>
	/// Changes a cell state from "alive" to "dead" or vice versa
	/// </summary>
	/// <param name="x">The x coordinate of the cell to update.</param>
	/// <param name="y">The y coordinate.</param>
	public void EnableCell(int x, int y)
	{
		foreach (Cell c in cells) {
			if (c.GetCellPosition ().x == x && c.GetCellPosition ().y == y) {

				if (c.isAlive) {
					c.isAlive = false;
				}
				else if (!c.isAlive) {
					c.isAlive = true;
				}
			}
		}
	}
	/// <summary>
	/// Changes the color of the cell.
	/// </summary>
	/// <param name="c">C.</param>
	/// <param name="cell_color">Cell color.</param>
	public void ChangeCellColor(Cell c, Color cell_color)
	{
		Renderer rend = c.GetCell().GetComponent<Renderer> ();

		//Set the main Color of the Material to green
		rend.material.shader = Shader.Find("_Color");
		rend.material.SetColor("_Color", cell_color);

		//Find the Specular shader and change its Color to red
		rend.material.shader = Shader.Find("Specular");
		rend.material.SetColor("_SpecColor", cell_color);
	}
		
	/// <summary>
	/// Prints Each cell located in the grid
	/// </summary>
	public void PrintGrid()
	{
		foreach(Cell c in cells)
		{
			print ("Cell Number = " + " " + c.GetCellID());
		}
	}
		
	/// <summary>
	/// Gets the grid info.
	/// </summary>
	/// <returns>The grid info.</returns>
	public string GetGridInfo()
	{
		string info = "";
		foreach (Cell c in cells) {
			if (c.GetCellState () == true) {
				info += "1";

			} else if (c.GetCellState () == false) {
				info += "0";

			}
		}
		return info;
	}
}

