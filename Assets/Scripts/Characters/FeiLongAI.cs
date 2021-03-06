﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeiLongAI : MonoBehaviour {
	
	[SerializeField] float decisionTimer, antiAirTimer;
	
	private Animator animator;
	private Player player;
	private Opponent opponent;
	private Character playerCharacter, opponentCharacter;
	private Character character;
	private SharedProperties sharedProperties;
	private AIControls AIcontrols;
	
	private int decision;
	private float decisionTimerInput, antiAirTimerInput;	
	
	// Use this for initialization
	void Start () {		
		
		if (GetComponentInParent<Opponent>() != null){
			player = FindObjectOfType<Player>();
			playerCharacter = player.GetComponentInChildren<Character>();
		}
		else if (GetComponentInParent<Player>() != null){
			opponent = FindObjectOfType<Opponent>();
			opponentCharacter = opponent.GetComponentInChildren<Character>();
		}
		
		character = GetComponent<Character>();
		animator = GetComponent<Animator>();	
		AIcontrols = GetComponentInParent<AIControls>();
		sharedProperties = GetComponentInParent<SharedProperties>();
		
		decisionTimerInput = decisionTimer; 
		antiAirTimerInput = antiAirTimer;
		antiAirTimer = 0f;
		decision = Random.Range(0,100);	
	}
		
	
	public void Behaviors(){
		decisionTimer--;
		antiAirTimer--;
		if (AIcontrols.FreeToMakeDecisions() && !TimeControl.inSuperStartup[0] && !TimeControl.inSuperStartup[1]){
			if (animator.GetBool("rekkaKenActive")){
				if (player != null){
					RekkaChainDecisions (playerCharacter);
				}
				else if (opponent != null){
					RekkaChainDecisions (opponentCharacter);
				}					
			}	
			else if (animator.GetBool("isAirborne") == true && animator.GetBool("isLiftingOff") == false){
				decision = Random.Range(0,100);	
				if (decision <= 1){
					AIcontrols.AIJumpFierce();
					sharedProperties.CharacterNeutralState();
				}
				else if (decision <= 6 && decision > 1){
					AIcontrols.AIJumpRoundhouse();
					sharedProperties.CharacterNeutralState();
				}				
			}
			else if (sharedProperties.GetAbDistanceFromOtherFighter() < 1f){
				if (player != null){
					if (playerCharacter.GetHitStunned() == true){
						CloseRangeOtherFighterGotHitDecisions ();
					}
					else if (playerCharacter.GetBlockStunned() == true){
						CloseRangeOtherFighterBlockedDecisions ();
					}
					else if (playerCharacter.GetKnockDown() == true && playerCharacter.GetAirborne() == false){					
						KnockDownCloseRangeDecisions ();
					}					
					else if (playerCharacter.GetAirborne() == true && playerCharacter.GetKnockDown() == false && playerCharacter.GetThrown() == false){
						if (antiAirTimer <= 0f){			
							sharedProperties.AIAntiAirDecision(48, RegularCloseRangeDecisions, PreparationForAntiAir);
							antiAirTimer = antiAirTimerInput;
						}
						else{
							RegularCloseRangeDecisions();
						}
					}
					else{
						RegularCloseRangeDecisions();
					}
				}
				else if (opponent != null){
					if (opponentCharacter.GetHitStunned() == true){
						CloseRangeOtherFighterGotHitDecisions ();
					}
					else if (opponentCharacter.GetBlockStunned() == true){
						CloseRangeOtherFighterBlockedDecisions ();
					}
					else if (opponentCharacter.GetKnockDown() == true && opponentCharacter.GetAirborne() == false && opponentCharacter.GetThrown() == false){					
						KnockDownCloseRangeDecisions ();
					}	
					else if (opponentCharacter.GetAirborne() == true && opponentCharacter.GetKnockDown() == false){
						if (antiAirTimer <= 0f){			
							sharedProperties.AIAntiAirDecision(48, RegularCloseRangeDecisions, PreparationForAntiAir);
							antiAirTimer = antiAirTimerInput;
						}
						else{
							RegularCloseRangeDecisions();
						}
					}
					else{
						RegularCloseRangeDecisions();
					}
				}
			}	
			else if (sharedProperties.GetAbDistanceFromOtherFighter() < 2f && sharedProperties.GetAbDistanceFromOtherFighter() >= 1f){				
				if (player != null){
					if (playerCharacter.GetKnockDown() == true){				
						KnockDownMidRangeDecisions ();
					}
					else {						
						RegularMidRangeDecisions ();
					}	
				}
				else if (opponent != null){			
					if (opponentCharacter.GetKnockDown() == true){				
						KnockDownMidRangeDecisions ();
					}
					else {						
						RegularMidRangeDecisions ();
					}
				}
			}
			else{
				RegularFarRangeDecisions ();
			}
			AIcontrols.AIWalks();
		}			
	}

	void RegularFarRangeDecisions (){
		DecisionMade (5, 1);
		if (decision <= 30) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 45 && decision > 30) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 50 && decision > 45) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 85 && decision > 50) {
			AIRekkaKuns ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 90 && decision > 85) {
			AIcontrols.AIStand ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}

	void RegularMidRangeDecisions (){
		DecisionMade (5, 2);
		if (decision <= 30) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 34 && decision > 30) {
			AIcontrols.AIFierce (2, 0);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 37 && decision > 34) {
			AIcontrols.AIStrong (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 39 && decision > 37) {
			AIcontrols.AIJab (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 40 && decision > 39) {
			AIcontrols.AIShort (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 50 && decision > 40) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 55 && decision > 50) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 65 && decision > 55 && !animator.GetBool("isAttacking")) {
			AIRekkaKens ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 75 && decision > 65 && !animator.GetBool("isAttacking")) {
			AIRekkaKuns ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 80 && decision > 75) {
			AIcontrols.AIStand ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}

	void RegularCloseRangeDecisions (){
		DecisionMade (5, 4);
		if (decision <= 10) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			character.SetBackPressed (false);
		}
		else if (decision <= 13 && decision > 10) {
			AIcontrols.AIFierce (10, 8);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 16 && decision > 13) {
			AIcontrols.AIStrong (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 22 && decision > 16) {
			if (character.GetSuper >= 100f){
				AIRekkaShinkens();
			}
			else{
				AIcontrols.AIJab (8);
			}
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 23 && decision > 22) {
			AIcontrols.AIShort (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 25 && decision > 23) {
			AIcontrols.AIForward (2);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 30 && decision > 25) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 32 && decision > 30) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 35 && decision > 32) {
			AIcontrols.AISweep ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 40 && decision > 35) {
			AIcontrols.AIThrow ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 45 && decision > 40 && !animator.GetBool("isAttacking")) {
			AIRekkaKens ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 48 && decision > 45 && !animator.GetBool("isAttacking")) {
			AIShortShienKyakus ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else if (decision <= 50 && decision > 48 && !animator.GetBool("isAttacking")) {
			AIRekkaKuns ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else {
			AIcontrols.AICrouch ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}

	void KnockDownMidRangeDecisions (){
		DecisionMade (5, 3);
		if (decision <= 60) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 75 && decision > 60) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 90 && decision > 75) {
			AIRekkaKuns ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
		else {
			AIRekkaKens ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
			decisionTimer = 0f;
		}
	}

	void KnockDownCloseRangeDecisions (){
		decision = Random.Range(0,100); 
		if (decision <= 60) {
			AIcontrols.AIStand ();
			AIcontrols.AIPressedForward ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 63 && decision > 60) {
			AIcontrols.AIPressedForward ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 75 && decision > 63) {
			sharedProperties.CharacterNeutralState ();
			AIcontrols.AIJump ();
			character.SetBackPressed (false);
		}
		else if (decision <= 90 && decision > 75) {
			AIRekkaKuns ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else {
			AIRekkaKens ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}

	void CloseRangeOtherFighterBlockedDecisions (){
		decision = Random.Range(0,100);
        if (decision <= 30)
        {
            AIcontrols.AIJab(8);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 50)
        {
            AIcontrols.AIStrong(2);
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 60)
        {
            if (character.GetSuper >= 100f)
            {
                AIRekkaShinkens();
            }
            else
            {
                AIcontrols.AIFierce(10, 8);
            }
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 65)
        {
            AIRekkaKens();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 68)
        {
            AIShortShienKyakus();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else if (decision <= 73)
        {
            AIRekkaKuns();
            sharedProperties.CharacterNeutralState();
            AIcontrols.DoesAIBlock();
        }
        else
        {
            AIcontrols.AICrouch();
            sharedProperties.CharacterNeutralState();
            character.SetBackPressed(true);
        }
	}

	void CloseRangeOtherFighterGotHitDecisions (){
		decision = Random.Range(0,100); 
		if (decision <= 60) {
			AIRekkaKens ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 70 && decision > 60) {
		    AIcontrols.AIFierce (10, 8);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else {
            if (character.GetSuper >= 100f)
            {
                AIRekkaShinkens();
            }
            else
            {
                AIShienKyakus();
            }
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}
	
	void PreparationForAntiAir (){
		decision = Random.Range (0, 100);
		if (decision <= 70) {
			AIShienKyakus ();
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();		
		}
		else if (decision <= 80 && decision > 70) {
			AIcontrols.AIFierce (10, 8);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else if (decision <= 90 && decision > 80){	
			AIcontrols.AIJab (8);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
		else{
			AIcontrols.AIForward (1);
			sharedProperties.CharacterNeutralState ();
			AIcontrols.DoesAIBlock ();
		}
	}

	void RekkaChainDecisions (Character whichFighter){
		decision = Random.Range(0,200);
		if (whichFighter.GetHitStunned () == true) {
			if (decision <= 40) {
				AIRekkaKens ();
				sharedProperties.CharacterNeutralState ();
				AIcontrols.DoesAIBlock ();
			}
		}
		else {
			if (decision <= 2) {
				AIRekkaKens ();
				sharedProperties.CharacterNeutralState ();
				AIcontrols.DoesAIBlock ();
			}
		}
	}
	
	void DecisionMade (int minDivisor, int maxDivisor){
		if (decisionTimer <= 0) {
			decision = Random.Range (0, 100);
			decisionTimer = Random.Range (decisionTimerInput / minDivisor, decisionTimerInput / maxDivisor);
		}
	}
	
	void AIRekkaShinkens(){
		if (AIcontrols.GetConditionsSpecialAttack()){			
			animator.SetTrigger("motionSuperInputed");		
			if (animator.GetBool("isAttacking") == false){
				AIcontrols.AIStand ();
				character.AttackState();
				animator.Play("FeiLongRekkaShinken",0);					
			}
		}
	}
	
	void AIRekkaKens(){
		int rekkaPunch = Random.Range(0,3);
		if (AIcontrols.GetConditionsSpecialAttack()){
			
			animator.SetTrigger("hadoukenInputed");			
			if (animator.GetBool("isAttacking") == false){
				AIcontrols.AIStand();
				character.AttackState();
				animator.Play("FeiLongRekkaKenFirstAttack",0);
				if (rekkaPunch == 0){
					animator.SetInteger("rekkaPunchType", 0);
				}
				else if (rekkaPunch == 1){
					animator.SetInteger("rekkaPunchType", 1);
				}
				else{				 
					animator.SetInteger("rekkaPunchType", 2);
				}
			}
		}
	}
	
	void AIShortShienKyakus(){
		if (AIcontrols.GetConditionsSpecialAttack()){
			
			animator.SetTrigger("reverseShoryukenInputed");			
			if (animator.GetBool("isAttacking") == false){
				AIcontrols.AIStand();
				character.AttackState();
				animator.Play("FeiLongShienKyakuShort",0);
				animator.SetInteger("shienKyakuKickType", 0);
			}
		}
	}
		
	void AIRekkaKuns(){
		int rekkaKunKick = Random.Range(0,3);
		if (AIcontrols.GetConditionsSpecialAttack()){
			
			animator.SetTrigger("shoryukenInputed");			
			if (animator.GetBool("isAttacking") == false){
				AIcontrols.AIStand();
				character.AttackState();
				animator.Play("FeiLongRekkaKun",0);
				if (rekkaKunKick == 0){
					animator.SetInteger("rekkaKunKickType", 0);
				}
				else if (rekkaKunKick == 1){
					animator.SetInteger("rekkaKunKickType", 1);
				}
				else{				 
					animator.SetInteger("rekkaKunKickType", 2);
				}
			}
		}
	}
	
	void AIShienKyakus(){
		int shienKyakuKick = Random.Range(0,3);
		if (AIcontrols.GetConditionsSpecialAttack()){
			
			animator.SetTrigger("reverseShoryukenInputed");			
			if (animator.GetBool("isAttacking") == false){
				AIcontrols.AIStand();
				character.AttackState();
				if (shienKyakuKick == 0){
					animator.Play("FeiLongShienKyakuShort",0);
					animator.SetInteger("shienKyakuKickType", 0);
				}
				else if (shienKyakuKick == 1){
					animator.Play("FeiLongShienKyakuForward",0);
					animator.SetInteger("shienKyakuKickType", 1);
				}
				else{				 
					animator.Play("FeiLongShienKyakuRoundhouse",0);
					animator.SetInteger("shienKyakuKickType", 2);
				}
			}
		}
	}
}
