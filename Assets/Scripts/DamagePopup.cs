using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{

    private TextMeshPro textMeshPro;
    private float lifetime;
    private const float LIFETIME_MAX = 1f;
    private Color color;
    private Vector3 movevec;

    private void Awake()
    {
        textMeshPro = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(float dmgAmt , bool isCrit, bool isPlayer)
    {
        textMeshPro.SetText(dmgAmt.ToString());
        if (!isCrit)
        {
            textMeshPro.fontSize = 5f;
            color = Color.yellow;
        }
        else
        {
            textMeshPro.fontSize = 7f;
            color = Color.red;
        }

        if (isPlayer)
        {
            textMeshPro.fontSize = 3f;
        }
        textMeshPro.color = color;
        lifetime = 1f;
        lifetime = LIFETIME_MAX;

        movevec = new Vector3(0.7f, 1) * 8f;
    }

    public void HealSetup(float healAmt)
    {
        textMeshPro.SetText(healAmt.ToString());

        textMeshPro.fontSize = 3f;
        color = Color.green;

        textMeshPro.color = color;
        lifetime = 1f;
        lifetime = LIFETIME_MAX;

        movevec = new Vector3(0.7f, 1) * 8f;
    }

    private void Update()
    {
        transform.position += movevec * Time.deltaTime; //move up
        movevec -= movevec * 8f * Time.deltaTime;

        if (lifetime > LIFETIME_MAX * 0.5)
        {
            float scaleamt = 1f;
            transform.localScale += Vector3.one * scaleamt * Time.deltaTime;
        }
        else
        {
            float scaleamt = 1f;
            transform.localScale -= Vector3.one * scaleamt * Time.deltaTime;
        }

        lifetime -= Time.deltaTime;
        if(lifetime < 0)
        {
            float dissapearspeed = 3f;
            color.a -= dissapearspeed * Time.deltaTime;
            textMeshPro.color = color;
            if(color.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
