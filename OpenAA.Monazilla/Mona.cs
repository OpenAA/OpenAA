namespace OpenAA.Monazilla
{
    using System;
    using System.Collections.Generic;

    public class MonaBbs
    {
        /// <summary>
        /// デバッグ用のRAWデータ
        /// </summary>
        string Raw;

        IList<MonaCategory> Category;
    }

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

    public class MonaBoard 
    {
        MonaCategory Category;

        /// <summary>
        /// デバッグ用のRAWデータ
        /// </summary>
        string Raw;

        string Server;
        int Id;
        int No;
        string Name;
        IList<MonaThread> Threads;
    }

    /// <summary>
    /// DATを読み込ませれば独立してそこそこ動くようにする。
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

