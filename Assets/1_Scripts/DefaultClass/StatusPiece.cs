using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatusPiece : MonoBehaviour, IPointerDownHandler
{
    public Piece piece;

    public GameObject deathObject;
    public GameObject freezeObject;
    public GameObject weakObject;
    public GameObject shockObject;
    public GameObject distortObject;
    public GameObject knockbackObject;
    public GameObject[] blankHeart = new GameObject[5];
    public GameObject[] displayHearts;                           // 활성화된 하트

    public int blankHp = 0;

    private void Start()
    {
        InitStatus();
    }

    public void InitStatus()
    {
        blankHp = (int)(piece.GetHp());
        displayHearts = new GameObject[blankHp];
        for (int i = 0; i < blankHp; i++)
        {
            displayHearts[i] = blankHeart[i];
            displayHearts[i].SetActive(true);
            displayHearts[i].transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void StatusUpdate()
    {
        if (deathObject.activeSelf)
            return;

        DisableEffect();

        if (!piece.GetIsAlive())
        {
            deathObject.SetActive(true);
            for (int i = 0; i < blankHp; i++)
            {
                displayHearts[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            return;
        }

        switch (piece.GetStatusEffect())
        {
            case eStatusEffectType.Freeze:
                freezeObject.SetActive(true);
                break;
            case eStatusEffectType.Weak:
                weakObject.SetActive(true);
                break;
            case eStatusEffectType.Shock:
                shockObject.SetActive(true);
                break;
            case eStatusEffectType.Distort:
                distortObject.SetActive(true);
                break;
            default:
                break;
        }

        for (int i = 0; i < blankHp; i++)
        {
            bool isHeartFilled = piece.GetHp() - 1 >= i;
            displayHearts[i].transform.GetChild(0).gameObject.SetActive(isHeartFilled);
        }
    }

    void DisableEffect()
    {
        deathObject.SetActive(false);
        freezeObject.SetActive(false);
        weakObject.SetActive(false);
        shockObject.SetActive(false);
        distortObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (clickedObject == gameObject)
        {
            BattlePieceManager.instance.DisableEffectPieces();
            BattlePieceManager.instance.EffectSelectedPiece(piece);
            PieceControlManager.instance.preventEffect = true;
        }
    }
}
