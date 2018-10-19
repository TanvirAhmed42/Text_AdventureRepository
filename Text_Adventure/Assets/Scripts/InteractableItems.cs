using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItems : MonoBehaviour {

	public List<InteractableObject> useableItemList;

	public Dictionary<string, string> examineDictionary = new Dictionary<string, string> ();
	public Dictionary<string, string> takeDictionary = new Dictionary<string, string> ();

	[HideInInspector]
	public List<string> nounsInRoom = new List<string> ();

	GameController controller;

	Dictionary<string, ActionResponse> useDictionary = new Dictionary<string, ActionResponse> ();
	List<string> nounsInInventory = new List<string> ();

	void Awake () {
		controller = GetComponent<GameController> ();
	}

	public string GetObjectsNotInInventory (Room currentRoom, int i) {
		InteractableObject interactableInRoom = currentRoom.interactableObjectsInRoom [i];

		if (!nounsInInventory.Contains (interactableInRoom.noun)) {
			nounsInRoom.Add (interactableInRoom.noun);
			return interactableInRoom.descriptioon;
		}

		return null;
	}

	public void GetActionResponsesInUseDictionary () {
		for (int i = 0; i < nounsInRoom.Count; i++) {
			string noun = nounsInRoom [i];

			InteractableObject interactableObjectInInventory = GetInteractableObjectFromUseableList (noun);

			if (interactableObjectInInventory == null)
				continue;
			
			for (int j = 0; j < interactableObjectInInventory.interactions.Length; j++) {
				Interaction interaction = interactableObjectInInventory.interactions [j];

				if (interaction.actionResponse == null)
					continue;

				if (!useDictionary.ContainsKey (noun)) {
					useDictionary.Add (noun, interaction.actionResponse);
				}
			}
		}
	}

	InteractableObject GetInteractableObjectFromUseableList (string noun) {
		for (int i = 0; i < useableItemList.Count; i++) {
			if (useableItemList [i].noun == noun) {
				return useableItemList [i];
			}
		}

		return null;
	}

	public void DisplayInventory () {
		controller.LogStringWithReturn ("You look in your inventory. Inside, you have: ");

		for (int i = 0; i < nounsInInventory.Count; i++) {
			controller.LogStringWithReturn (nounsInInventory [i]);
		}
	}
			

	public void ClearCollections () {
		examineDictionary.Clear ();
		takeDictionary.Clear ();
		nounsInRoom.Clear ();
	}

	public Dictionary<string, string> Take (string[] seperatedInputWords) {
		string noun = seperatedInputWords [1];

		if (nounsInRoom.Contains (noun)) {
			nounsInInventory.Add (noun);
			GetActionResponsesInUseDictionary ();
			nounsInRoom.Remove (noun);
			return takeDictionary;
		} else {
			controller.LogStringWithReturn ("There is no " + noun + " here to take.");
			return null;
		}
	}

	public void UseItem (string[] seperatedInputWords) {
		string nounToUse = seperatedInputWords [1];

		if (nounsInInventory.Contains (nounToUse)) {
			if (useDictionary.ContainsKey (nounToUse)) {
				bool actionResult = useDictionary [nounToUse].DoActionResponse (controller);

				if (!actionResult) {
					controller.LogStringWithReturn ("Hmm, nothing happens");
				}
			} else {
				controller.LogStringWithReturn ("You can't use the " + nounToUse);
			}
		} else {
			controller.LogStringWithReturn ("There is no " + nounToUse + " in your inventory.");
		}
	}
}
