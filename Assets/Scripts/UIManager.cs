using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText, _ammoText;
    [SerializeField]
    private Image _livesImage, _shieldsImage, _ammoImage;
    private int _maxAmmoCount = 15;
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
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLive(int currentLives)
    {
        _livesImage.sprite = _liveSprites[Mathf.Clamp(currentLives,0,3)];
        if (currentLives == 0) StartCoroutine(GameOverFlickerRoutine());
    }

    public void UpdateShield(int currentShields)
    {
        _shieldsImage.sprite = _shieldSprites[Mathf.Clamp(currentShields, 0, 3)];
    }

    public void SetMaxAmmoCount(int maxAmmoCount)
    {
        _maxAmmoCount = maxAmmoCount;
    }
    public void UpdateAmmo(int currentAmmo)
    {
        _ammoImage.fillAmount = (float)currentAmmo / _maxAmmoCount;
        _ammoText.text = currentAmmo.ToString();
    }

    IEnumerator GameOverFlickerRoutine()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "GAME OVER";
        }
    }
}
