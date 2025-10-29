using UnityEngine;

public class CardScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite frontSprite;
    [SerializeField] private int value;
    [SerializeField] private int rank; // card values

    // Set by Deck when dealt
    public void SetSprite(Sprite s)
    {
        frontSprite = s;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = s;
        spriteRenderer.enabled = true;
    }

    // setter and getter for value
    public void SetValue(int v) => value = v;
    public int GetValueOfCard() => value;

    // back of card
    public void ShowBack(Sprite back)
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = back;
        spriteRenderer.enabled = true;
    }
    // do not show card
    public void Hide()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    public void SetRank(int r) => rank = r;     // give value to all cards -> J,K,Q != 10
    public int  GetRank()   => rank;
}
