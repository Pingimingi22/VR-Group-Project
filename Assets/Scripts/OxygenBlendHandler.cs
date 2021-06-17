using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenBlendHandler : MonoBehaviour
{
    public PlayerManager m_playerManager;

    public SkinnedMeshRenderer m_skinnedMeshRenderer;
    private Mesh m_skinnedMesh;
    int m_blendShapeCount;
    // Start is called before the first frame update
    void Start()
    {
        m_skinnedMesh = m_skinnedMeshRenderer.sharedMesh;
        m_blendShapeCount = m_skinnedMesh.blendShapeCount;
    }

    // Update is called once per frame
    void Update()
    {
        m_skinnedMeshRenderer.SetBlendShapeWeight(0, 100 - m_playerManager.m_playerHealth);
    }
}
