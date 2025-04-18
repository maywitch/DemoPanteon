using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Grid koordinatları
    public int gridX;
    public int gridY;

    // Tile'ın geçilebilirlik durumu (A* algoritmasında kullanılır)
    public bool walkable = true;

    // A* hesaplamaları için maliyet değerleri:
    public int gCost; // Başlangıçtan bu tile'a olan maliyet
    public int hCost; // Hedefe olan tahmini maliyet
    public int fCost { get { return gCost + hCost; } } // Toplam maliyet

    // Yol bulma sırasında geriye dönük referans
    public Tile parent;

    // Renk ve görsel özellikler
    private SpriteRenderer rend;
    public Color highlightedColor;
    public Color creatableColor;

    public LayerMask obstacles;

    public bool isCreatable;

    // Diğer özellikler
    public float scaleIncreaseAmount = 0.1f;
    private bool isScaled = false;

    private AudioSource audioSource;

    // Tile'ın işgal durumunu tutan özel alan
    private bool occupied = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rend = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Bu tile'ın üzerinde engel var mı kontrol eder.
    /// Artık “occupied” durumunu da göz önüne alır.
    /// </summary>
    public bool IsClear()
    {
        if (occupied)
            return false;

        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.2f, obstacles);
        return (col == null);
    }

    /// <summary>
    /// Tile'ı vurgular ve walkable hale getirir.
    /// </summary>
    public void Highlight()
    {
        rend.color = highlightedColor;
        walkable = true;
    }

    /// <summary>
    /// Tile'ı varsayılan haline döndürür.
    /// </summary>
    public void ResetTile()
    {
        rend.color = Color.white;
        walkable = false;
        isCreatable = false;
    }

    /// <summary>
    /// Tile'ı üretime uygun hale getirir.
    /// </summary>
    public void SetCreatable()
    {
        rend.color = creatableColor;
        isCreatable = true;
    }

    /// <summary>
    /// Tile'ı işgalde (occupied) ya da boş (not occupied) olarak işaretler.
    /// Renk değiştirme kaldırıldı; tile'ın varsayılan rengi korunuyor.
    /// </summary>
    public void SetOccupied(bool isOccupied)
    {
        occupied = isOccupied;
        walkable = !isOccupied;
        // Renk değiştirme kısmı kaldırıldı, böylece tile rengi sabit kalır.
        // İsterseniz aşağıdaki satırları düzenleyebilirsiniz:
        // if (occupied)
        //     rend.color = Color.red;
        // else
        //     rend.color = Color.white;
    }

    private void OnMouseEnter()
    {
        if (IsClear())
        {
            if (audioSource != null)
                audioSource.Play();
            if (!isScaled)
            {
                transform.localScale += new Vector3(scaleIncreaseAmount, scaleIncreaseAmount, scaleIncreaseAmount);
                isScaled = true;
            }
        }
    }

    private void OnMouseExit()
    {
        if (isScaled)
        {
            transform.localScale -= new Vector3(scaleIncreaseAmount, scaleIncreaseAmount, scaleIncreaseAmount);
            isScaled = false;
        }
    }
}
