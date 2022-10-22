namespace Nailgod;
public class Storm
{
    public void UpdateFSM(PlayMakerFSM fsm)
    {
        fsm.AddState("Storm/Start");
        fsm.AddState("Storm/Hide");
        fsm.AddState("Storm/Show");
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
        fsm.AddAction("Storm/Hide", fsm.CreateWait(0.25f, fsm.GetFSMEvent("FINISHED")));
        fsm.AddTransition("Storm/Hide", "FINISHED", "Storm/Show");
        fsm.AddCustomAction("Storm/Show", () =>
        {
            var sly = fsm.gameObject;
            sly.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            sly.transform.position = new Vector3(46, 8, 0.0061f);
            var hero = HeroController.instance.gameObject;
            if (sly.transform.position.x < hero.transform.position.x)
            {
                sly.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                sly.transform.localScale = new Vector3(1, 1, 1);
            }
            sly.transform.Find("NA Charged").gameObject.SetActive(true);
        });
        fsm.AddAction("Storm/Show", fsm.CreateGeneralAction(() =>
         {
             var hero = HeroController.instance.gameObject;
             var sly = fsm.gameObject;
             var v = hero.transform.position - sly.transform.position;
             sly.GetComponent<Rigidbody2D>().velocity = v;
         }));
        fsm.AddAction("Storm/Show", fsm.CreateWait(0.125f, fsm.GetFSMEvent("FINISHED")));
        fsm.AddTransition("Storm/Show", "FINISHED", "Storm/Hide");
    }
}