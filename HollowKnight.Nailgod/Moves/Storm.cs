namespace Nailgod;
public class Storm
{
    public void UpdateFSM(PlayMakerFSM fsm)
    {
        var sly = fsm.gameObject;
        var hero = HeroController.instance.gameObject;
        fsm.AddState("Storm/Start");
        fsm.AddState("Storm/Hide");
        fsm.AddState("Storm/Show");
        fsm.AddState("Storm/Charged");
        fsm.AddState("Storm/Slash");
        fsm.AddState("Storm/Bounce");
        fsm.AddCustomAction("Storm/Start", () =>
        {
            sly.LocateMyFSM("Stun Control").SendEvent("STUN CONTROL STOP");
            fsm.AccessIntVariable("Storm/Show/LastDirection").Value = 0;
        });
        fsm.AddTransition("Storm/Start", "FINISHED", "Storm/Hide");
        fsm.AddCustomAction("Storm/Hide", () =>
        {
            sly.transform.position = Vector3.zero;
        });
        fsm.AddAction("Storm/Hide", fsm.CreateWait(0.5f, fsm.GetFSMEvent("FINISHED")));
        fsm.AddTransition("Storm/Hide", "FINISHED", "Storm/Show");
        fsm.AddAction("Storm/Show", fsm.CreateTk2dPlayAnimation(fsm.gameObject, "Charge Ground"));
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
        fsm.AddCustomAction("Stun Reset", () =>
        {
            sly.transform.Find("Storm Slash Effect").gameObject.SetActive(false);
            sly.transform.Find("Sharp Flash").gameObject.SetActive(false);
            sly.transform.Find("Storm Slash Bottom").gameObject.SetActive(false);
            sly.transform.Find("Storm Slash Top").gameObject.SetActive(false);
        });
        fsm.AddCustomAction("Storm/Show", () =>
        {
            sly.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            var xMin = 34f;
            var xMax = 59f;
            var yMin = 11.5f;
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
                if (Math.Abs(x - hero.transform.position.x) < 4 && c<16)
                {
                    continue;
                }
                if (Math.Abs(y - hero.transform.position.y) < 4 && c<16)
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
            sly.transform.position = new Vector3(x, y, 0.0061f);
            sly.transform.rotation = Quaternion.identity;
            sly.transform.Find("NA Charge").gameObject.SetActive(true);
            sly.transform.Find("Storm Slash Effect").gameObject.SetActive(false);
            sly.transform.Find("Sharp Flash").gameObject.SetActive(false);
            sly.transform.Find("Storm Slash Bottom").gameObject.SetActive(false);
            sly.transform.Find("Storm Slash Top").gameObject.SetActive(false);
            sly.GetComponent<Rigidbody2D>().gravityScale = 3;
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
    }
}