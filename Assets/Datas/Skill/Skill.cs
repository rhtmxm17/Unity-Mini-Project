using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Skill
{
    public event UnityAction OnSkillComplete;

    private SkillJudgeBase skillJudge;
    public float PreDelay { set => waitPreDelay = new WaitForSeconds(value); }
    public float PostDelay { set => waitPostDelay = new WaitForSeconds(value); }

    private YieldInstruction waitPreDelay;
    private YieldInstruction waitPostDelay;

    public Skill(SkillJudgeBase judge, SkillEffectBase effect)
    {
        this.skillJudge = judge;
        this.skillJudge.OnSkillHited = effect.Effect;
    }

    public IEnumerator CastSkill()
    {
        yield return waitPreDelay;
        skillJudge.Perform();
        yield return waitPostDelay;
        OnSkillComplete?.Invoke();
    }
}