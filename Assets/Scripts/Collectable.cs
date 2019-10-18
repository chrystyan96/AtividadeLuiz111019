using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour {
    private bool getDoubleJump;
    private bool getRapidShoot;
    [SerializeField] private int points;

    [Header("Power Up")]
	[Space]
    [SerializeField] private bool canPowerUp;
    [Space]
    [SerializeField] private string ability;

    public int getPoints() {
        return points;
    }

    public string GetAbilityName() {
        return ability;
    }
}
