using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////
/// : ����Ʈ�� �����մϴ�
////////////////////////////////////////////////////////////////////////////////
public class EffectManager : DontDestroySingleton<EffectManager>
{
    [SerializeField]
    private CrushEffect crushEffect;
    private List<CrushEffect> crushEffects = new List<CrushEffect>();

    ////////////////////////////////////////////////////////////////////////////////
    /// : ��Ϻ��� ����Ʈ
    ////////////////////////////////////////////////////////////////////////////////
    public void CrushEffect(BlockType pBlcokType, Vector3 pPos)
    {
        CrushEffect newEffect = null;
        foreach (CrushEffect effects in crushEffects)
        {
            if(effects.ParticleIsStop())
            {
                newEffect = effects;
                break;
            }
        }

        if (newEffect == null)
        {
            newEffect = Instantiate(crushEffect);
            crushEffects.Add(newEffect);
        }
        newEffect.effectType = pBlcokType;
        newEffect.transform.position = pPos;

        newEffect.ParticlePlay();
    }
}
