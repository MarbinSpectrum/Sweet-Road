using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MyLib;


////////////////////////////////////////////////////////////////////////////////
/// : 맵의 블록을 표시해주고 맵 정보를 저장하는 에디터
////////////////////////////////////////////////////////////////////////////////

[ExecuteInEditMode]
public class LevelEditor : SerializedMonoBehaviour
{
    //블록 이동 제한 횟수
    [SerializeField] private int moveCnt;

    //목표 블록
    [SerializeField]
    private HashSet<SaveTargetData> targetBlocks = new HashSet<SaveTargetData>();

    [Space]
    //블록의 크기 및 맵의 크기
    [SerializeField] private float blockWidth = 10;
    [SerializeField] private float blockHeight = 10;
    [SerializeField] private int mapWidth = 10;
    [SerializeField] private int mapHeight = 10;

    //블록오브젝트
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
    /// : 블록의 데이터를 갱신해준다.
    ////////////////////////////////////////////////////////////////////////////////
    private void UpdateBlock()
    {
        int blockIdx = 0;
        for (int y = 0; y < mapHeight; y++)
        {
            int pMapWidth = mapWidth;
            if (y % 2 == 1)
            {
                //홀수 높이칸은 하나 더들어간다. 추가해준다.
                pMapWidth += 1;
            }
            for (int x = 0; x < pMapWidth; x++)
            {
                if (blocks.Count <= blockIdx)
                {
                    //해당 idx에 해당하는 블록이 없다.
                    return;
                }

                EditorBlock editorBlock = blocks[blockIdx];

                //블록 위치 설정
                editorBlock.posX = x;
                editorBlock.posY = y;

                //블록 스프라이트 정렬
                editorBlock.tileRenderer.sortingOrder = blockIdx * 2;
                editorBlock.blockRenderer.sortingOrder = blockIdx * 2 + 1;

                blockIdx++;

                if (editorBlock == null)
                {
                    //블록 객체가 없다.
                    continue;
                }

                Transform blockTrans = editorBlock.transform;

                //블록의 위치를 구해서 설정해준다.
                Vector2 blockPos = Calculator.CalculateHexagonPos
                    (blockWidth, blockHeight, x, y);
                blockPos += (Vector2)transform.position;
                blockTrans.position = blockPos;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : 에디터로 만든 레벨을 Json파일로 내보낸다.
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
