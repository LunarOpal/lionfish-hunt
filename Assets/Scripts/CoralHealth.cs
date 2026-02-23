using UnityEngine;

public class CoralHealth : MonoBehaviour
{
    public Sprite coral_healthy;
    public Sprite coral_stressed;
    public Sprite coral_bleached;
    public Sprite coral_dead;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = coral_stressed;
        }
    }


    public void coralHealthCheck(float health)
    {
        if (spriteRenderer != null)
        {
            if (health >= 80f)
            {
                spriteRenderer.sprite = coral_healthy;
            }
            else if (health >= 30f)
            {
                spriteRenderer.sprite = coral_stressed;
            }
            else if (health >= -50f)
            {
                spriteRenderer.sprite = coral_bleached;
            }
            else
            {
                spriteRenderer.sprite = coral_dead;
            }
        }
    }


}
