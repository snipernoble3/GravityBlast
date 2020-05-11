using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour {

    [HideInInspector] public God god;
    [SerializeField] GameObject surface;
    [SerializeField] GameObject modificationChestPrefab;
    GameObject modificationChest;

    private void Awake () {
        //GenerateChest();
    }

    public void GenerateChest () {
        modificationChest = Instantiate(modificationChestPrefab, surface.transform, true);
        modificationChest.GetComponent<ModificationChest>().player = god.player;

        Vector3 point = RandomSpawnPoint();
        Vector3 dir = (point - surface.transform.position).normalized * 9.8f;

        modificationChest.transform.SetPositionAndRotation(point, Quaternion.LookRotation(dir));
        modificationChest.transform.Rotate(new Vector3(90, 0, 0), Space.Self);

    }

    public Vector3 RandomSpawnPoint (float distanceBuffer = 0f) {
        Vector3 spawnPoint;
        //get random point on sphere larger than the surface
        Vector3 outer = surface.transform.position + (Random.onUnitSphere * GetComponent<SphereCollider>().radius);
        spawnPoint = outer;
        //direction to planet
        Vector3 dir = (outer - surface.transform.position).normalized * -9.8f;
        //raycast to point on planet
        RaycastHit[] hits;
        //Physics.Raycast(outer, dir, out hit, 100f);
        //Debug.DrawLine(outer, hit.point, Color.green, 2f);
        //Vector3 spawnPoint = hit.point;
        hits = Physics.RaycastAll(outer, dir, Vector3.Distance(outer, surface.transform.position));
        for (int i = 0; i < hits.Length; i++) {
            if (hits[i].transform == surface.transform) {
                spawnPoint = hits[i].point - (dir.normalized * distanceBuffer);
                //Debug.DrawLine(outer, hits[i].point, Color.green, 2f);
            }
        }
        //return value
        return spawnPoint;
    }
}
