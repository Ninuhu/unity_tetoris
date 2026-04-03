using UnityEngine;
using TMPro;
using UnityEngine.UI;



public class SettingsMana : MonoBehaviour
{
    private bool isLoading = false;

    

    [Header("Dropdowns")]
    public TMP_Dropdown bgmDropdown;
    public TMP_Dropdown seDropdown;
    public TMP_Dropdown brightnessDropdown;

    [Header("Toggles")]
    public Toggle holdToggle;

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioSource seSource;



    void Start()
    {
        LoadSettings();
        ApplySettings();
    }

    // 保存処理(やめても変更点は保存される)
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("BGMVolume", bgmDropdown.value);
        PlayerPrefs.SetInt("SEVolume", seDropdown.value);
        PlayerPrefs.SetInt("Brightness", brightnessDropdown.value);
        PlayerPrefs.SetInt("HoldEnabled", holdToggle.isOn ? 1 : 0);

        PlayerPrefs.Save();

        ApplySettings();
    }

    //  読み込み
    void LoadSettings()
    {
        isLoading = true;
        bgmDropdown.value = PlayerPrefs.GetInt("BGMVolume", 2);        // 初期値3 → index2
        seDropdown.value = PlayerPrefs.GetInt("SEVolume", 2);
        brightnessDropdown.value = PlayerPrefs.GetInt("Brightness", 2);

        holdToggle.isOn = PlayerPrefs.GetInt("HoldEnabled", 1) == 1;  // 初期 ON
        isLoading = false;
    }
    public CanvasGroup brightnessPanel; //　黒壁で明るさ調整する

    // 実際の適用
    void ApplySettings()
    {
        if (isLoading) return;
        float bgmVolume = (bgmDropdown.value + 1) / 5f;
        float seVolume = (seDropdown.value + 1) / 5f;

        bgmSource.volume = bgmVolume;
        seSource.volume = seVolume;

        // 明るさ 1〜5 → black panel の濃さへ変換
        float level = brightnessDropdown.value; // 0〜4
        float darkness = 0.7f - (level * 0.175f);
        darkness = Mathf.Clamp(darkness, 0f, 0.7f);
        brightnessPanel.alpha = darkness;
        

        // ホールド の設定
        GameSettings.holdEnabled = holdToggle.isOn;
    }
}