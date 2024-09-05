using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest_Script : MonoBehaviour
{
    public Animator animator;
    public GameObject lava_explosion_Effect;
    public GameObject poison_explosion_Effect;

    string Anim_Open_Trigger = "Open_Chest";

    [Header("Gem Prefabs and Values")]
    public List<Gem_Data_Script> gems;

    [Header("Target Gem Value for Chest")]
    public int targetGemValue;

    private List<GameObject> spawnedGems = new List<GameObject>();
    private Transform heroTransform;
    private bool isMovingToHero = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hero"))
        {
            heroTransform = other.transform;
            animator.SetTrigger(Anim_Open_Trigger);
            StartCoroutine(OpenChestCoroutine());
        }

        if (other.CompareTag("Lava"))
        {
            Handle_Destroyed_Target();
            InstantiateExplosionEffect(lava_explosion_Effect);
        }

        if (other.CompareTag("Poison"))
        {
            Handle_Destroyed_Target();
            InstantiateExplosionEffect(poison_explosion_Effect);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Chest":
                {
                    heroTransform = collision.transform;
                    animator.SetTrigger(Anim_Open_Trigger);
                    StartCoroutine(OpenChestCoroutine());
                }
                break;
        }
    }

    private void Handle_Destroyed_Target()
    {
        Destroy(gameObject);

        if (Game_Manager_Script.instance.total_Energy > 0)
        {
            Game_Manager_Script.instance.total_Energy--;
            PlayerPrefs.SetInt("Heart", Game_Manager_Script.instance.total_Energy);
        }

        if (Levels_Manager_Script._instance.GetCurrentLevelType() == Level_Script.Type_ofLEVEL.CHEST)
        {
            Hero_Script hero = FindFirstObjectByType<Hero_Script>();
            if (hero != null)
            {
                hero.Play_Hero_Death_Animation();
            }
        }

        Game_Manager_Script.instance.Game_is_Over();
        Energy_Manager_Script._instance.UpdateEnergy(Game_Manager_Script.instance.total_Energy);
    }

    private void InstantiateExplosionEffect(GameObject explosionPrefab)
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }
    }

    private IEnumerator OpenChestCoroutine()
    {
        yield return new WaitForSeconds(1.5f);

        SpawnGems();

        yield return new WaitForSeconds(0.4f);

        isMovingToHero = true;
        SetGemsTrigger(true);
        StartCoroutine(DestroyGemsAfterDelay(0.4f));
    }

    private void Update()
    {
        if (isMovingToHero && heroTransform != null)
        {
            MoveGemsTowardsHero();
        }
    }

    void SpawnGems()
    {
        int remainingValue = targetGemValue;

        while (remainingValue > 0)
        {
            Gem_Data_Script selectedGem = SelectRandomGem(remainingValue);
            if (selectedGem != null && selectedGem.gemPrefab != null)
            {
                Vector3 randomPosition = transform.position;
                randomPosition.x += Random.Range(-0.25f, 0.25f);
                randomPosition.y += Random.Range(-0.25f, 0.25f);

                GameObject gemInstance = Instantiate(selectedGem.gemPrefab, randomPosition, Quaternion.identity);
                spawnedGems.Add(gemInstance);
                gemInstance.transform.SetParent(transform.parent);
                remainingValue -= selectedGem.gemValue;
            }
            else
            {
                break;
            }
        }
    }

    Gem_Data_Script SelectRandomGem(int remainingValue)
    {
        List<Gem_Data_Script> validGems = gems.FindAll(gem => gem.gemValue <= remainingValue && gem.gemPrefab != null);
        if (validGems.Count == 0)
        {
            return null;
        }
        return validGems[Random.Range(0, validGems.Count)];
    }

    private void MoveGemsTowardsHero()
    {
        foreach (GameObject gem in spawnedGems)
        {
            if (gem != null)
            {
                Vector3 targetPosition = heroTransform.position + new Vector3(0, 0.5f, 0);
                float distance = Vector3.Distance(gem.transform.position, targetPosition);
                if (distance > 0.1f)
                {
                    gem.transform.position = Vector3.MoveTowards(gem.transform.position, targetPosition, 5f * Time.deltaTime);
                }
            }
        }
    }

    private void SetGemsTrigger(bool isTrigger)
    {
        foreach (GameObject gem in spawnedGems)
        {
            if (gem != null)
            {
                Collider[] gemColliders = gem.GetComponents<Collider>();
                foreach (Collider gemCollider in gemColliders)
                {
                    gemCollider.isTrigger = isTrigger;
                }
            }
        }
    }

    private IEnumerator DestroyGemsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyGems();
    }

    private void DestroyGems()
    {
        foreach (GameObject gem in spawnedGems)
        {
            if (gem != null)
            {
                Destroy(gem);
            }
        }
        spawnedGems.Clear();
        Destroy(gameObject);
    }
}
