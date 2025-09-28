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

        currentTime = timeLimit;

        StartCoroutine("FlipAllCardsRoutine");
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
                GameOver(true);
            }
        }
        else
        {
            yield return new WaitForSeconds(1f);

            card1.FlipCard();
            card2.FlipCard();

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
