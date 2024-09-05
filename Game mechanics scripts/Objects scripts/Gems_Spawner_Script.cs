using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gems_Spawner_Script : MonoBehaviour
{
    [Header("Gem Prefabs and Values")]
    public List<Gem_Data_Script> gems;

    [Header("Target Gem Value for Level")]
    public int targetGemValue;

    private List<GameObject> spawnedGems = new List<GameObject>();
    private Transform heroTransform;
    private bool isMovingToHero = false;

    private void Awake()
    {
        targetGemValue = Mathf.Clamp(targetGemValue, 0, 100);
        SpawnGems();
    }

    void Update()
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
                randomPosition.y += Random.Range(-0.5f, 0f);

                GameObject gemInstance = Instantiate(selectedGem.gemPrefab, randomPosition, Quaternion.identity);
                spawnedGems.Add(gemInstance);
                gemInstance.transform.SetParent(transform.parent);
                gemInstance.GetComponent<Gem_Script>().spawnerScript = this;
                gemInstance.GetComponent<Gem_Script>().spawnerPosition = transform.position; // Set the spawner position
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

    public void MoveGemsTowardsHero(Transform hero)
    {
        heroTransform = hero;
        isMovingToHero = true;
        SetGemsTrigger(true);
        StartCoroutine(DestroyGemsAfterDelay(0.4f));
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
