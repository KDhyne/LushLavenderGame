using UnityEngine;
using System.Collections;

public class S_Beat : MonoBehaviour 
{
	public int i_measure;
	public float i_position;
	
	// Accepts parameters for which measure the beat is in and 
	//where in the measure (position) the beat is played
	public S_Beat(int measure, float position)
	{
		i_measure = measure;
		i_position = position;
	}
}
