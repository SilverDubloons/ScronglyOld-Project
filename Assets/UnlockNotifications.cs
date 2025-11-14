using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockNotifications : MonoBehaviour
{
	public static UnlockNotifications instance;
	public GameObject unlockNotifierPrefab;
	public Transform unlockNotifierParent;
	
	public void CreateNewUnlockNotifier(int type, int num) // type 0 = deck, 1 = bauble | num = relative deck/bauble/etc
	{
		GameObject newNotifier = Instantiate(unlockNotifierPrefab, new Vector3(0,0,0), Quaternion.identity, unlockNotifierParent);
		UnlockNotifier unlockNotifier = newNotifier.GetComponent<UnlockNotifier>();
		unlockNotifier.rt.anchoredPosition = new Vector2(42,0);
		unlockNotifier.SetupUnlockNotifier(type, num);
	}
	
	void Awake()
	{
		instance = this;
	}
}
