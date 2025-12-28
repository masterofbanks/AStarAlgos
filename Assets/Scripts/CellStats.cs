using UnityEngine;

public class CellStats : MonoBehaviour
{
    public CellStats parent;
    public float F;
    public float G;
    public float H;
    public bool Walkable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            //GetComponent<SpriteRenderer>().enabled = true;
            Walkable = false;
        }
    }

}
