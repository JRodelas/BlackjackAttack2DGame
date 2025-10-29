using UnityEngine;

public class DeckScript : MonoBehaviour
{
    
    public Sprite[] cardSprites;

    private int[] cardValues;    // aligned with cardSprites (A=1..10, J/Q/K=10)
    private int[] cardRanks;     // aligned with cardSprites (rank 1..13)
    private int   currentIndex = 1; // first face index
    private bool  isConfigured  = false;

    void Awake()
    {
        isConfigured = BuildValuesIfPossible();
        currentIndex = 1;
    }

    // check if spritess are assigned value
    bool BuildValuesIfPossible()
    {
        if (cardSprites == null)
        {
            Debug.LogError("Deck not configured: cardSprites is NULL. Assign sprites on the Deck object.");
            return false;
        }
        if (cardSprites.Length < 2)
        {
            Debug.LogError("Deck not configured: cardSprites needs at least 2 entries (back at [0], then faces).");
            return false;
        }

        cardValues = new int[cardSprites.Length];
        cardRanks  = new int[cardSprites.Length];

        cardValues[0] = 0; // back
        cardRanks[0]  = 0; // back

        // i=1..N-1: ranks 1-13; assign J/Q/K values to 10
        for (int i = 1; i < cardSprites.Length; i++)
        {
            int rank = ((i - 1) % 13) + 1; // 1..13
            cardRanks[i] = rank;

            int val = rank > 10 ? 10 : rank;
            cardValues[i] = val;
        }

        return true;
    }

    //fairly shuffle cards using fisher-yates
    public void Shuffle()
    {
        if (!isConfigured)
            isConfigured = BuildValuesIfPossible();
        if (!isConfigured)
        {
            Debug.LogWarning("DeckScript.Shuffle: Deck not configured; skipping shuffle.");
            return;
        }

        // Fisher-Yates over 1..N-1, skip the back card at [0]
        for (int i = cardSprites.Length - 1; i >= 2; i--)
        {
            int j = Random.Range(1, i + 1);
            (cardSprites[i], cardSprites[j]) = (cardSprites[j], cardSprites[i]);
            (cardValues[i],  cardValues[j])  = (cardValues[j],  cardValues[i]);
            (cardRanks[i],   cardRanks[j])   = (cardRanks[j],   cardRanks[i]);
        }

        currentIndex = 1;
    }
    //check if configuired
    public int RemainingCards => isConfigured ? (cardSprites.Length - currentIndex) : 0;

    //ensure fairness with shuffle after there are only <15 cards left
    public void EnsureShoe(int threshold = 15)
    {
        if (!isConfigured)
            isConfigured = BuildValuesIfPossible();
        if (!isConfigured) return;

        if (RemainingCards <= threshold)
        {
            Shuffle();
            currentIndex = 1;
        }
    }

    public Sprite GetCardBack()
    {
        if (!isConfigured)
            isConfigured = BuildValuesIfPossible();
        if (!isConfigured) return null;

        return cardSprites[0];
    }

    // Deals one card into CardScript; returns the card's VALUE
    //Double checks that cards are fair
    public int DealCard(CardScript receiver)
    {
        if (!isConfigured)
            isConfigured = BuildValuesIfPossible();
        if (!isConfigured)
        {
            Debug.LogWarning("DeckScript.DealCard: Deck not configured; cannot deal.");
            return 0;
        }

        if (currentIndex < 1 || currentIndex >= cardSprites.Length)
        {
            Debug.LogWarning("DeckScript.DealCard: Out of cards. Call EnsureShoe or Shuffle first.");
            return 0;
        }

        var sprite = cardSprites[currentIndex];
        var val    = cardValues[currentIndex];
        var rank   = cardRanks[currentIndex];

        receiver.SetSprite(sprite);
        receiver.SetValue(val);
        receiver.SetRank(rank);

        currentIndex++;
        return val;
    }
}

