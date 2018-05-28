﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedProperties : MonoBehaviour {

	//as the name implies, this is the script that controls properties, methods, and any items that both players share
	
	public delegate void RegularDecisions();
	public delegate void AIAntiAirs();
	
	private Player player;
	private Opponent opponent;
	private Animator animator;
	private Character character;
	private TimeControl timeControl;
	private bool isKOSoundPlayed, pressedForward, pressedBackward, pressedCrouch; 	
	
	void Start(){
		timeControl = FindObjectOfType<TimeControl>();
		
		if (GetComponent<Player>() != null){
			player = GetComponent<Player>();
		}
		else if (GetComponent<Opponent>() != null){
			opponent = GetComponent<Opponent>();
		}
		animator = GetComponentInChildren<Animator>();
		character = GetComponentInChildren<Character>();
		isKOSoundPlayed = false;
	}
	
	public void KOSequence (string winnerString){
		if (timeControl.gameState == TimeControl.GameState.KOHappened){
			if (animator.GetBool ("isKOed") == true && !isKOSoundPlayed) {				
				character.KOSound ();
				isKOSoundPlayed = true;
			}
		}
		if (timeControl.gameState == TimeControl.GameState.victoryPose && animator.GetBool ("isKOed") == false) {
			TimeControl.winner = winnerString;
			animator.Play ("VictoryPose");
		}
	}
	

	public void CharacterNeutralState(){
		GetForwardPressed = false;
		GetBackPressed = false;
	}
	
	public void IsThrown(Animator thrownAnim, Character throwingCharacter, Character thrownCharacter){
		if (thrownAnim.GetBool("isThrown") == true){
			if (throwingCharacter.GetComponent<Ken>() != null){
				ThrowSnapPoint (throwingCharacter, thrownCharacter, 0.25f);		
			}
			else if (throwingCharacter.GetComponent<FeiLong>() != null){
				ThrowSnapPoint (throwingCharacter, thrownCharacter, 0.5f);	
			}		
			else if (throwingCharacter.GetComponent<Balrog>() != null){
				ThrowSnapPoint (throwingCharacter, thrownCharacter, 0.3f);	
			}		
			else if (throwingCharacter.GetComponent<Akuma>() != null){
				ThrowSnapPoint (throwingCharacter, thrownCharacter, 0.55f);
			}
            else if (throwingCharacter.GetComponent<Sagat>() != null){
				ThrowSnapPoint (throwingCharacter, thrownCharacter, 0.4f);
			}
		}
	}	
	
	public void AIAntiAirDecision (int frequency, RegularDecisions regularDecisions, AIAntiAirs antiAir){		
		int decision = Random.Range (0, 100);
		bool doesAIAntiAir = DoesAIAntiAir(decision, frequency);		
		if (doesAIAntiAir){
			Debug.Log (gameObject.name + " anti aired");
			antiAir  ();
		}
		else{
			Debug.Log (gameObject.name + " didnt anti air");
			regularDecisions ();
		}
	}	

	void ThrowSnapPoint (Character throwingCharacter, Character thrownCharacter, float snapPoint){
		if (thrownCharacter.side == Character.Side.P2) {
			thrownCharacter.transform.position = new Vector3 (throwingCharacter.transform.position.x + snapPoint, throwingCharacter.transform.position.y, 0f);
		}
		else {
			thrownCharacter.transform.position = new Vector3 (throwingCharacter.transform.position.x - snapPoint, throwingCharacter.transform.position.y, 0f);
		}
	}
	
	bool DoesAIAntiAir(int randNum, int frequency){			
		if (randNum <= frequency) {
			return true;
		}
		else {
			return false;
		}
	}

	public bool GetBackPressed{
		get { return pressedBackward; }
		set { pressedBackward = value; }
	}	
	
	public bool GetDownPressed{   
		get { return pressedCrouch; }
		set { pressedCrouch = value; }
	}	
	
	public bool GetForwardPressed{
		get { return pressedForward; }
		set { pressedForward = value; }
	}	
	
	public float GetDistanceFromOtherFighter(){
		if (opponent != null){
			return Mathf.Abs(opponent.GetDistance());
		}
		else {
			return Mathf.Abs(player.GetDistance());
		}
	}		
}
