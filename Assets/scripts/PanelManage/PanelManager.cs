using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour {
	public RectTransform welcomePanel;

	public void OnClickEnterGame(){
		welcomePanel.gameObject.SetActive (false);
	}
}
