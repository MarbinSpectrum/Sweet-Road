using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using Sirenix.OdinInspector;

////////////////////////////////////////////////////////////////////////////////
/// : ºí·Ï±úÁü ÀÌÆåÆ®
////////////////////////////////////////////////////////////////////////////////
public class CrushEffect : SerializedMonoBehaviour
{
    private BlockType EffectType;
    public BlockType effectType
    {
        set
        {
            EffectType = value;

            Color color = effectColors[EffectType];

            foreach (ParticleSystem particle in particles)
            {
                ParticleSystem.MainModule mainModule = particle.main;
                mainModule.startColor = color;
            }
        }
        get
        {
            return EffectType;
        }
    }

    [SerializeField]
    private Dictionary<BlockType, Color> effectColors
        = new Dictionary<BlockType, Color>();

    [SerializeField] private List<ParticleSystem> particles;

    ////////////////////////////////////////////////////////////////////////////////
    /// : ÆÄÆ¼Å¬ÀÌ ¸ØÃá ¿©ºÎ
    ////////////////////////////////////////////////////////////////////////////////
    public bool ParticleIsStop()
    {
        foreach (ParticleSystem particle in particles)
        {
            if (particle.isPlaying)
                return false;
        }
        return true;
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ÆÄÆ¼Å¬ÀÌ Àç»ý
    ////////////////////////////////////////////////////////////////////////////////
    public void ParticlePlay()
    {
        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }
    }
}
