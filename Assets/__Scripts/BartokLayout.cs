using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotDef
{
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>();
    public float rot;
    public string type = "slot";
    public Vector2 stagger;
    public int player;
    public Vector3 pos;
}

public class BartokLayout : MonoBehaviour {

    public PT_XMLReader xmlr;
    public PT_XMLHashtable xml;
    public Vector2 multiplier;
    public List<SlotDef> slotDefs;
    public SlotDef drawPile;
    public SlotDef discardPile;
    public SlotDef target;

    //This function is called to read in the LayoutXML.xml file
    public void ReadLayout(string xmlText)
    {
        xmlr = new PT_XMLReader();
        xmlr.Parse(xmlText);
        xml = xmlr.xml["xml"][0]; //reference to the xml

        //read in the multiplier which sets card spacing
        multiplier.x = float.Parse(xml["multiplier"][0].att("x"));
        multiplier.y = float.Parse(xml["multiplier"][0].att("y"));

        //read in the slots
        SlotDef tSD;
        //slotsX is used as a shortcut to all the <slot>s
        PT_XMLHashList slotsX = xml["slot"];
        for (int i = 0; i < slotsX.Count; i++)
        {
            tSD = new SlotDef();
            if (slotsX[i].HasAtt("type"))
                tSD.type = slotsX[i].att("type");
            else
                tSD.type = "slot";

            tSD.x = float.Parse(slotsX[i].att("x"));
            tSD.y = float.Parse(slotsX[i].att("y"));
            tSD.pos = new Vector3(tSD.x * multiplier.x, tSD.y * multiplier.y, 0);

            //sorting layers
            tSD.layerID = int.Parse(slotsX[i].att("layer"));
            tSD.layerName = tSD.layerID.ToString();
            //the layers are used to make sure the correct cards are on top of the others

            //pull additional attributes based on the type of each <slot>
            try
            {
                switch (tSD.type)
                {
                    case "slot":
                        //ignore slots that are just of the "slot" type
                        break;
                    case "drawpile":
                        //the drawpile xstagger is read but not actually used in Bartok
                        tSD.stagger.x = float.Parse(slotsX[i].att("xstagger"));
                        drawPile = tSD;
                        break;
                    case "discardpile":
                        discardPile = tSD;
                        break;
                    case "target":
                        //the target card has a different layer from discardpile
                        target = tSD;
                        break;
                    case "hand":
                        //information for each player's hand
                        tSD.player = int.Parse(slotsX[i].att("player"));
                        tSD.rot = float.Parse(slotsX[i].att("rot"));
                        slotDefs.Add(tSD);
                        break;
                }
            }
            catch (System.FormatException e)
            {

                Debug.LogError(e.Message);
            }
            
        }        
    }
}
