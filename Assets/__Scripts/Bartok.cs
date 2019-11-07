﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bartok : MonoBehaviour {

    static public Bartok S;

    public TextAsset deckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCenter = Vector3.zero;
    public float handFanDegrees = 10f;
    public bool _______________;
    public Deck deck;
    public List<CardBartok> drawPile;
    public List<CardBartok> discardPile;
    public BartokLayout layout;
    public Transform layoutAnchor;
    public List<Player> players;
    public CardBartok targetCard;


    void Awake()
    {
        S = this;
    }

    void Start()
    {
        deck = GetComponent<Deck>();
        deck.InitDeck(deckXML.text);
        Deck.Shuffle(ref deck.cards);

        layout = GetComponent<BartokLayout>();
        layout.ReadLayout(layoutXML.text);
         
        drawPile = UpgradeCardsList(deck.cards);
        LayoutGame();
    }

    //UpgradeCardsList casts the Cards in lCD to be CardBartoks
    List<CardBartok> UpgradeCardsList(List<Card> lCD)
    {
        List<CardBartok> lCB = new List<CardBartok>();
        foreach (Card tCD in lCD)
        {
            lCB.Add(tCD as CardBartok);
        }
        return lCB;
    }

    //Position all the cards in the drawPile properly
    public void ArrangeDrawPile()
    {
        CardBartok tCB;
        for (int i = 0; i < drawPile.Count; i++)
        {
            tCB = drawPile[i];
            tCB.transform.parent = layoutAnchor;
            tCB.transform.localPosition = layout.drawPile.pos;
            tCB.faceUp = false;
            tCB.SetSortingLayerName(layout.drawPile.layerName);
            tCB.SetSortOrder(-i * 4);
            tCB.state = CBState.drawpile;
        }
    }

    //Perform the initial game layout
    void LayoutGame()
    {
        //create an empty GameObject to serve as an anchor for the tableau
        if(layoutAnchor== null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }

        //position the drawpile cards
        ArrangeDrawPile();

        //setup the players
        Player pl;
        players = new List<Player>();
        foreach (SlotDef tSD in layout.slotDefs)
        {
            pl = new Player();
            pl.handSlotDef = tSD;
            players.Add(pl);
            pl.playerNum = players.Count;
        }
        players[0].type = PlayerType.Human;
    }

    //The Draw function will pull a single card from the drawpile and return it
    public CardBartok Draw()
    {
        CardBartok cd = drawPile[0];
        drawPile.RemoveAt(0);
        return cd;
    }

    //This Update method is used to test adding cards to players' hands
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            players[0].AddCard(Draw());
        if (Input.GetKeyDown(KeyCode.Alpha2))
            players[1].AddCard(Draw());
        if (Input.GetKeyDown(KeyCode.Alpha3))
            players[2].AddCard(Draw());
        if (Input.GetKeyDown(KeyCode.Alpha4))
            players[3].AddCard(Draw());
    }
}
