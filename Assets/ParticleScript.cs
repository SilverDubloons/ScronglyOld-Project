using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    public RectTransform rt;
	public float timeUntilDeath;
	public float timeAlive;
	public float speed;
	public Vector2 direction;
	public AnimationCurve speedOverTime;
	
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(timeAlive < timeUntilDeath)
		{
			timeAlive += Time.deltaTime;
			float adjustedSpeed = speedOverTime.Evaluate(timeAlive / timeUntilDeath) * speed;
			rt.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timeAlive/timeUntilDeath);
			rt.anchoredPosition = rt.anchoredPosition + direction * speed;
		}
		else
		{
			Destroy(this.gameObject);
		}
    }
}
