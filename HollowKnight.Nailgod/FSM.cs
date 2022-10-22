namespace Nailgod;
public class GeneralAction : FsmStateAction
{
    public override void OnFixedUpdate()
    {
        action();
    }
    public Action action;
}
public static class FSM
{
    public static FsmFloat AccessFloatVariable(this PlayMakerFSM fsm, string name)
    {
        FsmFloat fsmFloat = fsm.FsmVariables.FloatVariables.FirstOrDefault(x => x.Name == name);
        if (fsmFloat != null)
            return fsmFloat;
        fsmFloat = new FsmFloat(name);
        fsm.FsmVariables.FloatVariables = fsm.FsmVariables.FloatVariables.Append(fsmFloat).ToArray();
        return fsmFloat;
    }
    public static FsmGameObject AccessGameObjectVariable(this PlayMakerFSM fsm, string name)
    {
        FsmGameObject fsmGameObject = fsm.FsmVariables.GameObjectVariables.FirstOrDefault(x => x.Name == name);
        if (fsmGameObject != null)
            return fsmGameObject;
        fsmGameObject = new FsmGameObject(name);
        fsm.FsmVariables.GameObjectVariables = fsm.FsmVariables.GameObjectVariables.Append(fsmGameObject).ToArray();
        return fsmGameObject;
    }
    public static FsmInt AccessIntVariable(this PlayMakerFSM fsm, string name)
    {
        FsmInt fsmInt = fsm.FsmVariables.IntVariables.FirstOrDefault(x => x.Name == name);
        if (fsmInt != null)
            return fsmInt;
        fsmInt = new FsmInt(name);
        fsm.FsmVariables.IntVariables = fsm.FsmVariables.IntVariables.Append(fsmInt).ToArray();
        return fsmInt;
    }
    public static FsmBool AccessBoolVariable(this PlayMakerFSM fsm, string name)
    {
        FsmBool fsmBool = fsm.FsmVariables.BoolVariables.FirstOrDefault(x => x.Name == name);
        if (fsmBool != null)
            return fsmBool;
        fsmBool = new FsmBool(name);
        fsm.FsmVariables.BoolVariables = fsm.FsmVariables.BoolVariables.Append(fsmBool).ToArray();
        return fsmBool;
    }
    public static FsmEvent GetFSMEvent(this PlayMakerFSM fsm, string name)
    {
        foreach (var fsmEvent in fsm.FsmEvents)
        {
            if (fsmEvent.Name == name)
            {
                return fsmEvent;
            }
        }
        throw new Exception();
    }
    public static Wait CreateWait(this PlayMakerFSM fsm, float time, FsmEvent fsmEvent)
    {
        var wait = new Wait()
        {
            time = time,
            finishEvent = fsmEvent,
            realTime = false,
        };
        return wait;
    }
    public static CheckCollisionSide CreateCheckCollisionSide(
        this PlayMakerFSM fsm, FsmEvent leftHitEvent, FsmEvent rightHitEvent, FsmEvent bottomHitEvent)
    {
        var checkCollisionSide = new CheckCollisionSide()
        {
            leftHit = false,
            rightHit = false,
            bottomHit = false,
            topHit = false,
            leftHitEvent = leftHitEvent,
            rightHitEvent = rightHitEvent,
            bottomHitEvent = bottomHitEvent,
            otherLayer = false,
            otherLayerNumber = 0,
            ignoreTriggers = false,
        };
        return checkCollisionSide;
    }
    public static CheckCollisionSideEnter CreateCheckCollisionSideEnter(
        this PlayMakerFSM fsm, FsmEvent leftHitEvent, FsmEvent rightHitEvent, FsmEvent bottomHitEvent)
    {
        var checkCollisionSideEnter = new CheckCollisionSideEnter()
        {
            leftHit = false,
            rightHit = false,
            bottomHit = false,
            topHit = false,
            leftHitEvent = leftHitEvent,
            rightHitEvent = rightHitEvent,
            bottomHitEvent = bottomHitEvent,
            otherLayer = false,
            otherLayerNumber = 0,
            ignoreTriggers = false,
        };
        return checkCollisionSideEnter;
    }
    public static GeneralAction CreateGeneralAction(this PlayMakerFSM fsm, Action action)
    {
        return new GeneralAction()
        {
            action = action,
        };
    }
}