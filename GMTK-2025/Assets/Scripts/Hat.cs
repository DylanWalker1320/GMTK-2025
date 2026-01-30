using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HatSpriteLayerUpdater))]
public class Hat : MonoBehaviour
{
    public GeneratedHat hatData;
    public int hatSpriteMinLayer;
    public bool playerHat = false;
    private GameObject hatBelow;
    public int hatNumber;
    private static readonly float moveThreshold = 0.01f;
    private static readonly float lerpSpeed = 50f;
    public static readonly float yDifference = 0.3f;
    public static readonly float initialYOffset = 0.52f;
    private static GameObject player;
    private bool isFirstHat => hatNumber == 0;
    private Rigidbody2D playerRb;
    private HatSpriteLayerUpdater spriteLayerUpdater;
    public GameObject hatVisuals;
    public GameObject hatShadow;

    void Start()
    {
        spriteLayerUpdater = GetComponent<HatSpriteLayerUpdater>();
        spriteLayerUpdater.ApplyComponents(hatData.components);
        spriteLayerUpdater.UpdateSpriteLayers();
    }

    void Update()
    {
        if (playerHat) // Could be replaced with a return, left as an if for posterity
        {     
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }

            if (playerRb == null)
            {
                playerRb = player.GetComponent<Rigidbody2D>();
            }

            // Get the speed of the player
            float playerSpeed = playerRb.linearVelocity.magnitude;
            if(hatBelow != null)
            {
                // Move in the inverse direction of the player's movement to create a bobbing effect, and multiply by the hat number
                if (isFirstHat)
                {
                    transform.position = new Vector3(player.transform.position.x,
                                                hatBelow.transform.position.y + yDifference, 
                                                0);
                    transform.position += (Vector3) HatPositioner.currentOffset;
                } else
                {
                    transform.position = new Vector3(player.transform.position.x - playerRb.linearVelocity.x * 0.01f * (hatNumber + 1) + Mathf.Sin(Time.time * 5f + hatNumber) * 0.01f * (hatNumber + 1),
                                        hatBelow.transform.position.y + yDifference, 
                                        0);
                }            
            }

            // Flip the player to face the movement direction
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            hatVisuals.transform.localScale = new Vector3(playerMovement.facingRight ? -1 : 1, 1, 1);
            hatShadow.transform.localScale = new Vector3(playerMovement.facingRight ? -1 : 1, 1, 1);
        }
    }

    // Only runs on hat spawn
    IEnumerator MoveHat()
    {
        if (hatBelow != null)
        {
            Vector3 targetPosition = hatBelow.transform.position + new Vector3(0, yDifference, 0);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
        }
        yield return null;
    }

    public void Initialize(GeneratedHat data, bool applyStats = true)
    {
        hatData = data;

        if (playerHat)
        {
            if (Vector3.Distance(transform.position, hatBelow.transform.position + new Vector3(0, yDifference, 0)) > moveThreshold)
            {
                // Clamp the y position to be above the hat below
                transform.position = new Vector3(transform.position.x, hatBelow.transform.position.y + yDifference, transform.position.z);
                StartCoroutine(MoveHat());
            }

            if (applyStats)
            {
                ApplyStats();
            }
        }
    }

    private void ApplyStats()
    {

        PlayerMovement player = FindFirstObjectByType<PlayerMovement>();

        foreach (var stat in hatData.stats)
        {
            switch (stat.type)
            {
                case StatType.Speed:
                    player.maxSpeed += stat.value;
                    break;

                case StatType.Health:
                    player.health += stat.value;
                    player.maxHealth += stat.value;
                    break;

                case StatType.CastSpeed:
                    player.castSpeed += stat.value;
                    break;

                case StatType.CastStrength:
                    player.castStrength += stat.value;
                    break;

                case StatType.SpellLevel:
                    if (stat.spellBonus != null)
                    {
                        // Upgrade the specific spell
                        for (int i = 0; i < stat.spellBonus.levelBonus; i++)
                        {
                            Spell.UpgradeSpell(stat.spellBonus.spell);
                        }
                    }
                    break;
            }
        }
        player.UpdateUI();
    }
    
    public void SetHatBelow(GameObject gameObject)
    {
        hatBelow = gameObject;
    }
}