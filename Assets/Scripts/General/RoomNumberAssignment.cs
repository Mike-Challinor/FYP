using UnityEngine;

public class RoomNumberAssignment : MonoBehaviour
{
    SpriteRenderer sr;
    [SerializeField] private Sprite[] spriteArray;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogError("SpriteRenderer is missing on " + gameObject.name);
        }

        Vector3 position = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.5f, 0);
        transform.position = position;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSprite(int roomnumber)
    {
        if (spriteArray == null || spriteArray.Length == 0)
        {
            Debug.LogError("spriteArray is null or empty on " + gameObject.name);
            return;
        }

        if (roomnumber < 0 || roomnumber >= spriteArray.Length)
        {
            Debug.LogError("Invalid roomnumber index: " + roomnumber);
            return;
        }

        if (sr != null)
        {
            sr.sprite = spriteArray[roomnumber];
        }

        else
        {
            Debug.LogError("SpriteRenderer is missing on " + gameObject.name);
        }

        
    }

}
