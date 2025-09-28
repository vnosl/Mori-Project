using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private List<Card> allCards;
    private Card tarotCard;
    private Card flippedCard;
    private bool isFlipping = false;

    [SerializeField]
    private Slider timeoutSlider;

    [SerializeField]
    private TextMeshProUGUI timeoutText;

    [SerializeField]
    private TextMeshProUGUI gameOverText;

    [SerializeField]
    private GameObject gameOverPanel;
    private bool isGameOver = false;

    [SerializeField]
    private float timeLimit = 30f;
    private float currentTime;
    private int totalMatches = 7;
    private int matchesFound = 0;

    [SerializeField]
    private int maxLives = 3;
    private int lives;

    [SerializeField]
    private TextMeshProUGUI livesText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Board board = FindObjectOfType<Board>();
        allCards = board.getCards();

        tarotCard = allCards.Find(c => c.GetIsTarot());
        currentTime = timeLimit;

        lives = maxLives;
        UpdateLivesUI();

        StartCoroutine("FlipAllCardsRoutine");
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.SetText($"Life: {lives}/{maxLives}");
        }
    }
    IEnumerator FlipAllCardsRoutine()
    {
        isFlipping = true;
        yield return new WaitForSeconds(0.5f);
        FlipAllCards();
        yield return new WaitForSeconds(3f);
        FlipAllCards();
        yield return new WaitForSeconds(0.5f);
        isFlipping = false;

        yield return StartCoroutine("CountDownTimerRoutine");
    }

    IEnumerator CountDownTimerRoutine()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            timeoutSlider.value = currentTime / timeLimit;
            yield return null;
        }

        GameOver(false);
    }

    void FlipAllCards()
    {
        foreach (Card card in allCards)
        {
            if (card.GetIsTarot())
            {
                continue;
            }

            card.FlipCard();
        }
    }

    public void CardClicked(Card card)
    {
        if (isFlipping || isGameOver)
        {
            return;
        }

        card.FlipCard();

        if (flippedCard == null)
        {
            flippedCard = card;
        }
        else
        {
            StartCoroutine(CheckMatchRoutine(flippedCard, card));
        }
    }

    IEnumerator CheckMatchRoutine(Card card1, Card card2)
    {
        isFlipping = true;

        if (card1.cardID == card2.cardID)
        {
            card1.SetMatched();
            card2.SetMatched();
            matchesFound++;

            if (matchesFound == totalMatches)
            {

                yield return new WaitForSeconds(1f);
                tarotCard.FlipCard();

                GameOver(true);
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);

            card1.FlipCard();
            card2.FlipCard();

            lives = Mathf.Max(0, lives - 1);
            UpdateLivesUI();

            if (lives <= 0)
            {
                GameOver(false);
                yield break;
            }

            yield return new WaitForSeconds(0.4f);
        }

        isFlipping = false;
        flippedCard = null;
    }

    void GameOver(bool successs)
    {
        if (!isGameOver)
        {
            isGameOver = true;

            StopCoroutine("CountDownTimerRoutine");

            if (successs)
            {
                gameOverText.SetText("Success!");
            }
            else
            {
                gameOverText.SetText("Failure!");
            }

            Invoke("ShowGameOverPanel", 2f);
        }
    }

    void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }
}
