using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    // Dictionary voor het mappen van texturenamen naar AudioClip-arrays voor voetstapgeluiden
    public Dictionary<Texture, AudioClip[]> footstepSounds = new Dictionary<Texture, AudioClip[]>();

    // AudioSource voor het afspelen van voetstapgeluiden
    public AudioSource footstepAudioSource;

    AudioClip selectedSound;

    // Het terrein waarop de speler zich bevindt
    public Terrain terrain;

    // Texturen om te controleren
    public Texture grassTexture;
    public Texture rockTexture;

    void Start()
    {
        // Voeg de texturen en bijbehorende voetstapgeluiden toe aan de dictionary
        footstepSounds.Add(grassTexture, grassFootstepSounds);
        footstepSounds.Add(rockTexture, rockFootstepSounds);
        // Voeg hier andere texturen en bijbehorende geluiden toe indien nodig
    }

    private void Update()
    {
        // Controleer welke texture de speler momenteel raakt
        int xCoordinate = Mathf.RoundToInt(transform.position.x / terrain.terrainData.size.x * terrain.terrainData.alphamapWidth);
        int zCoordinate = Mathf.RoundToInt(transform.position.z / terrain.terrainData.size.z * terrain.terrainData.alphamapHeight);

        // Alphamaps ophalen op de huidige positie
        float[,,] alphamaps = terrain.terrainData.GetAlphamaps(xCoordinate, zCoordinate, 1, 1);

        // Bepaal de index van de hoofdtextuur
        int mainTextureIndex = GetMainTexture(alphamaps);

        if (mainTextureIndex != -1 && mainTextureIndex < terrain.terrainData.alphamapLayers)
        {
            // Controleer of de index binnen het geldige bereik van de alphamap-lagen ligt
            Texture currentTexture = terrain.terrainData.splatPrototypes[mainTextureIndex].texture;

            if (footstepSounds.ContainsKey(currentTexture))
            {
                // Selecteer willekeurig een AudioClip uit de bijbehorende array
                AudioClip[] sounds = footstepSounds[currentTexture];
                int randomIndex = Random.Range(0, sounds.Length);
                selectedSound = sounds[randomIndex];
            }
        }
    }

    // Methode om de hoofdtextuur van een blend te bepalen
    private int GetMainTexture(float[,,] splatmapData)
    {
        float maxBlend = 0;
        int maxIndex = -1;

        for (int i = 0; i < splatmapData.GetLength(2); i++)
        {
            if (splatmapData[0, 0, i] > maxBlend)
            {
                maxBlend = splatmapData[0, 0, i];
                maxIndex = i;
            }
        }

        return maxIndex;
    }


    // Methode om een willekeurig voetstapgeluid af te spelen
    private void PlayFootstepSound()
    {
        footstepAudioSource.PlayOneShot(selectedSound);
    }

    // Hieronder worden de voetstapgeluiden voor elke texture gedefinieerd
    public AudioClip[] grassFootstepSounds;
    public AudioClip[] rockFootstepSounds;
    // Voeg hier andere geluiden toe indien nodig
}
