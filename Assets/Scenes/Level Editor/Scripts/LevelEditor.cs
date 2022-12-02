using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MyLib;


////////////////////////////////////////////////////////////////////////////////
/// : ���� ����� ǥ�����ְ� �� ������ �����ϴ� ������
////////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
public class LevelEditor : SerializedMonoBehaviour
{
    //��� �̵� ���� Ƚ��
    [SerializeField] private int moveCnt;

    //��ǥ ���
    [SerializeField]
    private HashSet<SaveTargetData> targetBlocks = new HashSet<SaveTargetData>();

    [Space]
    //����� ũ�� �� ���� ũ��
    [SerializeField] private float blockWidth = 10;
    [SerializeField] private float blockHeight = 10;
    [SerializeField] private int mapWidth = 10;
    [SerializeField] private int mapHeight = 10;

    //��Ͽ�����Ʈ
    [SerializeField]
    private List<EditorBlock> blocks
        = new List<EditorBlock>();


    ////////////////////////////////////////////////////////////////////////////////
    /// : Update
    ////////////////////////////////////////////////////////////////////////////////
    private void Update()
    {
        UpdateBlock();
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : ����� �����͸� �������ش�.
    ////////////////////////////////////////////////////////////////////////////////
    private void UpdateBlock()
    {
        int blockIdx = 0;
        for (int y = 0; y < mapHeight; y++)
        {
            int pMapWidth = mapWidth;
            if (y % 2 == 1)
            {
                //Ȧ�� ����ĭ�� �ϳ� ������. �߰����ش�.
                pMapWidth += 1;
            }
            for (int x = 0; x < pMapWidth; x++)
            {
                if (blocks.Count <= blockIdx)
                {
                    //�ش� idx�� �ش��ϴ� ����� ����.
                    return;
                }

                EditorBlock editorBlock = blocks[blockIdx];

                //��� ��ġ ����
                editorBlock.posX = x;
                editorBlock.posY = y;

                //��� ��������Ʈ ����
                editorBlock.tileRenderer.sortingOrder = blockIdx * 2;
                editorBlock.blockRenderer.sortingOrder = blockIdx * 2 + 1;

                blockIdx++;

                if (editorBlock == null)
                {
                    //��� ��ü�� ����.
                    continue;
                }

                Transform blockTrans = editorBlock.transform;

                //����� ��ġ�� ���ؼ� �������ش�.
                Vector2 blockPos = Calculator.CalculateHexagonPos
                    (blockWidth, blockHeight, x, y);
                blockPos += (Vector2)transform.position;
                blockTrans.position = blockPos;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : �����ͷ� ���� ������ Json���Ϸ� ��������.
    ////////////////////////////////////////////////////////////////////////////////
    [Button("Export Data", ButtonSizes.Large)]
    public void ExportData()
    {
        LevelData levelData = new LevelData(blocks, targetBlocks, moveCnt);
        string jsonData = Json.ObjectToJson(levelData);
        var jtc2 = Json.JsonToOject<LevelData>(jsonData);
        Json.CreateJsonFile(Application.dataPath, "Resources/LevelData", jsonData);
    }
}
