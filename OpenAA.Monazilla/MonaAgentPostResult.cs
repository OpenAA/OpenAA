namespace OpenAA.Monazilla
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.IO;

    using OpenAA.Extensions.DateTime;

    using OpenAA.Monazilla.Models;

    public class MonaAgentPostResult
    {
        public enum ResultTypes {
            /// <summary>
            /// 失敗
            /// </summary>
            FAILED,

            /// <summary>
            /// 失敗。もう書けないスレッドです。
            /// </summary>
            STOPED,

            /// <summary>
            /// やられた
            /// </summary>
            KILLED,

            /// <summary>
            /// やりなおし
            /// </summary>
            CONTINUE,

            /// <summary>
            /// 成功
            /// </summary>
            SUCCEED,
        }

        public ResultTypes ResultType { get; private set; }
        public MonaBoard Board { get; private set; }
        public MonaAgentSession Session { get; private set; }
        public string Message { get; private set; }

        public MonaAgentPostResult(MonaBoard board, MonaAgentSession session, string message)
        {
            if (board == null)
            {
                throw new ArgumentNullException("board");
            }
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            this.Board = board;
            this.Session = session;
            this.Message = message;
            this.AnalyzeMessage();
        }

        private void AnalyzeMessage()
        {
            var message = this.Message;
            var now   = DateTime.Now;
            var host = new Uri(this.Board.Server).Host;

            // ----
            // 成功
            if (message.Contains("書きこみました"))
            {
                this.ResultType = ResultTypes.SUCCEED;
                this.Session.Wait[host] = now;
                return;
            }

            // ----
            // 確認
            if (message.Contains("■ 書き込み確認 ■"))
            {
                this.ResultType = ResultTypes.CONTINUE;
                this.Session.Wait[host] = now.AddSeconds(1);
                return;
            }

            // ----
            // 忍法帖作成待ち
            if (message.Contains("ようこそ：貴方の忍法帖を作成します。"))
            {
                this.ResultType = ResultTypes.CONTINUE;
                this.Session.Wait[host] = now.AddSeconds(30);
                return;
            }

            // ----
            // Samba
            var mSamba = Regex.Match(message, @"ＥＲＲＯＲ - 593 (?<samba>[0-9]+) sec たたないと書けません。\((?<count>[0-9]+)回目、(?<lapse>[0-9]+) sec しかたってない\)");
            if (mSamba.Success)
            {
                this.ResultType = ResultTypes.CONTINUE;

                var samba = int.Parse(mSamba.Groups["samba"].Value);
                var count = int.Parse(mSamba.Groups["count"].Value);
                //var lapse = int.Parse(mSamba.Groups["lapse"].Value);
                this.Session.Wait[host] = now.AddSeconds(samba + count + 1);
                this.Session.SambaLimit[host] = samba;
                this.Session.SambaCount[host] = count;
                return;
            }

            // Samba限界突破
            if (message.Contains("599 連打しないでください。"))
            {
                this.ResultType = ResultTypes.FAILED;
                this.Session.Wait[host] = now.AddSeconds(600);
                return;
            }

            // 連投規制
            if (message.Contains("ＥＲＲＯＲ：連続投稿ですか？？"))
            {
                this.ResultType = ResultTypes.FAILED;
                this.Session.Wait[host] = now.AddSeconds(600);
                return;
            }
            if (message.Contains("ＥＲＲＯＲ：連続投稿ですね！！"))
            {
                this.ResultType = ResultTypes.FAILED;
                this.Session.Wait[host] = now.AddSeconds(1200);
                return;
            }

            // ----
            // STOP
            if (message.Contains("ＥＲＲＯＲ：このスレッドには書き込めません。"))
            {
                this.ResultType = ResultTypes.STOPED;
                this.Session.Wait[host] = now;
                return;
            }
            if (message.Contains("ＥＲＲＯＲ：このスレッドは512kを超えているので書けません。"))
            {
                this.ResultType = ResultTypes.STOPED;
                this.Session.Wait[host] = now;
                return;
            }

            // ----
            // unknown error
            using (var sw = new StreamWriter("/tmp/message_" + ((long)now.ToUnixTime()) + ".txt")) {
                sw.Write(message);
            }

            //未テスト
            if (message.Contains("公開ＰＲＯＸＹからの投稿は受け付けていません"))
            {
                this.ResultType = ResultTypes.FAILED;
            }
            if (message.Contains("バーボン"))
            {
                this.ResultType = ResultTypes.FAILED;
            }
            if (message.Contains("修行が足りません"))
            {
                this.ResultType = ResultTypes.FAILED;
            }

            this.ResultType = ResultTypes.FAILED;
            this.Session.Wait[host] = now;
        }

        public override string ToString()
        {
            return string.Format("[MonaAgentPostResult: ResultType={0}, Session={1}, Message={2}]", ResultType, Session, Message);
        }
    }
}

