using UnityEngine;

public class VegetationGenerator : MonoBehaviour
{
    [Header("Vegetation Settings")]
    public GameObject[] treePrefabs;
    public int treeCount = 15;

    [Header("Terrain (FBX) Reference")]
    public Transform terrainTransform;
    public MeshRenderer terrainRenderer;

    [Header("Raycast Settings")]
    public float raycastHeight = 200f;

    [Header("Spacing Settings")]
    public float treeSpacing = 2.5f;

    [Header("Layer Masks")]
    public LayerMask terrainLayer;

    private int placedTrees;

    void Start()
    {
        GenerateTrees();
    }

    void GenerateTrees()
    {
        placedTrees = 0;
        int attempts = 0;

        Bounds bounds = terrainRenderer.bounds;

        while (placedTrees < treeCount && attempts < 1000)
        {
            attempts++;

            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);

            Vector3 rayOrigin = new Vector3(
                randomX,
                bounds.max.y + raycastHeight,
                randomZ
            );

            Debug.DrawRay(rayOrigin, Vector3.down * (raycastHeight * 2f), Color.red, 2f);

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastHeight * 2f, terrainLayer))
            {
                Vector3 spawnPosition = hit.point;

                Collider[] overlaps = Physics.OverlapSphere(spawnPosition, treeSpacing);

                bool canSpawn = true;

                foreach (Collider col in overlaps)
                {
                    if (col.gameObject.layer == LayerMask.NameToLayer("Vegetation"))
                    {
                        canSpawn = false;
                        break;
                    }
                }

                if (canSpawn)
                {
                    Instantiate(
                        treePrefabs[Random.Range(0, treePrefabs.Length)],
                        spawnPosition,
                        Quaternion.Euler(0, Random.Range(0, 360f), 0)
                    );

                    placedTrees++;
                }
            }
        }

        Debug.Log("Trees Generated: " + placedTrees);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, treeSpacing);
    }
}
