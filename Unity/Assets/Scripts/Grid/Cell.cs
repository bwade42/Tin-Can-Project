using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Cell Class used to represent each cube object in scene
/// </summary>
public class Cell : MonoBehaviour
{
	private GameObject cell; //object reference
	private Vector3 position; // position of the cell
	private int cellID; // cell identifier used for debugging
	public bool isAlive; // set to true when a cell is considered "alive"

	/// <summary>
	/// Initializes a new instance of the <see cref="Cell"/> class.
	/// </summary>
	/// <param name="cube">Cube.</param>
	/// <param name="id">Identifier.</param>
	public Cell(int id,Vector3 position)
	{

		SetCellID (id);

		this.position = position;
		isAlive = false;

		cell = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		cell.tag = "cell";
		cell.name = "Cell " + id;

		cell.transform.localPosition = position;
		cell.transform.localScale = new Vector3 (1f, 0.05f, 1f);

		cell.transform.parent = GameObject.FindGameObjectWithTag("grid map").transform;
	
	}
		
	/// <summary>
	/// Gets the cel object reference
	/// </summary>
	/// <returns>The cel objectl.</returns>
	public GameObject GetCell()
	{
		return cell;
	}

	/// <summary>
	/// Gets the cell position.
	/// </summary>
	/// <returns>The cell position.</returns>
	public Vector3 GetCellPosition()
	{
		return cell.transform.position;
	}

	/// <summary>
	/// Gets the cell ID
	/// </summary>
	/// <returns>The cell I.</returns>
	public int GetCellID()
	{
		return cellID;
	}

	/// <summary>
	/// Gets the state of the cell.
	/// </summary>
	/// <returns><c>true</c>, if cell state was gotten, <c>false</c> otherwise.</returns>
	public bool GetCellState()
	{
		return isAlive;
	}
		
	/// <summary>
	/// Set the Cell ID
	/// </summary>
	/// <param name="id">Identifier.</param>
	public void SetCellID(int id)
	{
		cellID = id;
	}
		
	//This method is required by the IComparable
	//interface. 
	public int CompareTo(Cell other)
	{
		if(other == null)
		{
			return 1;
		}

		//Return the difference in ids.
		return cellID - other.cellID;
	}
}

