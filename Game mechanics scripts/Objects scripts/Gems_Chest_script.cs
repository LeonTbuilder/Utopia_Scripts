using UnityEngine;

public class Gems_Chest_script : MonoBehaviour
{

    public GameObject effect_Gems, effect_Explosion_Chest;

    [SerializeField] public bool isGrounded;

    RaycastHit2D _hit_Ground;

    // Start is called once before the first execution of FixedUpdate after the MonoBehaviour is created
    void Start()
    {
        if (Random.Range(0, 20) == 5 && effect_Gems != null)
        {
            GameObject _effect = Instantiate(effect_Gems, transform.position, transform.rotation);
            _effect.transform.parent = transform;
        }

        isGrounded = false;
    }

    // FixedUpdate is called once per frame
    void Update()
    {
        if (gameObject.CompareTag("Chest"))
            _hit_Ground = Physics2D.Raycast(
                new Vector3(transform.position.x,
                transform.position.y - 0.5f,
                transform.position.z)
            , Vector2.down, 1.0f, LayerMask.GetMask("Static"));
        else
            _hit_Ground = Physics2D.Raycast(
               new Vector3(transform.position.x,
               transform.position.y,
               transform.position.z)
           , Vector2.down, 1.0f, LayerMask.GetMask("Static"));
        if (_hit_Ground.collider != null)
        {
            if (_hit_Ground.collider.CompareTag( "Ground"))
                isGrounded = true;
            //Debug.Log("COIN HIT " + _hitGround.collider.name);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag( "Magma"))
        {
            Destroy(gameObject);
            GameObject _magma = Instantiate(effect_Explosion_Chest, transform.position, transform.rotation);
            Destroy(_magma, 1.0f);



            if (!Level_Script._instance._hero.isDead)
            {
                Level_Script._instance._hero.isDead = true;
                Game_Manager_Script.instance.Game_is_Over();
            }
        }
    }

}
