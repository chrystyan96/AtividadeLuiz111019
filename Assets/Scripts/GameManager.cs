using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {
    [SerializeField] private int score;
    [SerializeField] private TMP_Text scoreScreen;

    void Start() {
        score = 0;
    }

    void Update() {
        scoreScreen.text = score.ToString();
    }

    public void UpdateScore(int value) {
        score += value;
    }
}
