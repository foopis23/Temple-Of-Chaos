using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CallbackEvents;

public class CardHudController : MonoBehaviour
{
	// editor fields
	public int numCards;
	public float cardOffset;
	public float cardSelectTimeout;
	public float cardSlideSpeed;
	public float cardSlidePos;
	public Player player;
	public GameObject heldCardsObject;
	public GameObject newCard;
	public GameObject cardPrefab;
	public CardDeck cardDeck;
	public TMP_Text coinText;
	public TMP_Text expiresIn;
	public TMP_Text tutorialText;
	private bool _hasStoredCard;
	private bool _hasReplacedCard;

	// protected fields
	PlayerControls controls;

	// private fields
	private CardInventory cardInventory;
	private RawImage[] heldCards;
	private int selectedCard;
	private float selectTime;

	void Awake()
	{
		controls = new PlayerControls();
	}

	void OnEnable()
	{
		controls.Enable();
	}

	void OnDisable()
	{
		controls.Disable();
	}

	void Start()
	{
		cardInventory = player.Inventory;
		heldCards = new RawImage[numCards];
		selectedCard = -1;
		newCard.SetActive(false);
		tutorialText.gameObject.SetActive(false);
		EventSystem.Current.RegisterEventListener<CardFailed>(OnCardFailed);

		for (int i = 0; i < numCards; i++)
		{
			var obj = Instantiate(cardPrefab, heldCardsObject.transform);
			var rawImage = obj.GetComponent<RawImage>();
			rawImage.rectTransform.anchoredPosition = new Vector3(cardOffset * i, cardSlidePos, 0f);
			heldCards[i] = rawImage;
		}

		// handle input
		controls.CardManagement.CardSlot1.performed += ctx => HandleCardSelect(0);
		controls.CardManagement.CardSlot2.performed += ctx => HandleCardSelect(1);
		controls.CardManagement.CardSlot3.performed += ctx => HandleCardSelect(2);
		controls.CardManagement.CardSlot4.performed += ctx => HandleCardSelect(3);
		controls.CardManagement.CardSlot5.performed += ctx => HandleCardSelect(4);
	}

	void Update()
	{
		if (player.purchaseCardPoints > 0)
		{
			coinText.text = $"Press Q to Buy Card ({player.purchaseCardPoints})";
		}
		else
		{
			coinText.text = "Find more coins to buy cards.";
		}

		if (cardDeck.PurchasedCard != null)
		{
			newCard.SetActive(true);
			newCard.gameObject.transform.Find("Icon").GetComponent<RawImage>().texture = cardDeck.PurchasedCard.Icon;
			newCard.gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = cardDeck.PurchasedCard.Name;
			newCard.gameObject.transform.Find("Info Text").GetComponent<TextMeshProUGUI>().text =
				cardDeck.PurchasedCard.FlavorText;
			newCard.gameObject.transform.Find("Chaos Percent").GetComponent<TextMeshProUGUI>().text = ((int)(cardDeck.PurchasedCard.ChaosLevel * 100)).ToString() + "%";
			expiresIn.text = $"Expires In: {(int)(cardDeck.cardExpireTime - (Time.time - cardDeck.LastBoughtCardTime))}";

			if (!_hasStoredCard)
			{
				tutorialText.text = "Press 1-5 to Equip Purchased Card";
				tutorialText.gameObject.SetActive(true);
			}
			else if (!_hasReplacedCard)
			{
				tutorialText.text = "Double tap 1-5 to Replace Card with Purchased Card";
				tutorialText.gameObject.SetActive(true);
			}
		}
		else
		{
			tutorialText.gameObject.SetActive(false);
			newCard.SetActive(false);
		}

		if (Time.time - selectTime > cardSelectTimeout)
		{
			selectedCard = -1;
		}

		for (int i = 0; i < heldCards.Length; i++)
		{
			var heldCard = heldCards[i];
			if (cardInventory.Cards[i] == null)
			{
				heldCard.gameObject.SetActive(false);
				continue;
			}
			else
			{
				var cardData = cardInventory.Cards[i];
				heldCard.gameObject.SetActive(true);
				heldCard.gameObject.transform.Find("Icon").GetComponent<RawImage>().texture = cardData.Icon;
				heldCard.gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = cardData.Name;
				heldCard.gameObject.transform.Find("Info Text").GetComponent<TextMeshProUGUI>().text =
					cardData.FlavorText;
				heldCard.gameObject.transform.Find("Chaos Percent").GetComponent<TextMeshProUGUI>().text = ((int)(cardData.ChaosLevel * 100)).ToString() + "%";
			}

			int slideDir = i == selectedCard ? 1 : -1;
			float currentY = heldCard.rectTransform.anchoredPosition.y;
			if (currentY > -0.001f && slideDir == 1)
			{
				currentY = 0;
			}
			else if (currentY < cardSlidePos + 0.001f && slideDir == -1)
			{
				currentY = cardSlidePos;
			}
			else
			{
				currentY += slideDir * cardSlideSpeed * Time.deltaTime;
				if (currentY > -0.001f)
				{
					currentY = 0;
				}
				else if (currentY < cardSlidePos + 0.001f)
				{
					currentY = cardSlidePos;
				}

				heldCard.rectTransform.anchoredPosition = new Vector3(heldCard.rectTransform.anchoredPosition.x, currentY, 0f);
			}
		}
	}

	private void HandleCardSelect(int cardIndex)
	{
		if (cardDeck.PurchasedCard != null && cardIndex == selectedCard)
		{
			_hasStoredCard = true;
			if (cardInventory.Cards[cardIndex] != null) _hasReplacedCard = true;

			cardInventory.Equip(cardDeck.PurchasedCard, cardIndex);
			cardDeck.TakePurchasedCard();
		}
		else
		{
			if (cardInventory.Cards[cardIndex] != null)
			{
				selectedCard = cardIndex;
				selectTime = Time.time;
			}
			else if (cardDeck.PurchasedCard != null)
			{
				_hasStoredCard = true;
				if (cardInventory.Cards[cardIndex] != null) _hasReplacedCard = true;

				cardInventory.Equip(cardDeck.PurchasedCard, cardIndex);
				cardDeck.TakePurchasedCard();
			}
		}
	}

	private void OnCardFailed(CardFailed context)
	{
		for (int i = 0; i < heldCards.Length; i++)
		{
			var heldCard = heldCards[i];
			var cardData = cardInventory.Cards[i];
			if (cardData == context.CardObject)
			{
				heldCard.color = new Color(1, 0, 0);
				EventSystem.Current.CallbackAfter(() =>
				{
					heldCard.color = new Color(1, 1, 1);
				}, 500);
			}
		}
	}
}
