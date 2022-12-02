
public enum BlockType
{
    //기타
    none,
    empty,
    spawn,

    //일반 블록
    red,
    orange,
    yellow,
    green,
    blue,
    purple,

    //방해 블록
    spin,
}

public enum SpecialType
{
    normal,
    rocket
}

public enum MatchType
{
    none = -1,          //아무일도 안일어남

    match5,             //5개이상의 블록이 일직선
    match4,             //4개의 블록이 일직선
    match3,             //3개의 블록이 일직선

    gather4,            //4개의 블록이 모임


    size
}

public enum BlockDic
{
    left,
    up,
    right
}