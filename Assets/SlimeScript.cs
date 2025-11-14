using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeScript : MonoBehaviour
{
    private RectTransform rt;
	public float speed;
	public bool spawnedNext = false;
	public ScoreVial scoreVial;
    void Start()
    {
        rt = this.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + Time.deltaTime * speed, rt.anchoredPosition.y);
		if(!spawnedNext && rt.anchoredPosition.x > -2)
		{
			scoreVial.SpawnNextSlime(rt, speed);
			spawnedNext = true;
		}
		if(rt.anchoredPosition.x > 15)
		{
			Destroy(this.gameObject);
		}
    }
}
