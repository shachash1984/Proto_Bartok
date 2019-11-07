using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CBState
{
    drawpile,
    toHand,
    hand,
    toTarget,
    target,
    discard,
    to,
    idle
}
public class CardBartok : Card {

    static public float MOVE_DURATION = 0.5f;
    static public string MOVE_EASING = Easing.InOut;
    static public float CARD_HEIGHT = 3.5f;
    static public float CARD_WIDTH = 2f;

    public CBState state = CBState.drawpile;

    //fields to store info the card will use to move and rotate
    public List<Vector3> bezierPts;
    public List<Quaternion> bezierRots;
    public float timeStart, timeDuration;

    //when the card is done moving, it will call reportFinishTo.SendMessage()
    public GameObject reportFinishTo = null;

    //moveTo tells the card to interpolate to a new position and rotation
    public void MoveTo(Vector3 ePos, Quaternion eRot)
    {
        //make new interpolation lists for the card
        //Position and Rotation will each have only two points
        bezierPts = new List<Vector3>();
        bezierPts.Add(transform.localPosition);
        bezierPts.Add(ePos);
        bezierRots = new List<Quaternion>();
        bezierRots.Add(transform.rotation);
        bezierRots.Add(eRot);

        if (timeStart == 0)
            timeStart = Time.time;

        timeDuration = MOVE_DURATION;

        //setting state to either toHand or toTarget will be handled by the calling method

        state = CBState.to;
    }

    public void MoveTo(Vector3 ePos)
    {
        MoveTo(ePos, Quaternion.identity);
    }

    void Update()
    {
        switch (state)
        {
            case CBState.toHand:
                break;
            case CBState.toTarget:
                break;
            case CBState.to:
                //get u from the current time and duration, u ranges from 0 to 1 usually
                float u = (Time.time - timeStart) / timeDuration;
                //use Easing class from Utils to curve the u value
                float uC = Easing.Ease(u, MOVE_EASING);
                if (u < 0) // if u < 0 we shouldnt move yet
                {
                    transform.localPosition = bezierPts[0];
                    transform.rotation = bezierRots[0];
                    return;
                }
                else if (u >= 1) // wer'e finished moving
                {
                    uC = 1;
                    //move from the to____ state to the following state
                    if (state == CBState.toHand)
                        state = CBState.toHand;
                    if (state == CBState.toTarget)
                        state = CBState.toTarget;
                    if (state == CBState.to)
                        state = CBState.idle;

                    //move to the final position
                    transform.localPosition = bezierPts[bezierPts.Count - 1];
                    transform.rotation = bezierRots[bezierRots.Count - 1];

                    //reset timeStart to 0 so it gets overwritten next time
                    timeStart = 0;

                    if (reportFinishTo != null) // if there's a callback GameObject then use SendMessage to call the CBCallback method with this as the parameter
                    {
                        reportFinishTo.SendMessage("CBCallback", this);
                        //after calling SendMessage(), reportFinishTo must be set to null so that the card dowsnt continue to report to the same GameObject every subsequent time it moves
                        reportFinishTo = null;
                    }
                }
                else // 0<=u<=1, which means that this is interpolating now. user Bezier curve to move this to the right point
                {
                    Vector3 pos = Utils.Bezier(uC, bezierPts);
                    transform.localPosition = pos;
                    Quaternion rotQ = Utils.Bezier(uC, bezierRots);
                    transform.rotation = rotQ;
                }

                
                break;
            case CBState.drawpile:
                break;            
            case CBState.hand:
                break;
            
            case CBState.target:
                break;
            case CBState.discard:
                break;
            
            case CBState.idle:
                break;
            default:
                break;
        }
    }
    
}
