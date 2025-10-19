using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour
{
    [Header("Audio")]
    public float explosionVolume = 1f; 

    [Header("Explosion")]
    public float delay = 3f;                 // seconds before auto-explode
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public float upModifier = 1f;
    public LayerMask affectLayers = ~0;      // which layers are affected by explosion

    [Header("Optional")]
    public GameObject explosionPrefab;       // optional VFX prefab to instantiate
    public AudioClip explosionSound;         // optional SFX

    private bool exploded = false;

    void Start()
    {
        // Auto-explode after delay if not exploded earlier
        if (delay > 0f) Invoke(nameof(Explode), delay);
    }

    void OnCollisionEnter(Collision other)
    {
        // Explode on first collision
        if (!exploded) {
            Explode();
        }
    }

    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        // spawn VFX
        if (explosionPrefab != null) {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // play sound globally (2D, not positional)
        if (explosionSound != null) {
            GameObject audioObj = new GameObject("ExplosionSound");
            AudioSource audioSource = audioObj.AddComponent<AudioSource>();
            audioSource.clip = explosionSound;
            audioSource.volume = explosionVolume;
            audioSource.spatialBlend = 0f; // 0 = 2D (global), 1 = 3D (positional)
            audioSource.Play();
            Destroy(audioObj, explosionSound.length);
        }

        // apply physics explosion
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, affectLayers);
        foreach (Collider c in colliders) {
            Rigidbody rb = c.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upModifier, ForceMode.Impulse);
            }

            // optional: damage destructible objects by calling an interface or method
            var destruct = c.GetComponent<IDestructible>();
            if (destruct != null) {
                destruct.ApplyDamage(1); // simple 1-damage; you can expand
            }
        }

        // destroy bomb object
        Destroy(gameObject);
    }
}

public interface IDestructible {
    void ApplyDamage(int amount);
}
