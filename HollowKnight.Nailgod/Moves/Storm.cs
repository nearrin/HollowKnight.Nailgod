namespace Nailgod;
public class Storm
{
    GameObject radiantSpikeTemplate;
    List<GameObject> radiantSpikes;
    public void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        var bossControl = preloadedObjects["GG_Radiance"]["Boss Control"];
        var spikeControl = bossControl.transform.Find("Spike Control").gameObject;
        var farL = spikeControl.transform.Find("Far L").gameObject;
        radiantSpikeTemplate = farL.transform.Find("Radiant Spike").gameObject;
    }
    public void UpdateFSM(PlayMakerFSM fsm)
    {
        var sly = fsm.gameObject;
        var hero = HeroController.instance.gameObject;
        var dSlashEffect = sly.transform.Find("DSlash Effect");
        var stormSlashEffect = UnityEngine.Object.Instantiate(dSlashEffect, sly.transform);
        stormSlashEffect.name = "Storm Slash Effect";
        stormSlashEffect.localPosition = new Vector3(-4, 0.85f, -0.0009f);
        stormSlashEffect.localScale = new Vector3(1, 0.6319f, 1.2047f);
        stormSlashEffect.rotation = Quaternion.Euler(0, 0, 180);
        var s2 = sly.transform.Find("S2");
        var stormSlashBottom = UnityEngine.Object.Instantiate(s2, sly.transform);
        stormSlashBottom.name = "Storm Slash Bottom";
        stormSlashBottom.localPosition = new Vector3(-5.5f, -0.3f, 0);
        stormSlashBottom.localScale = new Vector3(0.5f, 3, 1);
        stormSlashBottom.rotation = Quaternion.Euler(0, 0, 270);
        var stormSlashTop = UnityEngine.Object.Instantiate(s2, sly.transform);
        stormSlashTop.name = "Storm Slash Top";
        stormSlashTop.localPosition = new Vector3(-5.5f, 0.25f, 0);
        stormSlashTop.localScale = new Vector3(-0.5f, 3, 1);
        stormSlashTop.rotation = Quaternion.Euler(0, 0, 270);
        radiantSpikes = new List<GameObject>();
        for (int x = 34; x <= 59; ++x)
        {
            var radiantSpike=UnityEngine.Object.Instantiate(radiantSpikeTemplate, new Vector3(x, 5.2f, -0.001f), Quaternion.identity);
            radiantSpikes.Add(radiantSpike);
        }
        fsm.AddState("Storm/Start");
        fsm.AddState("Storm/Jump");
        fsm.AddState("Storm/Hide");
        fsm.AddState("Storm/Show");
        fsm.AddState("Storm/Charged");
        fsm.AddState("Storm/Slash");
        fsm.AddState("Storm/Bounce");
        fsm.AddState("Storm/End");
        fsm.AddCustomAction("Storm/Start", () =>
        {
            fsm.AccessIntVariable("Storm/Phase1/Count").Value = 16;
            fsm.AccessFloatVariable("Storm/Phase1/DelayStart").Value = 1;
            fsm.AccessFloatVariable("Storm/Phase1/DelayEnd").Value = 0.02f;
            fsm.AccessIntVariable("Storm/Phase2/Count").Value = 0;
            fsm.AccessIntVariable("Storm/Count").Value = 0;
            fsm.AccessIntVariable("Storm/Show/LastDirection").Value = 0;
            fsm.AccessBoolVariable("Storm/Show/First").Value = true;
            sly.LocateMyFSM("Stun Control").SendEvent("STUN CONTROL STOP");
            foreach(var radiantSpike in radiantSpikes)
            {
                radiantSpike.LocateMyFSM("Control").SendEvent("UP");
            }
        });
        fsm.AddTransition("Storm/Start", "FINISHED", "Storm/Jump");
        fsm.AddAction("Storm/Jump", fsm.CreateTk2dPlayAnimation(fsm.gameObject, "Jump"));
        fsm.AddAction("Storm/Jump", fsm.GetAction("Air Roar", 6));
        fsm.AddCustomAction("Storm/Jump", () =>
        {
            sly.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 32, 0);
            sly.GetComponent<Rigidbody2D>().gravityScale = 1;
        });
        fsm.AddAction("Storm/Jump", fsm.CreateWait(0.5f, fsm.GetFSMEvent("FINISHED")));
        fsm.AddTransition("Storm/Jump", "FINISHED", "Storm/Show");
        fsm.AddCustomAction("Storm/Hide", () =>
        {
            sly.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            fsm.AccessIntVariable("Storm/Count").Value += 1;
            var count = fsm.AccessIntVariable("Storm/Count").Value;
            if (count < fsm.AccessIntVariable("Storm/Phase1/Count").Value)
            {
                sly.transform.position = Vector3.zero;
                var delayStart = fsm.AccessFloatVariable("Storm/Phase1/DelayStart").Value;
                var delayEnd = fsm.AccessFloatVariable("Storm/Phase1/DelayEnd").Value;
                var delay = delayStart + (delayEnd - delayStart) * count / (fsm.AccessIntVariable("Storm/Phase1/Count").Value - 1);
                fsm.GetAction<Wait>("Storm/Hide", 1).time = delay;
            }
            else if (count < fsm.AccessIntVariable("Storm/Phase1/Count").Value + fsm.AccessIntVariable("Storm/Phase2/Count").Value)
            {
                sly.transform.position = Vector3.zero;
                fsm.GetAction<Wait>("Storm/Hide", 1).time = 1;
            }
            else
            {
                sly.transform.rotation = Quaternion.identity;
                sly.GetComponent<Rigidbody2D>().gravityScale = 3;
                sly.transform.Find("Storm Slash Effect").gameObject.SetActive(false);
                sly.transform.Find("Sharp Flash").gameObject.SetActive(false);
                sly.transform.Find("Storm Slash Bottom").gameObject.SetActive(false);
                sly.transform.Find("Storm Slash Top").gameObject.SetActive(false);
                sly.LocateMyFSM("Stun Control").SendEvent("STUN CONTROL START");
                foreach (var radiantSpike in radiantSpikes)
                {
                    radiantSpike.LocateMyFSM("Control").SendEvent("DOWN");
                }
                fsm.SetState("Storm/End");
            }
        });
        fsm.AddAction("Storm/Hide", fsm.CreateWait(0, fsm.GetFSMEvent("FINISHED")));
        fsm.AddTransition("Storm/Hide", "FINISHED", "Storm/Show");
        fsm.AddAction("Storm/Show", fsm.CreateTk2dPlayAnimation(fsm.gameObject, "Charge Ground"));
        fsm.AddAction("Storm/Show", fsm.GetAction("Dash Charge", 2));
        fsm.AddCustomAction("Storm/Show", () =>
        {
            sly.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            var xMin = 34f;
            var xMax = 59f;
            var yMin = 7f;
            var yMax = 16f;
            var hero = HeroController.instance.gameObject;
            xMin = Math.Max(xMin, hero.transform.position.x - 12.5f);
            xMax = Math.Min(xMax, hero.transform.position.x + 12.5f);
            float x, y;
            var c = 0;
            while (true)
            {
                c += 1;
                x = UnityEngine.Random.Range(xMin, xMax);
                y = UnityEngine.Random.Range(yMin, yMax);
                if (Math.Abs(x - hero.transform.position.x) < 2)
                {
                    continue;
                }
                if (Math.Abs(y - hero.transform.position.y) < 2)
                {
                    continue;
                }
                if (Math.Abs(x - hero.transform.position.x) < 4 && c < 16)
                {
                    continue;
                }
                if (Math.Abs(y - hero.transform.position.y) < 4 && c < 16)
                {
                    continue;
                }
                var lD = fsm.AccessIntVariable("Storm/Show/LastDirection").Value;
                if (lD * (x - hero.transform.position.x) > 0 && c < 16)
                {
                    continue;
                }
                lD = Math.Sign(x - hero.transform.position.x);
                fsm.AccessIntVariable("Storm/Show/LastDirection").Value = lD;
                break;
            }
            if (fsm.AccessBoolVariable("Storm/Show/First").Value)
            {
                fsm.AccessBoolVariable("Storm/Show/First").Value = false;
            }
            else
            {
                sly.transform.position = new Vector3(x, y, 0.0061f);
            }
            sly.transform.rotation = Quaternion.identity;
            sly.GetComponent<Rigidbody2D>().gravityScale = 3;
            sly.transform.Find("NA Charge").gameObject.SetActive(true);
            sly.transform.Find("Storm Slash Effect").gameObject.SetActive(false);
            sly.transform.Find("Sharp Flash").gameObject.SetActive(false);
            sly.transform.Find("Storm Slash Bottom").gameObject.SetActive(false);
            sly.transform.Find("Storm Slash Top").gameObject.SetActive(false);
        });
        fsm.AddAction("Storm/Show", fsm.CreateGeneralAction(() =>
        {
            if (sly.transform.position.x < hero.transform.position.x)
            {
                sly.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                sly.transform.localScale = new Vector3(1, 1, 1);
            }
            var v = hero.transform.position - sly.transform.position;
            sly.GetComponent<Rigidbody2D>().velocity = v * 0.125f;
        }));
        fsm.AddAction("Storm/Show", fsm.CreateWait(0.1f, fsm.GetFSMEvent("FINISHED")));
        fsm.AddTransition("Storm/Show", "FINISHED", "Storm/Charged");
        fsm.AddAction("Storm/Charged", fsm.CreateTk2dPlayAnimation(fsm.gameObject, "Dash"));
        fsm.AddAction("Storm/Charged", fsm.GetAction("Dash Charged", 1));
        fsm.AddCustomAction("Storm/Charged", () =>
        {
            sly.transform.Find("NA Charge").gameObject.SetActive(false);
            sly.transform.Find("NA Charged").gameObject.SetActive(true);
        });
        fsm.AddAction("Storm/Charged", fsm.CreateGeneralAction(() =>
        {
            if (sly.transform.position.x < hero.transform.position.x)
            {
                sly.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                sly.transform.localScale = new Vector3(1, 1, 1);
            }
            var v = hero.transform.position - sly.transform.position;
            sly.GetComponent<Rigidbody2D>().velocity = v * 0.125f;
            float a = (float)Math.Atan2(v.y, v.x);
            if (sly.transform.localScale.x > 0)
            {
                a += (float)Math.PI;
                if (a > Math.PI)
                {
                    a -= 2 * (float)Math.PI;
                }
            }
            a = a / (float)Math.PI * 180;
            if (a < 0)
            {
                a = Math.Max(a, -36);
            }
            else
            {
                a = Math.Min(a, 36);
            }
            sly.transform.rotation = Quaternion.Euler(0, 0, a);
        }));
        fsm.AddAction("Storm/Charged", fsm.CreateWait(0.1f, fsm.GetFSMEvent("FINISHED")));
        fsm.AddTransition("Storm/Charged", "FINISHED", "Storm/Slash");
        fsm.AddAction("Storm/Slash", fsm.GetAction("Slash S1", 1));
        fsm.AddAction("Storm/Slash", fsm.GetAction("D Slash S1", 1));
        fsm.AddCustomAction("Storm/Slash", () =>
        {
            if (sly.transform.position.x < hero.transform.position.x)
            {
                sly.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                sly.transform.localScale = new Vector3(1, 1, 1);
            }
            var v = hero.transform.position - sly.transform.position;
            v = v.normalized * 128;
            sly.GetComponent<Rigidbody2D>().velocity = v;
            float a = (float)Math.Atan2(v.y, v.x);
            if (sly.transform.localScale.x < 0)
            {
                a += (float)Math.PI;
                if (a > Math.PI)
                {
                    a -= 2 * (float)Math.PI;
                }
            }
            a = a / (float)Math.PI * 180;
            sly.transform.rotation = Quaternion.Euler(0, 0, a);
            sly.GetComponent<Rigidbody2D>().gravityScale = 0;
            sly.transform.Find("NA Charged").gameObject.SetActive(false);
            sly.transform.Find("Storm Slash Effect").gameObject.SetActive(true);
            sly.transform.Find("Sharp Flash").gameObject.SetActive(true);
            sly.transform.Find("Storm Slash Bottom").gameObject.SetActive(true);
            sly.transform.Find("Storm Slash Top").gameObject.SetActive(true);
        });
        fsm.AddAction("Storm/Slash", fsm.CreateGeneralAction(() =>
        {
            if (sly.transform.position.y > 17)
            {
                fsm.SendEvent("END");
            }
        }));
        fsm.AddTransition("Storm/Slash", "END", "Storm/Hide");
        fsm.AddAction("Storm/Slash", fsm.CreateCheckCollisionSide(fsm.GetFSMEvent("CANCEL"), fsm.GetFSMEvent("CANCEL"), fsm.GetFSMEvent("CANCEL")));
        fsm.AddAction("Storm/Slash", fsm.CreateCheckCollisionSideEnter(fsm.GetFSMEvent("CANCEL"), fsm.GetFSMEvent("CANCEL"), fsm.GetFSMEvent("CANCEL")));
        fsm.AddTransition("Storm/Slash", "CANCEL", "Storm/Bounce");
        fsm.AddAction("Storm/Bounce", fsm.GetAction("Bounce L", 1));
        fsm.AddTransition("Storm/Bounce", "FINISHED", "Storm/Hide");
        fsm.AddAction("Storm/End", fsm.CreateTk2dPlayAnimation(fsm.gameObject, "Jump"));
        fsm.AddAction("Storm/End", fsm.CreateCheckCollisionSide(null, null, fsm.GetFSMEvent("LAND")));
        fsm.AddAction("Storm/End", fsm.CreateCheckCollisionSideEnter(null, null, fsm.GetFSMEvent("LAND")));
        fsm.AddTransition("Storm/End", "LAND", "Evade Antic");
    }
}