using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

////////////////////////////////////////////////////////////////////////////////
/// : ���Ӻ��带 �����ϴ� ������ �մϴ�.
////////////////////////////////////////////////////////////////////////////////
public class CreateGameBoard : FieldObjectSingleton<CreateGameBoard>
{
    [SerializeField] private TextAsset stageData;
    [SerializeField] private Vector2 centerPos;

    //����� ũ�� �� ���� ũ�⸦ �����Ҷ� ���˴ϴ�.
    [SerializeField] private float blockWidth;
    [SerializeField] private float blockHeight;
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;

    ////////////////////////////////////////////////////////////////////////////////
    /// : Start
    ////////////////////////////////////////////////////////////////////////////////
    private void Start()
    {
        LoadStageData(); 
    }

    ////////////////////////////////////////////////////////////////////////////////
    /// : json��ü�� ����� �������� ������ �о���� �����Ѵ�.
    ////////////////////////////////////////////////////////////////////////////////
    private void LoadStageData()
    {
        if (stageData == null)
        {
            //�������� ������ ����.
            return;
        }

        LevelData levelData
            = MyLib.Json.JsonToOject<LevelData>(stageData.text);

        List<SaveBlockData> eBlockDatas = levelData.blockDatas;
        List<SaveTargetData> eTargetDatas = levelData.targetDatas;
        int eMoveCnt = levelData.moveCnt;

        //�����ʱ�ȭ
        BoardManager boardManager = BoardManager.instance;
        boardManager.InitBoardData(blockWidth, blockHeight, 
            mapWidth, mapHeight, centerPos);

        //Ÿ���ʱ�ȭ
        TileGroup tileGroup = TileGroup.instance;
        tileGroup.InitTileMap(eBlockDatas);

        //����ʱ�ȭ
        BlockGroup blockGroup = BlockGroup.instance;
        blockGroup.InitBlockMap(eBlockDatas);

        //��ġ�����ʱ�ȭ
        TouchManager touchManager = TouchManager.instance;
        touchManager.InitTouchMap(eBlockDatas);

        //UI �ʱ�ȭ
        InGameUI inGameUI = InGameUI.instance;
        inGameUI.InitGameUI(eMoveCnt, eTargetDatas);

        boardManager.StartBlockEvent();
    }
}
