using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public RectTransform backgroundRT;
	public float rotationSpeed;
	
	void Update()
	{
		if(GameOptions.instance.rotatingBackground)
		{
			backgroundRT.Rotate(0,0, rotationSpeed * Time.deltaTime);
		}
	}
}
