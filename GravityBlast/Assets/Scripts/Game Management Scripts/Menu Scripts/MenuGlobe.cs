using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuGlobe : MonoBehaviour
{
	private float globeWidth;
	private GameObject currentSelection;
	
    void Start()
    {
        globeWidth = GetComponent<RectTransform>().sizeDelta.x;
		/*
		currentSelection = EventSystem.current.currentSelectedGameObject;
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(currentSelection);
		
		//EnsureSelectedElement();
		
		UpdateSelection();
		*/
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateSelection();
    }
	/*
	public void UpdateSelection()
	{
		GameObject newSelection = EventSystem.current.currentSelectedGameObject;
		if (newSelection != null && currentSelection != newSelection)
		{
			MoveToNewChoice(newSelection);
		}
	}
	*/
	
	public void MoveToNewChoice(GameObject selection)
	{
		//int numOfChar = selection.GetComponentInChildren<TextMeshProUGUI>().text.Length;
		//float offset = 30.0f;
		//transform.position = selection.transform.position + (offset * numOfChar * Vector3.left);
		
		//Vector2 textSize = selection.GetComponentInChildren<TextMeshProUGUI>().GetRenderedValues(true);
		//transform.position = selection.transform.position + (textSize.x * Vector3.left);
		
		//float textWidth = selection.GetComponentInChildren<TextMeshProUGUI>().preferredWidth;
		
		TextMeshProUGUI selectionText = selection.GetComponentInChildren<TextMeshProUGUI>();
		selectionText.ForceMeshUpdate();
		float textWidth = selectionText.preferredWidth;
		//float extraPadding = Screen.height * 0.004f;
		
		float extraPadding = 15f;
		
		
		
		transform.position = selection.transform.position + (((globeWidth + (textWidth / 2.0f) + extraPadding) * Vector3.left) / 2.0f);
    }
	
	/*
	void EnsureSelectedElement()
	{
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			var firstSelectable = menuObject.GetComponentInChildren<Selectable>();
			if (firstSelectable != null)
            EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
		}
	}
	*/
}
