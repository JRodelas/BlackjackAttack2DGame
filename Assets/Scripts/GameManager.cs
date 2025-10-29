using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Battle fx
    public BattleAnimator battleFX;    
    // Main buttons
    public Button dealBtn;
    public Button hitBtn;
    public Button standBtn;
    public Button betBtn;
    public Button restartBtn;

    // Players
    public PlayerScript playerScript;
    public PlayerScript dealerScript;

    // HUD
    public TMP_Text scoreText;
    public TMP_Text dealerScoreText;
    public TMP_Text mainText;
    public TMP_Text standBtnText;
    
    // HP Options
    public TMP_Text selectedHPText;
    public Button HP100Btn;
    public Button HP150Btn;
    public Button HP200Btn;
    private int selectedHP = 100;

    // How to play screen
    public GameObject howToPlayPanel;
    public Button howToPlayBtn;
    public Button backButton;

    // HP UI
    public TMP_Text playerHPText;
    public TMP_Text dealerHPText;
    public Image playerHPBar;
    public Image dealerHPBar;

    // Audio
    public AudioSource sfx;
    public AudioClip uiClick;
    public AudioClip playerDamage;
    public AudioClip dealerDamage;
    public AudioClip shieldBlock;   //immunity block

    void PlayOne(AudioClip clip, float vol = 1f)
    {
        if (sfx && clip) sfx.PlayOneShot(clip, vol);
    }
    void WireClick(Button b) { if (b) b.onClick.AddListener(() => PlayOne(uiClick, 0.6f)); }

    // status of each move
    public TMP_Text playerStatusText;
    public TMP_Text dealerStatusText;
    public TMP_Text dealerNarrationText;
    public Image mainTextBackground;
    public GameObject hideCard;
    public GameObject startScreenPanel;
    public Button playGameBtn;

    // Keep track of win streaks and wins
    private int standClicks = 0;
    private int playerWinStreak = 0;
    private int dealerWinStreak = 0;
    private string lastPlayerEvent = "—";
    private string lastDealerEvent = "—";

    void UpdateSelectedHPLabel()
    {
        if (selectedHPText) selectedHPText.text = $"Starting HP: {selectedHP}";
    }

    void Start()
    {
        if (dealBtn)  dealBtn.onClick.AddListener(DealClicked);
        if (hitBtn)   hitBtn.onClick.AddListener(HitClicked);
        if (standBtn) standBtn.onClick.AddListener(StandClicked);

        // Click sounds
        WireClick(dealBtn);
        WireClick(hitBtn);
        WireClick(standBtn);
        WireClick(restartBtn);
        WireClick(playGameBtn);
        WireClick(howToPlayBtn);
        WireClick(backButton);
        WireClick(HP100Btn);
        WireClick(HP150Btn);
        WireClick(HP200Btn);

        // restart button clicked
        if (restartBtn)
        {
            restartBtn.gameObject.SetActive(false);
            restartBtn.onClick.AddListener(RestartGame);
        }

        // play button 
        if (playGameBtn) playGameBtn.onClick.AddListener(PlayGameClicked);

        // HP select
        if (HP100Btn) HP100Btn.onClick.AddListener(() => SelectHP(100));
        if (HP150Btn) HP150Btn.onClick.AddListener(() => SelectHP(150));
        if (HP200Btn) HP200Btn.onClick.AddListener(() => SelectHP(200));

        //Handles HP text update
        UpdateHPUI();
        UpdateSelectedHPLabel();         
        ShowStartText("Select HP, then press PLAY");

        playerScript.ResetHealth(100);
        dealerScript.ResetHealth(100);
        UpdateHPUI();

        ShowStartText("Select HP, then press PLAY");
        if (dealerNarrationText) dealerNarrationText.text = "";

        dealBtn.gameObject.SetActive(true);
        hitBtn.gameObject.SetActive(false);
        standBtn.gameObject.SetActive(false);

        if (startScreenPanel)
        {
            startScreenPanel.SetActive(true);
            dealBtn.gameObject.SetActive(false);
            startScreenPanel.SetActive(startScreenPanel.activeSelf);
        }

        if (howToPlayBtn)
            howToPlayBtn.onClick.AddListener(OpenHowToPlay);
        if (backButton)
            backButton.onClick.AddListener(CloseHowToPlay);

        UpdateStatusUI();
    }

    // open and close how to play
    void OpenHowToPlay()
    {
        if (howToPlayPanel) howToPlayPanel.SetActive(true);
        if (startScreenPanel) startScreenPanel.SetActive(false);
    }

    void CloseHowToPlay()
    {
        if (howToPlayPanel) howToPlayPanel.SetActive(false);
        if (startScreenPanel) startScreenPanel.SetActive(true);
    }

    //  play game button clicked
    void PlayGameClicked()
    {
        if (startScreenPanel) startScreenPanel.SetActive(false);
        if (howToPlayPanel) howToPlayPanel.SetActive(false);

        // ensure we actually use the chosen value
        playerScript.ResetHealth(selectedHP);
        dealerScript.ResetHealth(100);

        UpdateHPUI();
        ShowStartText("Press DEAL to start");
        dealBtn.gameObject.SetActive(true);
    }

    //dynamicaly chooseHP value
    void SelectHP(int hp)
    {
        selectedHP = hp;
        UpdateSelectedHPLabel();
        ShowStartText($"Selected your {hp} HP");
        playerScript.ResetHealth(hp);
        UpdateHPUI();
    }

    // restart scene
    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // update status texts
    void UpdateHPUI()
    {
        if (playerHPText) playerHPText.text = $"HP: {playerScript.currentHP}/{playerScript.maxHP}";
        if (dealerHPText) dealerHPText.text = $"HP: {dealerScript.currentHP}/{dealerScript.maxHP}";
        if (playerHPBar) playerHPBar.fillAmount = (float)playerScript.currentHP / playerScript.maxHP;
        if (dealerHPBar) dealerHPBar.fillAmount = (float)dealerScript.currentHP / dealerScript.maxHP;
    }

    void UpdateStatusUI()
    {
        string p = $"Player Win Streak: {playerWinStreak}";
        string d = $"Dealer Win Streak: {dealerWinStreak}";

        if (!string.IsNullOrEmpty(lastPlayerEvent)) p += $"  Last: {lastPlayerEvent}";
        if (!string.IsNullOrEmpty(lastDealerEvent)) d += $"  Last: {lastDealerEvent}";

        if (playerScript.HasPairRightNow()) p += "  Immunity READY";
        if (dealerScript.HasPairRightNow()) d += "  Immunity READY";

        if (playerStatusText) playerStatusText.text = p;
        if (dealerStatusText) dealerStatusText.text = d;
    }
    //show and hide texts
    void ShowStartText(string msg)
    {
        if (mainText) { mainText.text = msg; mainText.gameObject.SetActive(true); }
        if (mainTextBackground) mainTextBackground.gameObject.SetActive(true);
    }

    void HideMainText()
    {
        if (mainText) mainText.gameObject.SetActive(false);
        if (mainTextBackground) mainTextBackground.gameObject.SetActive(false);
    }

    // Deal button is clicked
    private void DealClicked()
    {
        var deck = GameObject.Find("Deck").GetComponent<DeckScript>();
        deck.EnsureShoe(15);
        playerScript.ResetHand();
        dealerScript.ResetHand();
        dealerScoreText.gameObject.SetActive(false);
        HideMainText();
        if (dealerNarrationText) dealerNarrationText.text = "";
        lastPlayerEvent = "—";
        lastDealerEvent = "—";
        deck.Shuffle();
        playerScript.StartHand();
        dealerScript.StartHand();
        scoreText.text = "Hand: " + playerScript.handValue.ToString();
        dealerScoreText.text = "Hand: " + dealerScript.handValue.ToString();

        if (hideCard)
        {
            var back = deck.GetCardBack();
            var card = hideCard.GetComponent<CardScript>();
            if (card && back) card.ShowBack(back);
        }

        dealBtn.gameObject.SetActive(false);
        hitBtn.gameObject.SetActive(true);
        standBtn.gameObject.SetActive(true);
        standBtnText.text = "Stand";

        if (battleFX) battleFX.BothIdle();

        UpdateStatusUI();
    }

    // player chooses to hit
    private void HitClicked()
    {
        if (playerScript.cardIndex <= 10)
        {
            playerScript.GetCard();
            scoreText.text = "Hand: " + playerScript.handValue.ToString();
            if (playerScript.handValue > 20) { RoundOver(); return; }
        }
        UpdateStatusUI();
    }

    // player chooses to stand
    private void StandClicked()
    {
        standClicks++;
        if (standClicks > 1) { RoundOver(); return; }
        HitDealer();
        standBtnText.text = "Call";
    }

    //Dealer hits
    private void HitDealer()
    {
        while (dealerScript.handValue < 16 && dealerScript.cardIndex < 10)
        {
            dealerScript.GetCard();
            dealerScoreText.text = "Hand: " + dealerScript.handValue.ToString();
            if (dealerNarrationText) dealerNarrationText.text = "Dealer hits";
            if (dealerScript.handValue > 20) { RoundOver(); return; }
        }
        if (dealerNarrationText) dealerNarrationText.text = "Dealer stands";
        UpdateStatusUI();
    }

    int BonusFromStreak(int streakAfterThisWin)
    {
        int bonus = 0;
        if (streakAfterThisWin >= 3) bonus += 10;
        if (streakAfterThisWin >= 5) bonus += 10;
        return bonus;
    }

    // once one of the players has won, the round is over
    void RoundOver()
    {
        bool playerBust = playerScript.handValue > 21;
        bool dealerBust = dealerScript.handValue > 21;
        bool player21 = playerScript.handValue == 21;
        bool dealer21 = dealerScript.handValue == 21;

        if (standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21) return;

        int winner = 0;
        if (playerBust && dealerBust) winner = 0;
        else if (playerBust || (!dealerBust && dealerScript.handValue > playerScript.handValue)) winner = -1;
        else if (dealerBust || playerScript.handValue > dealerScript.handValue) winner = 1;

        // Lock in pair immunity flags for THIS round before any damage is dealt
        playerScript.lastHandHasPair = playerScript.HandHasPair();
        dealerScript.lastHandHasPair = dealerScript.HandHasPair();

        bool playerImmune = playerScript.lastHandHasPair;
        bool dealerImmune = dealerScript.lastHandHasPair;

        //play attack then cause damage
        StartCoroutine(DoRoundFXAndResolve(winner, playerImmune, dealerImmune));
        return; // make sure we don't also run the old inline logic
    }

    private void ResolveRoundApplyDamageAndUI(int winner)
    {
        // Re-read immunity from the flags we set in RoundOver()
        bool playerImmune = playerScript.lastHandHasPair;
        bool dealerImmune = dealerScript.lastHandHasPair;

        string msg;
        string extra = "";

        if (winner == 1)
        {
            //increase streak
            int streakAfter = playerWinStreak + 1;
            int bonus = BonusFromStreak(streakAfter);
            int dmg = playerScript.handValue + bonus;

            // check for immunity
            if (dealerImmune)
            {
                dealerScript.lastHandHasPair = false; // consume immunity
                msg = "You win! Dealer blocked damage with pair immunity.";
                lastDealerEvent = "blocked damage (pair immunity)";
                lastPlayerEvent = $"streak {streakAfter}" + (bonus > 0 ? $" (+{bonus})" : "");
                PlayOne(shieldBlock, 0.8f);
            }
            else
            {
                int dealt = dealerScript.TakeDamage(dmg);
                msg = $"You win! Dealer took -{dealt} damage.";
                if (bonus >= 10) extra = $" Streak active (+{bonus}).";
                lastDealerEvent = $"took -{dealt} HP";
                lastPlayerEvent = $"streak {streakAfter}" + (bonus > 0 ? $" (+{bonus})" : "");
                if (dealt > 0) PlayOne(dealerDamage);
            }

            if (dealerWinStreak > 0) lastDealerEvent += " • win streak reset";
            playerWinStreak = streakAfter;
            dealerWinStreak = 0;
        }
        //reset win streak
        else if (winner == -1)
        {
            int streakAfter = dealerWinStreak + 1;
            int bonus = BonusFromStreak(streakAfter);
            int dmg = dealerScript.handValue + bonus;

            if (playerImmune)
            {
                playerScript.lastHandHasPair = false; // consume immunity
                msg = "Dealer wins! You blocked damage with pair immunity.";
                lastPlayerEvent = "blocked damage (pair immunity)";
                lastDealerEvent = $"streak {streakAfter}" + (bonus > 0 ? $" (+{bonus})" : "");
                PlayOne(shieldBlock, 0.8f);
            }
            else
            { // if not immune then deal damage
                int taken = playerScript.TakeDamage(dmg);
                msg = $"Dealer wins! You took -{taken} damage.";
                if (bonus >= 10) extra = $" win Streak active (+{bonus}).";
                lastPlayerEvent = $"took -{taken} HP";
                lastDealerEvent = $"streak {streakAfter}" + (bonus > 0 ? $" (+{bonus})" : "");
                if (taken > 0) PlayOne(playerDamage);
            }
            // check if streak was lost
            if (playerWinStreak > 0) lastPlayerEvent += " • win streak reset";
            dealerWinStreak = streakAfter;
            playerWinStreak = 0;
        }
        else
        {
            // player and dealer match hands
            msg = "Tie: No damage.";
            if (playerWinStreak > 0) lastPlayerEvent = "win streak reset";
            if (dealerWinStreak > 0) lastDealerEvent = "win streak reset";
            playerWinStreak = 0;
            dealerWinStreak = 0;
        }

        // UI & buttons
        mainText.text = msg + extra;
        UpdateHPUI();

        //deal cards screen
        hitBtn.gameObject.SetActive(false);
        standBtn.gameObject.SetActive(false);
        dealBtn.gameObject.SetActive(true);
        mainText.gameObject.SetActive(true);
        if (mainTextBackground) mainTextBackground.gameObject.SetActive(true);
        dealerScoreText.gameObject.SetActive(true);

        if (hideCard)
        {
            var sr = hideCard.GetComponent<Renderer>();
            if (sr) sr.enabled = false;
        }

        standClicks = 0;

        if (playerScript.IsDead || dealerScript.IsDead)
        {
            if (playerScript.IsDead && dealerScript.IsDead) mainText.text = "Double KO!";
            else if (dealerScript.IsDead) mainText.text = "Victory!";
            else mainText.text = "Defeat!";

            if (restartBtn) restartBtn.gameObject.SetActive(true);
            dealBtn.gameObject.SetActive(false);
            hitBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);
        }

        UpdateStatusUI();
    }

    private IEnumerator DoRoundFXAndResolve(int winner, bool playerImmune, bool dealerImmune)
    {
        // Play attack/defense animation before damage
        if (battleFX)
        {
            if (winner == 1)        // Player wins -> Player attacks Dealer
            {
                // dealerImmune => block; KO will be checked after damage
                yield return StartCoroutine(battleFX.PlayerAttacksDealer(dealerImmune, false));
            }
            else if (winner == -1)  // Dealer wins -> Dealer attacks Player
            {
                // playerImmune => block; KO will be checked after damage
                yield return StartCoroutine(battleFX.DealerAttacksPlayer(playerImmune, false));
            }
            else
            {
                // Include tiny pause so the beat feels intentional
                yield return new WaitForSeconds(0.15f);
            }
        }

        // Apply damage/HP/UI/sfx
        ResolveRoundApplyDamageAndUI(winner);

        // KO if someone died (game is over)
        bool playerKO = playerScript != null && playerScript.IsDead;
        bool dealerKO = dealerScript != null && dealerScript.IsDead;

        if (battleFX)
    {
        if (dealerKO) battleFX.KillDealer();
        if (playerKO) battleFX.KillPlayer();
        }
    }
}
