using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    public TextMeshProUGUI comboPopupText;
    public float popupDuration = 0.5f;
    public AnimationCurve popupScaleCurve;
    public AudioClip comboSound;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void UpdateCombo(int combo)
    {
        comboText.text = combo > 2 ? $"Combo: {combo}!" : "";
    }

    public void ShowComboPopup(int combo)
    {
        if (combo < 2) return;

        comboPopupText.gameObject.SetActive(true);
        comboPopupText.text = $"COMBO {combo}!";

        comboPopupText.DOKill();
        comboPopupText.transform.DOKill();
        comboPopupText.color = new Color(1, 1, 1, 1);
        comboPopupText.transform.localScale = Vector3.zero;

        comboPopupText.transform
            .DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutBack);

        // Fade out and disable
        comboPopupText.DOFade(0f, 0.5f)
            .SetDelay(0.5f)
            .OnComplete(() => comboPopupText.gameObject.SetActive(false));

        if (comboSound != null && audioSource != null)
            audioSource.PlayOneShot(comboSound);
    }
}
