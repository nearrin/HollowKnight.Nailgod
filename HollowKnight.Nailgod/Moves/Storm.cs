﻿namespace Nailgod;
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
        stormSlashEffect.name = "Storm/Slash/Effect";
        stormSlashEffect.localPosition = new Vector3(-1.03f, -1.1738f, -0.0009f);
        fsm.AddCustomAction("Stun Reset", () =>
        {
            sly.transform.Find("Storm/Slash/Effect").gameObject.SetActive(false);
        });
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
            while (true)
            {
                x = UnityEngine.Random.Range(xMin, xMax);
                y = UnityEngine.Random.Range(yMin, yMax);
                if (Math.Abs(x - hero.transform.position.x) < 4)
                {
                    continue;
                }
                if (Math.Abs(y - hero.transform.position.y) < 4)
                {
                    continue;
                }
                var lD = fsm.AccessIntVariable("Storm/Show/LastDirection").Value;
                if (lD * (x - hero.transform.position.x) > 0)
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
            sly.transform.Find("NA Charged").gameObject.SetActive(false);
            sly.transform.Find("Storm/Slash/Effect").gameObject.SetActive(false);
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
            v = v.normalized * 8;
            sly.GetComponent<Rigidbody2D>().velocity = v;
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
            sly.transform.rotation = Quaternion.Euler(0, 0, a);
            sly.GetComponent<Rigidbody2D>().gravityScale = 0;
            sly.transform.Find("NA Charge").gameObject.SetActive(false);
            sly.transform.Find("NA Charged").gameObject.SetActive(false);
            sly.transform.Find("Storm/Slash/Effect").gameObject.SetActive(true);
        });
        fsm.AddAction("Storm/Slash", fsm.CreateGeneralAction(() =>
        {
        }));
        fsm.AddAction("Storm/Slash", fsm.CreateWait(0.5f, fsm.GetFSMEvent("FINISHED")));
        fsm.AddTransition("Storm/Slash", "FINISHED", "Storm/Hide");
    }
}