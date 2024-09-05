using UnityEngine;

public class Burn_Effect_Script : MonoBehaviour
{
    public float burnDuration = 0.5f; // Duration of the burning effect
    private Texture2D originalTexture;
    private Texture2D burnTexture;
    private SpriteRenderer spriteRenderer;
    private float burnAmount = 0.0f;
    private bool isBurning = false;
    private Color[] originalPixels;
    private Color[] burnPixels;

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure the texture is readable
        if (!IsTextureReadable(spriteRenderer.sprite.texture))
        {
            Debug.LogError("Texture is not readable. Please enable Read/Write in the texture import settings.");
            return;
        }

        // Create a copy of the original texture
        originalTexture = spriteRenderer.sprite.texture;
        burnTexture = new Texture2D(originalTexture.width, originalTexture.height);
        originalPixels = originalTexture.GetPixels();
        burnPixels = new Color[originalPixels.Length];
        System.Array.Copy(originalPixels, burnPixels, originalPixels.Length);
    }

    void Update()
    {
        if (isBurning)
        {
            burnAmount += (Time.deltaTime / burnDuration) * 2;
            ApplyBurnEffect(burnAmount);

            // Destroy the object when fully burned
            if (burnAmount >= 1.0f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void StartBurning()
    {
        isBurning = true;
    }

    private void ApplyBurnEffect(float burnAmount)
    {
        float radius = Mathf.Lerp(0, Mathf.Max(originalTexture.width, originalTexture.height) / 2, burnAmount);
        Vector2 center = new Vector2(originalTexture.width / 2, originalTexture.height / 2);

        for (int y = 0; y < originalTexture.height; y++)
        {
            for (int x = 0; x < originalTexture.width; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                if (distance <= radius)
                {
                    burnPixels[y * originalTexture.width + x].a = 0.0f; // Set alpha to 0
                }
            }
        }

        burnTexture.SetPixels(burnPixels);
        burnTexture.Apply();
        spriteRenderer.sprite = Sprite.Create(burnTexture, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f));
    }

    private bool IsTextureReadable(Texture2D texture)
    {
        try
        {
            texture.GetPixels();
            return true;
        }
        catch (UnityException e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }
}
