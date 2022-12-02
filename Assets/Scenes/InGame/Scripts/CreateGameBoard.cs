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

    //블록의 크기 및 맵의 크기를 설정할때 사용됩니다.
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

        LevelData levelData
            = MyLib.Json.JsonToOject<LevelData>(stageData.text);

        List<SaveBlockData> eBlockDatas = levelData.blockDatas;
        List<SaveTargetData> eTargetDatas = levelData.targetDatas;
        int eMoveCnt = levelData.moveCnt;

        //보드초기화
        BoardManager boardManager = BoardManager.instance;
        boardManager.InitBoardData(blockWidth, blockHeight, 
            mapWidth, mapHeight, centerPos);

        //타일초기화
        TileGroup tileGroup = TileGroup.instance;
        tileGroup.InitTileMap(eBlockDatas);

        //블록초기화
        BlockGroup blockGroup = BlockGroup.instance;
        blockGroup.InitBlockMap(eBlockDatas);

        //터치판정초기화
        TouchManager touchManager = TouchManager.instance;
        touchManager.InitTouchMap(eBlockDatas);

        //UI 초기화
        InGameUI inGameUI = InGameUI.instance;
        inGameUI.InitGameUI(eMoveCnt, eTargetDatas);

        boardManager.StartBlockEvent();
    }
}
