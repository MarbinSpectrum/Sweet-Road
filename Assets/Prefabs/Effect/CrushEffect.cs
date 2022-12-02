using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;
using Sirenix.OdinInspector;

////////////////////////////////////////////////////////////////////////////////
/// : ��ϱ��� ����Ʈ
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
    /// : ��ƼŬ�� ���� ����
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
    /// : ��ƼŬ�� ���
    ////////////////////////////////////////////////////////////////////////////////
    public void ParticlePlay()
    {
        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }
    }
}
