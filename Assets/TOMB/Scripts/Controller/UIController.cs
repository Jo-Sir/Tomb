using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

[System.Serializable]
public class GunSpriteData
{
    public Sprite gunsBodySprite;
    public Sprite magazineSprite;
    public Sprite scopeSprite;
}
public class UIController : MonoBehaviour
{
    public UnityAction<float, float> changeUIText;
    public UnityAction<int> changeGunImage;
    public UnityAction<float> changeHpBar;
    [SerializeField] private List<GunSpriteData> gunSpriteDatas = new List<GunSpriteData>();
    [SerializeField] private TextMeshProUGUI curAmmo;
    [SerializeField] private TextMeshProUGUI totalAmmo;
    [SerializeField] private Image gunsBody;
    [SerializeField] private Image magazine;
    [SerializeField] private Image scope;
    [SerializeField] private Image hpbar;
    [SerializeField] private Image damageBar;
    [SerializeField] private Image fade;
    [SerializeField] private GameObject alert;
    private CanvasRenderer[] alerts;
    [SerializeField] private float playerMaxHp;

    private void Awake()
    {
        playerMaxHp = PlayerManager.Instance.player.MaxHp;
        changeUIText = (float curAmmo, float totalAmmo) => TextAmmoChange(curAmmo, totalAmmo);
        changeGunImage = (int curGunNum) => GunsImageChange(curGunNum);
        changeHpBar = (float curHp) => UpdateHp(curHp);
        hpbar.fillAmount = 1f; 
        damageBar.fillAmount = 1f;
        alerts = alert.GetComponentsInChildren<CanvasRenderer>(true);
    }

    public void TextAmmoChange(float curAmmo, float totalAmmo)
    {
        string strCurAmmo = curAmmo.ToString();
        string strTotalAmmo = totalAmmo.ToString();
        if (totalAmmo == 999)
        {
            strTotalAmmo = " ∞";
        }
        this.curAmmo.text = strCurAmmo;
        this.totalAmmo.text = strTotalAmmo;
    }
    public void GunsImageChange(int curGunNum)
    {
        gunsBody.sprite = gunSpriteDatas[curGunNum].gunsBodySprite;
        magazine.sprite = gunSpriteDatas[curGunNum].magazineSprite;
        scope.sprite = gunSpriteDatas[curGunNum].scopeSprite;
    }
    public void IsDie()
    {
        for (int i = 0; i < alerts.Length; i++)
        {
            alerts[i].gameObject.SetActive(true);
        }
        StartCoroutine(FadeOut());
    }
    public void GameStart()
    {
        PlayerManager.Instance.player.IsDie = true;
        Color a = fade.GetComponent<Image>().color;
        a.a = 1f;
        StartCoroutine(FadeIn());
    }
    public void GameClear()
    {
        TextMeshProUGUI text = alerts[1].GetComponentInChildren<TextMeshProUGUI>();
        for (int i = 0; i < alerts.Length; i++)
        {
            alerts[i].gameObject.SetActive(true);
        }
        text.color = new Color32(222, 182, 102, 255);
        text.text = "Clear";
    }
    private void UpdateHp(float curHp)
    {
        float cent = curHp / playerMaxHp;// 목적지
        if (hpbar.fillAmount < cent) 
        { // 회복
            StartCoroutine(UpdateGreenHp(cent));
        }
        else 
        { // 맞았을때
            hpbar.fillAmount = cent;
            StartCoroutine(UpdateRedHp(cent));
        }
    }
    private IEnumerator UpdateRedHp(float cent)
    {
        for (float i = damageBar.fillAmount; i > cent; i-=0.007f)
        {
            damageBar.fillAmount = i;
            yield return null;
        }
        damageBar.fillAmount -= 0.005f;
    }
    private IEnumerator UpdateGreenHp(float cent)
    {
        for (float i = hpbar.fillAmount; i < cent; i += 0.007f)
        {
            hpbar.fillAmount = i;
            yield return null;
        }
        damageBar.fillAmount = hpbar.fillAmount;
    }

    private IEnumerator FadeOut()
    {
        for (float f = 0f; f < 1; f += 0.5f * Time.deltaTime)
        {
            Color c = fade.GetComponentInChildren<Image>().color;
            c.a = f;
            fade.GetComponentInChildren<Image>().color = c;
            yield return null;
        }
    }
    private IEnumerator FadeIn()
    {
        for (float f = 1f; f > 0; f -= 0.5f * Time.deltaTime)
        {
            Color c = fade.GetComponentInChildren<Image>().color;
            c.a = f;
            fade.GetComponentInChildren<Image>().color = c;
            yield return null;
        }
        PlayerManager.Instance.player.IsDie = false;
    }
}
