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
        Spell.UpgradeSpell(Spell.Spells.Fireball);
        fireLevelText.text = Spell.GetSpellLevel(Spell.Spells.Fireball).ToString();
        SubtractExperience(basicSpellCost);
        
    }
    public void UpgradeWaterball()
    {
        Spell.UpgradeSpell(Spell.Spells.Waterball);
        waterLevelText.text = Spell.GetSpellLevel(Spell.Spells.Waterball).ToString();
        SubtractExperience(basicSpellCost);
        
    }
    public void UpgradeLightning()
    {
        Spell.UpgradeSpell(Spell.Spells.Lightning);
        lightLevelText.text = Spell.GetSpellLevel(Spell.Spells.Lightning).ToString();
        SubtractExperience(basicSpellCost);
        
    }
    public void UpgradeDark()
    {
        Spell.UpgradeSpell(Spell.Spells.Dark);
        darkLevelText.text = Spell.GetSpellLevel(Spell.Spells.Dark).ToString();
        SubtractExperience(basicSpellCost);
        
    }
    public void UpgradeExplosiveShot()
    {
        Spell.UpgradeSpell(Spell.Spells.ExplosiveShot);
        esLevelText.text = Spell.GetSpellLevel(Spell.Spells.ExplosiveShot).ToString();
        SubtractExperience(bigSpellCost);
        
    }
    public void UpgradeSteamVent()
    {
        Spell.UpgradeSpell(Spell.Spells.SteamVent);
        svLevelText.text = Spell.GetSpellLevel(Spell.Spells.SteamVent).ToString();
        SubtractExperience(bigSpellCost);
        
    }
    public void UpgradeFissureFlare()
    {
        Spell.UpgradeSpell(Spell.Spells.FissureFlare);
        ffLevelText.text = Spell.GetSpellLevel(Spell.Spells.FissureFlare).ToString();
        SubtractExperience(bigSpellCost);
        
    }
    public void UpgradeGhostFlame()
    {
        Spell.UpgradeSpell(Spell.Spells.Ghostflame);
        ghLevelText.text = Spell.GetSpellLevel(Spell.Spells.Ghostflame).ToString();
        SubtractExperience(bigSpellCost);
        
    }
    public void UpgradeWave()
    {
        Spell.UpgradeSpell(Spell.Spells.Wave);
        waveLevelText.text = Spell.GetSpellLevel(Spell.Spells.Wave).ToString();
        SubtractExperience(bigSpellCost);
        
    }
    public void UpgradeChainLightning()
    {
        Spell.UpgradeSpell(Spell.Spells.ChainLightning);
        clLevelText.text = Spell.GetSpellLevel(Spell.Spells.ChainLightning).ToString();
        SubtractExperience(bigSpellCost);
        
    }
    public void UpgradePoisonPuddles()
    {
        Spell.UpgradeSpell(Spell.Spells.PoisonPuddle);
        ppLevelText.text = Spell.GetSpellLevel(Spell.Spells.PoisonPuddle).ToString();
        SubtractExperience(bigSpellCost);
        
    }
    public void UpgradeStorm()
    {
        Spell.UpgradeSpell(Spell.Spells.Storm);
        stLevelText.text = Spell.GetSpellLevel(Spell.Spells.Storm).ToString();
        SubtractExperience(bigSpellCost);
        
    }
    public void UpgradeBlackFlash()
    {
        Spell.UpgradeSpell(Spell.Spells.BlackFlash);
        bfLevelText.text = Spell.GetSpellLevel(Spell.Spells.BlackFlash).ToString();
        SubtractExperience(bigSpellCost);
        
    }
    public void UpgradeBlackHole()
    {
        Spell.UpgradeSpell(Spell.Spells.BlackHole);
        bhLevelText.text = Spell.GetSpellLevel(Spell.Spells.BlackHole).ToString();
        SubtractExperience(bigSpellCost);

    }

    void SubtractExperience(int cost)
    {
        if (player.experience - cost >= 0)
        {
            player.experience -= cost;
        }
    }
        
}
