using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image foregroundImage;
    [SerializeField]
    private float updateSpeedSeconds = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponentInParent<Health>().OnHealthPctChanged += HandleHealthChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleHealthChanged(float percent)
    {
        StartCoroutine(ChangeToPct(percent));
    }

    private IEnumerator ChangeToPct(float percent)
    {
        float preChangePercent = foregroundImage.fillAmount;
        float elasped = 0f;

        while (elasped < updateSpeedSeconds)
        {
            elasped += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(preChangePercent, percent, elasped / updateSpeedSeconds);
            yield return null;
        }
        foregroundImage.fillAmount = percent;
        //test
    }
}
