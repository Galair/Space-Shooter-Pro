using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText, _ammoText, _maxAmmoText, _thrusterText, _waveText;
    [SerializeField]
    private Image _livesImage, _shieldsImage, _ammoImage, _thrusterImage;
    private int _maxAmmoCount = 15;
    private float _maxThrusterNitro = 5.0f;
    [SerializeField]
    private Sprite[] _liveSprites, _shieldSprites;
    [SerializeField]
    private Text _gameOverText;
    private GameManager _gameManager;


    // Start is called before the first frame update
    void Start()
    {
        if (!GameObject.Find("Game_Manager").TryGetComponent<GameManager>(out _gameManager))
        {
            Debug.LogError(gameObject.name + ": The GameManager is NULL.");
        }
        UpdateScore(0);
        UpdateLive(3);
        UpdateShield(0);
        _gameOverText.gameObject.SetActive(false);
        _waveText.gameObject.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLive(int currentLives)
    {
        _livesImage.sprite = _liveSprites[Mathf.Clamp(currentLives,0,3)];
        if (currentLives == 0) StartCoroutine(GameOverFlickerRoutine("GAME OVER"));
    }

    public void UpdateShield(int currentShields)
    {
        _shieldsImage.sprite = _shieldSprites[Mathf.Clamp(currentShields, 0, 3)];
    }

    public void SetMaxAmmoCount(int maxAmmoCount)
    {
        _maxAmmoCount = maxAmmoCount;
        _maxAmmoText.text = _maxAmmoCount.ToString();
    }
    public void UpdateAmmo(int currentAmmo)
    {
        _ammoImage.fillAmount = (float)currentAmmo / _maxAmmoCount;
        _ammoText.text = currentAmmo.ToString();
    }

    public void SetMaxThrusterNitro(int maxThrusterNitro)
    {
        _maxThrusterNitro = maxThrusterNitro;
    }
    public void UpdateThrusterNitro(float thrusterNitro)
    {
        _thrusterImage.fillAmount = thrusterNitro / _maxThrusterNitro;
        _thrusterText.text = Mathf.RoundToInt(_thrusterImage.fillAmount * 100).ToString() + "%";
    }

    public void ShowWaveNumber(int waveNumber, int enemyNumber, bool isFinalWave = false)
    {
        if (isFinalWave)
        {
            _waveText.text = "Final Wave";
        }
        else
        {
            _waveText.text = "Wave: " + waveNumber + Environment.NewLine + " Enemies: " + enemyNumber;
        }
        StartCoroutine(WaveNumberShowRoutine());
    }

    IEnumerator GameOverFlickerRoutine(string gameOverText)
    {
        yield return new WaitForSeconds(1.0f);
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = gameOverText;
        }
    }

    IEnumerator WaveNumberShowRoutine()
    {
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        _waveText.gameObject.SetActive(false);
    }

    public void GameWon()
    {
        StartCoroutine(GameOverFlickerRoutine("GAME WON!"));
    }
}
