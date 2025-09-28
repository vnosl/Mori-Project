using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer cardRenderer;

    [SerializeField]
    private Sprite frontSprite;

    [SerializeField]
    private Sprite backSprite;

    private bool isFlipped = false;
    private bool isFlipping = false;
    private bool isMatched = false;

    public int cardID;

    public void SetCardID(int id)
    {
        this.cardID = id;
    }

    public void SetMatched()
    {
        isMatched = true;
    }

    public void SetFrontSprite(Sprite sprite)
    {
        this.frontSprite = sprite;
    }

    public void FlipCard()
    {
        isFlipping = true;

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = new Vector3(0f, originalScale.y, originalScale.z);

        transform.DOScale(targetScale, 0.2f).OnComplete(() =>
        {
            isFlipped = !isFlipped;

            if (isFlipped)
            {
                cardRenderer.sprite = frontSprite;
            }
            else
            {
                cardRenderer.sprite = backSprite;
            }

            transform.DOScale(originalScale, 0.2f).OnComplete(() => 
            {
                isFlipping = false;
            });
        });


    }

    void OnMouseDown()
    {
        if (!isFlipping && !isMatched && !isFlipped)
        {
            GameManager.instance.CardClicked(this);
        }
    }
}
