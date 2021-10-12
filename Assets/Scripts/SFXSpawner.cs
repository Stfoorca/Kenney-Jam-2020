using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXSpawner : MonoBehaviour
{
    public static SFXSpawner instance;


    public bool spawnEmptyParticleObjects = false;
    public Material sheetParticleMat;
    public GameObject fakeParticlePrefab;
    public GameObject sheetExplosionPrefab;
    public GameObject heartExplosionPrefab;
    public GameObject bloodExplosionPrefab;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        Lean.Pool.LeanPool.Preload(sheetExplosionPrefab, 20);
        Lean.Pool.LeanPool.Preload(bloodExplosionPrefab, 20);
    }

    public void SpawnHeartExplosion(Vector2 position)
    {
        if(spawnEmptyParticleObjects) Lean.Pool.LeanPool.Spawn(fakeParticlePrefab).transform.position = position;
        else
        {
            Lean.Pool.LeanPool.Spawn(heartExplosionPrefab).transform.position = position;

        }
        //Instantiate(heartExplosionPrefab).transform.position = position;
    }

    public void SpawnBloodExplosion(Vector2 position)
    {
        if (spawnEmptyParticleObjects) Lean.Pool.LeanPool.Spawn(fakeParticlePrefab).transform.position = position;
        else
        {
            Lean.Pool.LeanPool.Spawn(bloodExplosionPrefab).transform.position = position;
        }
            //Instantiate(bloodExplosionPrefab).transform.position = position;
    }

    public void SpawnSheetExplosion(Vector2 position, Sprite sprite, Color color, Vector2 direction, float arc, float radius = 0, float speedMultiplier = 1)
    {
        //ParticleSystem ps = Instantiate(sheetExplosionPrefab).GetComponent<ParticleSystem>();
        if (spawnEmptyParticleObjects) Lean.Pool.LeanPool.Spawn(fakeParticlePrefab).transform.position = position;
        else
        {
            ParticleSystem ps = Lean.Pool.LeanPool.Spawn(sheetExplosionPrefab).GetComponentInChildren<ParticleSystem>();

            ps.transform.position = position;
            var main = ps.main;
            main.startColor = color;
            main.startSpeedMultiplier = speedMultiplier;

            var shape = ps.shape;
            shape.arc = arc;
            shape.radius = radius;
            shape.radiusThickness = 1;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            shape.rotation = new Vector3(0, 0, (angle - arc / 2));

            Material mat = Instantiate(sheetParticleMat);
            Texture2D t = TextureFromSprite(sprite);
            t.filterMode = FilterMode.Point;
            mat.mainTexture = t;

            var psr = ps.GetComponent<Renderer>();
            psr.material = mat;
        }
    }

    public static Texture2D TextureFromSprite(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }
}
