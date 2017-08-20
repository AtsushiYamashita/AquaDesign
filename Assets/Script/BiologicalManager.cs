﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <history date="17/08/20 18:49" Auth="Atsushi.Y.">
/// I had fixed almost.
/// class split and function split and process update,
/// </history>

/// <summary>
/// 水槽全体の管理をします。
/// </summary>
public class BiologicalManager
{
    /// <summary>
    ///このBiologicalManagerが複数作られた時に、
    ///エラーを表示するために使います。
    /// </summary>
    static private int _instantiated = 0;

    /// <summary>
    /// 魚の管理を行っているインスタンスです。
    /// </summary>
    private FishManager _fishManager;

    /// <summary>
    /// 水のインスタンスを保持しています。
    /// </summary>
    private GameObject _water;

    /// <summary>
    /// 魚を管理しているマネージャーを取得します。
    /// </summary>
    /// <returns></returns>
    public FishManager GetFishManager ( )
    {
        return _fishManager;
    }

    /// <summary>
    /// シーン開始時に呼び出され、
    /// 水インスタンスの取得と、魚の管理クラスを初期化しています。
    /// </summary>
    private void Start ( )
    {
        Assert.IsTrue( _instantiated == 0, "複数のマネージャが作成されています。" );
        GameObject water_pot = GameObject.Find( "WaterPot" );
        Assert.IsTrue( water_pot != null, "水槽オブジェクトがみつかりません。" );
        _water = water_pot.transform.FindChild( "Water" ).gameObject;
        _fishManager = new FishManager( _water );
    }
}

/// <summary>
/// 魚の管理を行うクラスです
/// </summary>
public class FishManager
{
    /// <summary>
    /// 魚を生成刷るときに使う、方向の初期値です。
    /// </summary>
    private Quaternion _firstDirection = Quaternion.Euler( new Vector3( 0, 90, 0 ) );

    /// <summary>
    /// 魚のインスタンスを辞書形式で保持しています。
    /// </summary>
    private Dictionary<string, Stack<GameObject>> _fishDictionary = new Dictionary<string, Stack<GameObject>>();

    /// <summary>
    /// 魚を生成するために使う水のオブジェクトです。
    /// </summary>
    private GameObject _water;

    /// <summary>
    /// マネージャーから水の情報をもらいます。
    /// </summary>
    /// <param name="water"> 水のオブジェクトです。 </param>
    public FishManager ( GameObject water ) { _water = water; }

    /// <summary>
    /// 魚のインスタンスを増やす関数です。
    /// </summary>
    /// <param name="type">魚の種類です</param>
    /// <param name="fish_name">魚の名前です</param>
    public void FishCreate ( string type, string fish_name )
    {
        DictionaryFix( fish_name );
        Vector3 waterScale = _water.transform.localScale;
        GameObject objResource = LoadRecource( type, fish_name );
        GameObject obj = GameObject.Instantiate( objResource );
        obj.transform.position = FirstPosition( waterScale );
        obj.transform.rotation = _firstDirection;

        obj.name = fish_name;
        _fishDictionary[fish_name].Push( obj );
    }

    /// <summary>
    /// 魚のインスタンスを削除します。
    /// </summary>
    /// <param name="fish_name">削除する魚の名前です。</param>
    /// <returns></returns>
    public bool ObjectDelete ( string fish_name )
    {
        if (_fishDictionary[fish_name].Count < 1) return false;
        GameObject.Destroy( _fishDictionary[fish_name].Pop() );
        return true;
    }

    /// <summary>
    /// 魚インスタンスを管理する辞書に、項目が足りなければ追加します。
    /// </summary>
    /// <param name="fish_name"></param>
    private void DictionaryFix ( string fish_name )
    {
        bool isArrayExist = _fishDictionary.ContainsKey( fish_name );
        if (isArrayExist) return;
        _fishDictionary.Add( fish_name, new Stack<GameObject>() );
    }

    /// <summary>
    /// 魚の初期位置を決める関数です。ランダムを使っています。
    /// </summary>
    /// <param name="waterScale">生成位置を決める為に使う、水の大きさです。</param>
    /// <returns>初期位置をベクトルで返します。</returns>
    private Vector3 FirstPosition ( Vector3 waterScale )
    {
        float randomX = Random.Range( waterScale.x / -2.0f, waterScale.x / 2.0f );
        float randomY = Random.Range( waterScale.y / -2.0f, waterScale.y / 2.0f );
        return new Vector3( randomX, randomY, 0 );
    }

    /// <summary>
    /// 魚のリソースを読み込んでいます。
    /// このコードは処理が重いため、改修予定です。
    /// </summary>
    /// <param name="Type">魚の種類を指定します。</param>
    /// <param name="fish_name">魚の名前です。</param>
    /// <returns>読み込まれたリソースを返します。</returns>
    private GameObject LoadRecource ( string Type, string fish_name )
    {
        return Resources.Load( "Prefub/" + Type + "/" + fish_name ) as GameObject;
    }
}
