using TMPro;
using UnityEngine;

public class SpellUpgradeUI : MonoBehaviour
{

    private GameManager gameManager; // Reference to the GameManager script
    private PlayerMovement player; // Reference to the PlayerMovement script
    private UIManager uiManager;
    [SerializeField] private TextMeshProUGUI experienceText;

    [Header("Spell Levels")]
    [SerializeField] private TextMeshProUGUI fireLevelText;
    [SerializeField] private TextMeshProUGUI waterLevelText;
    [SerializeField] private TextMeshProUGUI lightLevelText;
    [SerializeField] private TextMeshProUGUI darkLevelText;
    [SerializeField] private TextMeshProUGUI esLevelText;
    [SerializeField] private TextMeshProUGUI svLevelText;
    [SerializeField] private TextMeshProUGUI ffLevelText;
    [SerializeField] private TextMeshProUGUI ghLevelText;
    [SerializeField] private TextMeshProUGUI waveLevelText;
    [SerializeField] private TextMeshProUGUI clLevelText;
    [SerializeField] private TextMeshProUGUI ppLevelText;
    [SerializeField] private TextMeshProUGUI stLevelText;
    [SerializeField] private TextMeshProUGUI bfLevelText;
    [SerializeField] private TextMeshProUGUI bhLevelText;

    private int bigSpellCost = 250;
    private int basicSpellCost = 50;


    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        player = FindFirstObjectByType<PlayerMovement>();
        uiManager = FindFirstObjectByType<UIManager>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateSpellUILevels();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSpellUILevels()
    {
        fireLevelText.text = Spell.GetSpellLevel(Spell.Spells.Fireball).ToString();
        waterLevelText.text = Spell.GetSpellLevel(Spell.Spells.Waterball).ToString();
        lightLevelText.text = Spell.GetSpellLevel(Spell.Spells.Lightning).ToString();
        darkLevelText.text = Spell.GetSpellLevel(Spell.Spells.Dark).ToString();
        esLevelText.text = Spell.GetSpellLevel(Spell.Spells.ExplosiveShot).ToString();
        svLevelText.text = Spell.GetSpellLevel(Spell.Spells.SteamVent).ToString();
        ffLevelText.text = Spell.GetSpellLevel(Spell.Spells.FissureFlare).ToString();
        ghLevelText.text = Spell.GetSpellLevel(Spell.Spells.Ghostflame).ToString();
        waveLevelText.text = Spell.GetSpellLevel(Spell.Spells.Wave).ToString();
        clLevelText.text = Spell.GetSpellLevel(Spell.Spells.ChainLightning).ToString();
        ppLevelText.text = Spell.GetSpellLevel(Spell.Spells.PoisonPuddle).ToString();
        stLevelText.text = Spell.GetSpellLevel(Spell.Spells.Storm).ToString();
        bfLevelText.text = Spell.GetSpellLevel(Spell.Spells.BlackFlash).ToString();
        bhLevelText.text = Spell.GetSpellLevel(Spell.Spells.BlackHole).ToString();
    }

    public void UpdateExperience()
    {
        experienceText.text = player.experience.ToString();
    }


    // void UpgradeFireElement()
    // {
    //     Spell.UpgradeModifier(Spell.SpellType.Fire, 0.2f); //20% increase to dark type spells
    // }
    // void UpgradeWaterElement()
    // {
    //     Spell.UpgradeModifier(Spell.SpellType.Water, 0.2f); //20% increase to dark type spells
    // }
    // void UpgradeLightningElement()
    // {
    //     Spell.UpgradeModifier(Spell.SpellType.Lightning, 0.2f); //20% increase to dark type spells
    // }
    // void UpgradeDarkElement()
    // {
    //     Spell.UpgradeModifier(Spell.SpellType.Dark, 0.2f); //20% increase to dark type spells
    // }

    public void UpgradeFireball()
    {
        SubtractExperience(basicSpellCost, Spell.Spells.Fireball);
        fireLevelText.text = Spell.GetSpellLevel(Spell.Spells.Fireball).ToString();
        
    }
    public void UpgradeWaterball()
    {
        SubtractExperience(basicSpellCost, Spell.Spells.Waterball);
        waterLevelText.text = Spell.GetSpellLevel(Spell.Spells.Waterball).ToString();
        
    }
    public void UpgradeLightning()
    {
        SubtractExperience(basicSpellCost, Spell.Spells.Lightning);
        lightLevelText.text = Spell.GetSpellLevel(Spell.Spells.Lightning).ToString();
        
    }
    public void UpgradeDark()
    {
        SubtractExperience(basicSpellCost, Spell.Spells.Dark);
        darkLevelText.text = Spell.GetSpellLevel(Spell.Spells.Dark).ToString();
    }
    public void UpgradeExplosiveShot()
    {
        SubtractExperience(bigSpellCost, Spell.Spells.ExplosiveShot);
        esLevelText.text = Spell.GetSpellLevel(Spell.Spells.ExplosiveShot).ToString();
        
    }
    public void UpgradeSteamVent()
    {
        SubtractExperience(bigSpellCost, Spell.Spells.SteamVent);
        svLevelText.text = Spell.GetSpellLevel(Spell.Spells.SteamVent).ToString();
        
    }
    public void UpgradeFissureFlare()
    {
        SubtractExperience(bigSpellCost, Spell.Spells.FissureFlare);
        ffLevelText.text = Spell.GetSpellLevel(Spell.Spells.FissureFlare).ToString();
        
    }
    public void UpgradeGhostFlame()
    {
        SubtractExperience(bigSpellCost, Spell.Spells.Ghostflame);
        ghLevelText.text = Spell.GetSpellLevel(Spell.Spells.Ghostflame).ToString();
        
    }
    public void UpgradeWave()
    {
        SubtractExperience(bigSpellCost, Spell.Spells.Wave);
        waveLevelText.text = Spell.GetSpellLevel(Spell.Spells.Wave).ToString();
        
    }
    public void UpgradeChainLightning()
    {
        SubtractExperience(bigSpellCost, Spell.Spells.ChainLightning);
        clLevelText.text = Spell.GetSpellLevel(Spell.Spells.ChainLightning).ToString();
        
    }
    public void UpgradePoisonPuddles()
    {
        SubtractExperience(bigSpellCost, Spell.Spells.PoisonPuddle);
        ppLevelText.text = Spell.GetSpellLevel(Spell.Spells.PoisonPuddle).ToString();
        
    }
    public void UpgradeStorm()
    {
        SubtractExperience(bigSpellCost, Spell.Spells.Storm);
        stLevelText.text = Spell.GetSpellLevel(Spell.Spells.Storm).ToString();
        
    }
    public void UpgradeBlackFlash()
    {
        SubtractExperience(bigSpellCost, Spell.Spells.BlackFlash);
        bfLevelText.text = Spell.GetSpellLevel(Spell.Spells.BlackFlash).ToString();
        
    }
    public void UpgradeBlackHole()
    {
        SubtractExperience(bigSpellCost, Spell.Spells.BlackHole);
        bhLevelText.text = Spell.GetSpellLevel(Spell.Spells.BlackHole).ToString();


    }

    void SubtractExperience(int cost, Spell.Spells spell)
    {
        if (player.experience - cost >= 0)
        {
            Spell.UpgradeSpell(spell);
            player.experience -= cost;
        }
    }
        
}
