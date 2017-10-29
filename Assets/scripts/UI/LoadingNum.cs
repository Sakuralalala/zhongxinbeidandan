using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingNum : MonoBehaviour {
    private Text loadingNum;
	public float delta = 0.03f;

	void OnEnable(){
		loadingNum = GetComponentInChildren<Text> ();        
	}

	public void countFromAndTo(int start,int to){
		if (start < to) {
			StartCoroutine (count(start,to));
		}
	}

	private IEnumerator count(int start,int end){
		SetNumber (start);
		float current = start;
		yield return 0;
		while(current<=end){
			Debug.Log ("a frame");
			current += delta;
			SetNumber ((int)current);
			yield return 0;
		}
	}

	private void SetNumber(int number){
		if(number>=0&&number<=100)
			loadingNum.text = "" + number+"%";
	}
}
