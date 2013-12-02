namespace OpenAA.Monazilla.Models
{
    using System;
    using System.Collections.Generic;
    using OpenAA;

    /// <summary>
    /// スレ
    /// </summary>
    public class MonaThread
    {
        /// <summary>
        /// スレが所属する板
        /// </summary>
        public MonaBoard Board;

        /// <summary>
        /// 出現順序
        /// ソート用
        /// </summary>
        public int No;

        /// <summary>
        /// スレID
        /// 実はUnixTime
        /// </summary>
        public string Id;

        /// <summary>
        /// スレタイ
        /// </summary>
        public string Title;

        /// <summary>
        /// レス数（概算）
        /// スレタイから抜き出した概算値なので正確な値は
        /// datを取得してレス数をカウントせよ。
        /// </summary>
        public int Nums;

        /// <summary>
        /// スレId (実態はUnix Time)をDateTimeに変換したスレ作成日時
        /// </summary>
        /// <value>The create time.</value>
        public DateTime CreateTime
        {
            get
            {
                double unixTime = DoubleUtility.ParseOrDefault(this.Id);
                return DateTimeUtility.UnixTimeToDateTime(unixTime);
            }
        }

        /// <summary>
        /// スレ一覧の取得日時
        /// 勢い算出用
        /// </summary>
        /// <value>The update time.</value>
        public DateTime UpdateTime;

        /// <summary>
        /// 勢い
        /// </summary>
        /// <value>The trend.</value>
        public double Trend
        {
            get 
            {
                var ret = this.Nums / ((this.UpdateTime - this.CreateTime).TotalSeconds / 86400.0);
                if (ret < 0.0)
                {
                    return 0.0;
                }
                else
                {
                    return ret;
                }
            }
        }
    }

}

