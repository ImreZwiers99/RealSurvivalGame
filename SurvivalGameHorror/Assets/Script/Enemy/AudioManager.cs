using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    // Dictionary to map texture names to AudioClip arrays for footstep sounds
    public Dictionary<Texture, AudioClip[]> footstepSounds = new Dictionary<Texture, AudioClip[]>();

    // AudioSource for playing footstep sounds
    public AudioSource footstepAudioSource;

    // List of terrains the player can walk on
    public List<Terrain> terrains;

    // AudioClip array to store the footstep sounds for grass
    public AudioClip[] grassFootstepSounds;

    // AudioClip array to store the footstep sounds for rock
    public AudioClip[] rockFootstepSounds;

    // Currently selected footstep sound
    private AudioClip selectedSound;

    public Texture grassTexture;
    public Texture rockTexture;

    void Start()
    {
        // Add textures and their corresponding footstep sounds to the dictionary
        footstepSounds.Add(grassTexture, grassFootstepSounds);
        footstepSounds.Add(rockTexture, rockFootstepSounds);
        // Add other textures and their sounds here if needed
    }

    private void Update()
    {
        // Check which texture the player is currently walking on
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.5f))
        {
            Texture currentTexture = GetTextureAtPoint(hit.point);

            if (footstepSounds.ContainsKey(currentTexture))
            {
                // Select a random AudioClip from the corresponding array
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

    // Method to play a footstep sound
    private void PlayFootstepSound()
    {
        footstepAudioSource.PlayOneShot(selectedSound);
    }
}
