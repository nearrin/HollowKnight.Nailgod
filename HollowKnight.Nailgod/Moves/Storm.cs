namespace Nailgod;
public class Storm
{
    public void UpdateFSM(PlayMakerFSM fsm)
    {
        fsm.AddState("Storm/Start");
        fsm.AddState("Storm/Hide");
        fsm.AddState("Storm/Show");
        fsm.AddState("Storm/Charged");
        fsm.AddState("Storm/Slash");
        fsm.AddCustomAction("Storm/Start", () =>
        {
            var sly = fsm.gameObject;
            sly.LocateMyFSM("Stun Control").SendEvent("STUN CONTROL STOP");
        });
        fsm.AddTransition("Storm/Start", "FINISHED", "Storm/Hide");
        fsm.AddCustomAction("Storm/Hide", () =>
        {
            var sly = fsm.gameObject;
            sly.transform.position = Vector3.zero;
        });
        fsm.AddAction("Storm/Hide", fsm.CreateWait(0.5f, fsm.GetFSMEvent("FINISHED")));
        fsm.AddTransition("Storm/Hide", "FINISHED", "Storm/Show");
        fsm.AddCustomAction("Storm/Show", () =>
        {
            var sly = fsm.gameObject;
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
                break;
            }
            sly.transform.position = new Vector3(x, y, 0.0061f);
            if (sly.transform.position.x < hero.transform.position.x)
            {
                sly.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                sly.transform.localScale = new Vector3(1, 1, 1);
            }
            sly.transform.Find("NA Charge").gameObject.SetActive(true);
            sly.transform.Find("NA Charged").gameObject.SetActive(false);
        });
        fsm.AddAction("Storm/Show", fsm.CreateGeneralAction(() =>
         {
             var hero = HeroController.instance.gameObject;
             var sly = fsm.gameObject;
             var v = hero.transform.position - sly.transform.position;
             sly.GetComponent<Rigidbody2D>().velocity = v * 0.125f;
         }));
        fsm.AddAction("Storm/Show", fsm.CreateWait(0.1f, fsm.GetFSMEvent("FINISHED")));
        fsm.AddTransition("Storm/Show", "FINISHED", "Storm/Charged");
        fsm.AddCustomAction("Storm/Charged", () =>
        {
            var sly = fsm.gameObject;
            sly.transform.Find("NA Charge").gameObject.SetActive(false);
            sly.transform.Find("NA Charged").gameObject.SetActive(true);
        });
        fsm.AddAction("Storm/Charged", fsm.CreateGeneralAction(() =>
        {
            var hero = HeroController.instance.gameObject;
            var sly = fsm.gameObject;
            var v = hero.transform.position - sly.transform.position;
            sly.GetComponent<Rigidbody2D>().velocity = v * 0.125f;
        }));
        fsm.AddAction("Storm/Charged", fsm.CreateWait(0.1f, fsm.GetFSMEvent("FINISHED")));
        fsm.AddTransition("Storm/Charged", "FINISHED", "Storm/Hide");
    }
}