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

        LevelEditor.LevelData levelData
            = MyLib.Json.JsonToOject<LevelEditor.LevelData>(stageData.text);


        //Ÿ�ϻ���
        List<LevelEditor.SaveBlockData> eblockDatas = levelData.blockDatas;

        BoardManager boardManager = BoardManager.instance;
        boardManager.InitBoardData(blockWidth, blockHeight, 
            mapWidth, mapHeight, centerPos);

        TileGroup tileGroup = TileGroup.instance;
        tileGroup.InitTileMap(eblockDatas);

        BlockGroup blockGroup = BlockGroup.instance;
        blockGroup.InitBlockMap(eblockDatas);

        TouchManager touchManager = TouchManager.instance;
        touchManager.InitTouchMap(eblockDatas);
    }
}
