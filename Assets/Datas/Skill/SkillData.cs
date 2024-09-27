using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Skill Data")]
public class SkillData : ScriptableObject
{
    public IDamageable.Flag hitMaskBase; // 피아를 제외하고, 지형지물에 가로막히는지 등을 설정

    //public bool isHumanoid; // 휴머노이드의 애니메이션인지 표시
    //public AnimationClip animation;

    public float preDelay; // 선딜레이 또는 캐스팅
    public float postDelay; // 후딜레이
    public SkillJudgeBase judge; // 시전된 스킬의 판정 규칙(Overlab/OnTriggerEnter 등을 구현)
    public SkillEffectBase effect; // 적중시 처리

    public Skill BakeSkill(IDamageable.Flag targetLayer, IUnit source)
    {
        SkillJudgeBase judge = this.judge.Clone();
        judge.hitMask |= targetLayer;

        SkillEffectBase effect = this.effect.Clone();
        effect.source = source;

        Skill baked = new Skill(judge, effect);
        baked.PreDelay = preDelay;
        baked.PostDelay = postDelay;

        return baked;
    }
}
