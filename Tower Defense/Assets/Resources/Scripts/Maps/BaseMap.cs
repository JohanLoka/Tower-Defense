using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMap : MonoBehaviour {

	//  Load Components needed for GameManager
	void Start () {
		GameManager.Instance.EnemyTargetPoint = transform.Find("WayPoints").Find("EnemyEndTarget");
	}
}
