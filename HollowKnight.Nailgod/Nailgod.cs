namespace Nailgod;
public class Settings
{
    public bool on = true;
}
public class Nailgod : Mod, IGlobalSettings<Settings>, IMenuMod
{
    public static Nailgod nailgod;
    Settings settings_ = new();
    public bool ToggleButtonInsideMenu => true;
    Texture2D skin;
    Storm storm = new();
    public Nailgod() : base("Nailgod")
    {
        nailgod = this;
    }
    public override string GetVersion() => "1.0.0.0";
    public override List<(string, string)> GetPreloadNames()
    {
        return new List<(string, string)> {
            ("GG_Radiance","Boss Control")
        };
    }
    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ActiveSceneChanged;
        ModHooks.HeroUpdateHook += HeroUpdateHook;
        On.PlayMakerFSM.OnEnable += PlayMakerFSMOnEnable;
        var stream = typeof(Nailgod).Assembly.GetManifestResourceStream("Nailgod.Skin.png");
        MemoryStream memoryStream = new((int)stream.Length);
        stream.CopyTo(memoryStream);
        stream.Close();
        var bytes = memoryStream.ToArray();
        memoryStream.Close();
        skin = new(0, 0);
        skin.LoadImage(bytes, true);
        storm.Initialize(preloadedObjects);
    }
    public void OnLoadGlobal(Settings settings) => settings_ = settings;
    public Settings OnSaveGlobal() => settings_;
    public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? menu)
    {
        List<IMenuMod.MenuEntry> menus = new();
        menus.Add(
            new()
            {
                Values = new string[]
                {
                    Language.Language.Get("MOH_ON", "MainMenu"),
                    Language.Language.Get("MOH_OFF", "MainMenu"),
                },
                Saver = i => settings_.on = i == 0,
                Loader = () => settings_.on ? 0 : 1
            }
        );
        return menus;
    }
    private void ActiveSceneChanged(UnityEngine.SceneManagement.Scene previous, UnityEngine.SceneManagement.Scene current)
    {
        if (!settings_.on)
        {
            return;
        }
        if (current.name == "GG_Sly")
        {
            var slyBoss = GameObject.Find("Sly Boss").gameObject;
            var tk2dSprite = slyBoss.GetComponent<tk2dSprite>();
            tk2dSprite.CurrentSprite.material.mainTexture = skin;
            foreach (var gameObject in current.GetAllGameObjects())
            {
                var destroy = false;
                if (gameObject.name == "Godseeker Crowd")
                {
                    destroy = true;
                }
                if (gameObject.name == "throne")
                {
                    destroy = true;
                }
                if (gameObject.name.StartsWith("bg_pillar"))
                {
                    destroy = true;
                }
                if (gameObject.name.StartsWith("GG_crowd"))
                {
                    destroy = true;
                }
                if (gameObject.name.StartsWith("GG_step"))
                {
                    destroy = true;
                }
                if (gameObject.name == "black_fader_GG (9)" && gameObject.name == "black_fader_GG (10)")
                {
                    destroy = true;
                }
                if (destroy)
                {
                    UnityEngine.Object.Destroy(gameObject);
                }
                if (gameObject.name == "gg_incense" || gameObject.name == "gg_incense (1)" || gameObject.name == "gg_incense (2)")
                {
                    var p = gameObject.transform.position;
                    gameObject.transform.position = new Vector3(p.x - 0.5f, p.y, p.z);
                }
            }
        }
    }
    private void HeroUpdateHook()
    {
        if (!settings_.on)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GG_Sly");
        }
    }
    private void PlayMakerFSMOnEnable(On.PlayMakerFSM.orig_OnEnable onEnable, PlayMakerFSM fsm)
    {
        if (!settings_.on)
        {
            onEnable(fsm);
            return;
        }
        var gameObject = fsm.gameObject;
        if (gameObject.scene.name == "GG_Sly" && gameObject.name == "Sly Boss" && fsm.FsmName == "Control")
        {
            fsm.InsertCustomAction("Idle", () =>
            {
                fsm.SetState("Storm/Start");
            }, 0);
            storm.UpdateFSM(fsm);
        }
        if (gameObject.scene.name == "GG_Sly" && gameObject.name.StartsWith("Radiant Spike") && fsm.FsmName == "Hero Saver")
        {
            fsm.InsertCustomAction("Send", () =>
            {
                HeroController.instance.damageMode = GlobalEnums.DamageMode.FULL_DAMAGE;
                fsm.SendEvent("FINISHED");
            }, 0);
        }
        onEnable(fsm);
    }

}
