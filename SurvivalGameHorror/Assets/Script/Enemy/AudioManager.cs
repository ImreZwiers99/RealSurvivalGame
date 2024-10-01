using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public Dictionary<Texture, AudioClip[]> footstepSounds = new Dictionary<Texture, AudioClip[]>();

    public AudioSource footstepAudioSource;

    public List<Terrain> terrains;

    public AudioClip[] grassFootstepSounds;

    public AudioClip[] rockFootstepSounds;

    public AudioClip[] sandFootstepSounds;

    private AudioClip selectedSound;

    public Texture grassTexture;
    public Texture rockTexture;
    public Texture sandTexture;

    void Start()
    {
        footstepSounds.Add(grassTexture, grassFootstepSounds);
        footstepSounds.Add(rockTexture, rockFootstepSounds);
        footstepSounds.Add(sandTexture, sandFootstepSounds);
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            Texture currentTexture = GetTextureAtPoint(hit.point);

            if (footstepSounds.ContainsKey(currentTexture))
            {
                AudioClip[] sounds = footstepSounds[currentTexture];
                int randomIndex = Random.Range(0, sounds.Length);
                selectedSound = sounds[randomIndex];
            }
        }
    }

    private Texture GetTextureAtPoint(Vector3 point)
    {
        foreach (Terrain terrain in terrains)
        {
            TerrainData terrainData = terrain.terrainData;
            int mapX = Mathf.FloorToInt((point.x - terrain.transform.position.x) / terrainData.size.x * terrainData.alphamapWidth);
            int mapZ = Mathf.FloorToInt((point.z - terrain.transform.position.z) / terrainData.size.z * terrainData.alphamapHeight);
            float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

            int maxTextureIndex = 0;
            float maxTextureMix = 0;

            for (int i = 0; i < splatmapData.GetLength(2); i++)
            {
                if (splatmapData[0, 0, i] > maxTextureMix)
                {
                    maxTextureIndex = i;
                    maxTextureMix = splatmapData[0, 0, i];
                }
            }

            return terrainData.splatPrototypes[maxTextureIndex].texture;
        }

        return null;
    }

    private void PlayFootstepSound()
    {
        footstepAudioSource.PlayOneShot(selectedSound);
    }
}
