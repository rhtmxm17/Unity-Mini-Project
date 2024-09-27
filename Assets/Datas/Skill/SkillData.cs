using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData : ScriptableObject
{
    public IDamageable.Flag hitMaskBase; // 피아를 제외하고, 지형지물에 가로막히는지 등을 설정

    public bool isHumanoid; // 휴머노이드의 애니메이션인지 표시
    public AnimationClip animation;

    public float castingTime; // 실제 시전(또는 발사)까지 걸리는 시간
    public SkillJudgeBase hitLogic; // 시전된 스킬의 판정 규칙(Overlab/OnTriggerEnter 등을 구현)
    public SkillEffectBase effect; // 적중시 처리

    public Projectile projectilePrefab; // 발사형 스킬이라면 사용할 투사체
}
