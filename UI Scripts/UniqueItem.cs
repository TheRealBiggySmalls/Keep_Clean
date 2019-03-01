using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UniqueItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

	public Texture texture; private GameObject tooltip;
	public string uniqueName; public string displayName;
	public string description;

	public bool activated;

	public void SetUniqueItemParams(string nam, string disNam, string desc, Texture text, GameObject toolti){
		uniqueName = nam;
		description = desc;
		displayName = disNam;
		texture=text;
		activated=false;
		tooltip=toolti;
	}

	public void OnPointerEnter(PointerEventData eventData){
		if(activated){
			showToolTip();
		}
	}

	public void OnPointerExit(PointerEventData eventData){
		if(activated){
			hideToolTip();
		}
	}

	public void hideToolTip(){
		tooltip.SetActive(false);
	}

	public void showToolTip(){
		tooltip.SetActive(true);
		Text[] fields = tooltip.GetComponentsInChildren<Text>();
		fields[0].text = displayName;
		fields[1].text = description;
	}

}
