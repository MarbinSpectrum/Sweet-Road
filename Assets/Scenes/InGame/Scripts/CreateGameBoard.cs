using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

////////////////////////////////////////////////////////////////////////////////
/// : 게임보드를 생성하는 역할을 합니다.
////////////////////////////////////////////////////////////////////////////////
public class CreateGameBoard : FieldObjectSingleton<CreateGameBoard>
{
    [SerializeField] private TextAsset stageData;
    [SerializeField] private Vector2 centerPos;

    //블록의 크기 및 맵의 크기
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
    /// : json객체로 저장된 스테이지 정보를 읽어오고 적용한다.
    ////////////////////////////////////////////////////////////////////////////////
    private void LoadStageData()
    {
        if (stageData == null)
        {
            //스테이지 정보가 없다.
            return;
        }

        LevelEditor.LevelData levelData
            = MyLib.Json.JsonToOject<LevelEditor.LevelData>(stageData.text);


        //타일생성
        List<LevelEditor.SaveBlockData> eblockDatas = levelData.blockDatas;

        TileGroup tileGroup = TileGroup.instance;
        tileGroup.InitTileMap(eblockDatas, centerPos, 
            blockWidth, blockHeight, mapWidth, mapHeight);

        BlockGroup blockGroup = BlockGroup.instance;
        blockGroup.InitBlockMap(eblockDatas, centerPos,
            blockWidth, blockHeight, mapWidth, mapHeight);

        TouchManager touchManager = TouchManager.instance;
        touchManager.InitTouchMap(eblockDatas, centerPos,
        blockWidth, blockHeight, mapWidth, mapHeight);
    }
}
