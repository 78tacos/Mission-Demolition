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
        // A delay of 0 leaves the timer off; collision still detonates.
        if (delay > 0f) Invoke(nameof(Explode), delay);
    }

    void OnCollisionEnter(Collision other)
    {
        if (!exploded) {
            Explode();
        }
    }

    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explosionPrefab != null) {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (explosionSound != null) {
            GameObject audioObj = new GameObject("ExplosionSound");
            AudioSource audioSource = audioObj.AddComponent<AudioSource>();
            audioSource.clip = explosionSound;
            audioSource.volume = explosionVolume;
            audioSource.spatialBlend = 0f; // 0 = 2D (global), 1 = 3D (positional)
            audioSource.Play();
            Destroy(audioObj, explosionSound.length);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, affectLayers);
        foreach (Collider c in colliders) {
            Rigidbody rb = c.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upModifier, ForceMode.Impulse);
            }

            // Props opt in by implementing IDestructible.
            var destruct = c.GetComponent<IDestructible>();
            if (destruct != null) {
                destruct.ApplyDamage(1);
            }
        }

        Destroy(gameObject);
    }
}

public interface IDestructible {
    void ApplyDamage(int amount);
}
