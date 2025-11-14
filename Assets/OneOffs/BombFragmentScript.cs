using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombFragmentScript : MonoBehaviour
{
    public RectTransform rt;
	public float timeAlive;
/* 	public float randomAngle;
	public float speed;
	public float sinSpeed;
	public float sinAdjust;*/
	public bool leftSide;
	public BombExplosionScript bombExplosionScript;
	public bool alreadyReportedDeath = false;
	
	float xStart;
	float yStart;
	float xSpeed;
	float ySpeed;
	
    void Start()
    {
		/* randomAngle = Random.Range(0f,360f);
		speed = Random.Range(200f, 550f);
		sinSpeed = Random.Range(3f, 5f);
		sinAdjust = Random.Range(80f, 160f); */
        //StartCoroutine(Animate());
		
		xStart = Random.Range(7f, 13f);
		yStart = Random.Range(10f, 15f);
		xSpeed = Random.Range(20f, 40f);
		ySpeed = Random.Range(35f, 65f); 
    }
	
/* 	public IEnumerator Animate()
	{
		
	} */

    void Update()
    {
		/* timeAlive += Time.deltaTime;
        float sinWave = Mathf.Sin(timeAlive * sinSpeed + randomAngle);
		float xChange = sinWave * sinAdjust * Time.deltaTime;
		float ySpeed = (0.9f - Mathf.Abs(sinWave))/2 * -speed;
		rt.localRotation = Quaternion.Euler(new Vector3(rt.localRotation.x, rt.localRotation.y, sinWave*Time.deltaTime*1000));
		rt.anchoredPosition += new Vector2(xChange, ySpeed * Time.deltaTime); */
		
		timeAlive += Time.deltaTime;
		float xChange;
		if(leftSide)
		{
			xChange = (xStart - timeAlive) * Time.deltaTime * -xSpeed;
		}
		else
		{
			xChange = (xStart - timeAlive) * Time.deltaTime * xSpeed;
		}
		float yChange = (3 - yStart * timeAlive) * Time.deltaTime * ySpeed;
		rt.anchoredPosition += new Vector2(xChange, yChange);
		if(rt.anchoredPosition.y < -300 && !alreadyReportedDeath)
		{
			bombExplosionScript.FragmentDied();
			alreadyReportedDeath = true;
			Destroy(this.gameObject);
		}
    }
}
