using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Skill
{
    public event UnityAction OnSkillComplete;

    private SkillJudgeBase judge;
    private SkillEffectBase effect;

    public float PreDelay { set => waitPreDelay = new WaitForSeconds(value); }
    public float PostDelay { set => waitPostDelay = new WaitForSeconds(value); }

    private YieldInstruction waitPreDelay;
    private YieldInstruction waitPostDelay;

    public Skill(SkillJudgeBase judge, SkillEffectBase effect)
    {
        this.judge = judge;
        this.effect = effect;
        this.judge.OnSkillHited = effect.Effect;
    }

    public IEnumerator CastSkill(Transform where)
    {
        yield return waitPreDelay;
        judge.Perform(where);
        yield return waitPostDelay;
        OnSkillComplete?.Invoke();
    }
}