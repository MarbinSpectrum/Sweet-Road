
public enum BlockType
{
    //��Ÿ
    none,
    empty,
    spawn,

    //�Ϲ� ���
    red,
    orange,
    yellow,
    green,
    blue,
    purple,

    //���� ���
    spin,
}

public enum SpecialType
{
    normal,
    rocket
}

public enum MatchType
{
    none = -1,          //�ƹ��ϵ� ���Ͼ

    match5,             //5���̻��� ����� ������
    match4,             //4���� ����� ������
    match3,             //3���� ����� ������

    gather4,            //4���� ����� ����


    size
}

public enum BlockDic
{
    left,
    up,
    right
}