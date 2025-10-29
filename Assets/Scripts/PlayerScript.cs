﻿using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    // Card 1/2, Hand 1/2/3/4
    public GameObject[] hand;

    public int handValue = 0;
    public int cardIndex = 0;

    // Ace handling - makes them dynamic
    private int softAcesAs11 = 0;

    // Health System
    public int maxHP = 100;
    public int currentHP = 100;
    public bool IsDead => currentHP <= 0;
    [HideInInspector] public bool lastHandHasPair = false;

    // Health helper
    public void ResetHealth(int hp)
    {
        maxHP = hp;
        currentHP = hp;
    }

    public int TakeDamage(int dmg)
    {
        // IMMUNITY: if you ended the hand with a pair, you take NO damage once
        if (lastHandHasPair)
        {
            lastHandHasPair = false;   
            return 0;                  // there is no HP change
        }

        currentHP = Mathf.Max(0, currentHP - Mathf.Max(0, dmg));
        return dmg;
    }

    // hand logicc
    public void ResetHand()
    {
        handValue = 0;
        cardIndex = 0;
        softAcesAs11 = 0;

        foreach (var slot in hand)
        {
            if (!slot) continue;
            var card = slot.GetComponent<CardScript>();
            if (card) card.Hide();
        }
    }

    public void StartHand()
    {
        GetCard(); // first
        GetCard(); // second
    }

    public int GetCard()
    {
        if (cardIndex >= hand.Length)
        {
            Debug.LogWarning("No more hand slots.");
            return handValue;
        }

        var deck = GameObject.Find("Deck").GetComponent<DeckScript>();
        var cardComp = hand[cardIndex].GetComponent<CardScript>();
        int value = deck.DealCard(cardComp);
        hand[cardIndex].GetComponent<Renderer>().enabled = true;

        // Ace handling: treat as 11 if it fits, else 1.
        if (value == 1)
        {
            int as11 = handValue + 11;
            if (as11 <= 21)
            {
                handValue = as11;
                softAcesAs11++;
            }
            else
            {
                handValue += 1;
            }
        }
        else
        {
            handValue += value;

            // If player busts but has a soft Ace counted as 11, change it to 1
            if (handValue > 21 && softAcesAs11 > 0)
            {
                handValue -= 10; // 11 -> 1
                softAcesAs11--;
            }
        }
        cardIndex++;
        return handValue;
    }

    public bool HandHasPair()
    {
        var seen = new HashSet<int>(); // card ranks
        for (int i = 0; i < cardIndex; i++)
        {
            var c = hand[i].GetComponent<CardScript>();
            if (!c) continue;
            int r = c.GetRank();       // use rank, give values to Q, K, J, and 10
            if (r <= 0) continue;      
            if (seen.Contains(r)) return true;
            seen.Add(r);
        }
        return false;
    }
    public bool HasPairRightNow() => HandHasPair();
}

