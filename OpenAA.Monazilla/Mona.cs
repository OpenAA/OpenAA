namespace OpenAA.Monazilla
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// BBSルート
    /// </summary>
    public class MonaBbs
    {
        /// <summary>
        /// デバッグ用のRAWデータ
        /// </summary>
        string Raw;

        IList<MonaCategory> Category;
    }

    /// <summary>
    /// カテゴリ
    /// </summary>
    public class MonaCategory
    {
        MonaBbs Bbs;

        /// <summary>
        /// デバッグ用のRAWデータ
        /// </summary>
        string Raw;

        int No;
        string Name;
        IList<MonaBoard> Boards;
    }

    /// <summary>
    /// 板
    /// </summary>
    public class MonaBoard 
    {
        MonaCategory Category;

        /// <summary>
        /// デバッグ用のRAWデータ
        /// </summary>
        string Raw;

        Uri Server;
        int Id;
        int No;
        string Name;
        IList<MonaThread> Threads;
    }

    /// <summary>
    /// スレッド
    /// DATを読み込ませれば独立してそこそこ動くようにしたい。
    /// </summary>
    public class MonaThread 
    {
        MonaBoard Board;

        /// <summary>
        /// デバッグ用のRAWデータ
        /// </summary>
        string Raw;

        int Id;
        int No;
        string Title;
        int Count;
        IList<MonaResponse> Responses;
    }

    /// <summary>
    /// レス
    /// </summary>
    public class MonaResponse 
    {
        MonaThread Thread;

        /// <summary>
        /// デバッグ用のRAWデータ
        /// </summary>
        string Raw;

        int No;
        string Name;
        string Mail;
        string Message;
    }
}

