using JsonFx.Json;
using System;
using System.Collections.Generic;
using System.IO;
using YouduSDK.EntApp;
using YouduSDK.EntApp.AES;
using YouduSDK.EntApp.MessageEntity;
using YouduSDK.EntApp.SessionEntity;

namespace AppDemo
{
    public class myAppServer : HttpServer
    {
        public myAppServer(int port)
            : base(port)
        {
        }

        private const int Buin = 0; // 请填写企业总机号
        private const string AppId = ""; // 请填写AppId
        private const string Token = ""; // 请填写回调Token
        private const string Uri = "/msg/receive"; // 请填写回调URI

        private const string Address = "127.0.0.1:7080"; // 请填写有度服务器地址
        private const string EncodingaesKey = ""; // 请填写企业应用EncodingaesKey
        private const string OutDir = @""; // 请填写文件下载目录


        private AESCrypto m_crypto = new AESCrypto(AppId, EncodingaesKey);
        private AppClient m_appClient = new AppClient(Address, Buin, AppId, EncodingaesKey);

        public override void handleGETRequest(HttpProcessor p)
        {
            Console.WriteLine("request: {0}", p.http_url);
            p.writeSuccess();
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            Console.WriteLine("POST request: {0}", p.http_url);

            var compnents = p.http_url.Split('?');
            if (compnents.Length != 2)
            {
                Console.WriteLine("invalid url compnents: {0}", compnents.ToString());
                p.writeFailure();
                return;
            }

            var reqUri = compnents[0];
            if (!reqUri.Equals(Uri))
            {
                p.writeFailure();
                return;
            }
            var queryPath = compnents[1];
            var queryDict = new Dictionary<string, string>();
            foreach (var statement in queryPath.Split("&&".ToCharArray()))
            {
                var elements = statement.Split('=');
                if (elements.Length == 2)
                {
                    queryDict[elements[0]] = elements[1];
                }
            }

            var data = inputData.ReadToEnd();
            var jsonReader = new JsonReader();
            var reqJson = jsonReader.Read<Dictionary<string, object>>(data);
            object toBuin;
            object toApp;
            object encrypt;
            if (!reqJson.TryGetValue("toBuin", out toBuin)
                || toBuin is int == false
                || (int)toBuin <= 0
                || !reqJson.TryGetValue("toApp", out toApp)
                || toApp is string == false
                || ((string)toApp).Length == 0
                || !reqJson.TryGetValue("encrypt", out encrypt)
                || encrypt is string == false
                || ((string)encrypt).Length == 0)
            {
                Console.WriteLine("invalid toBuin or toApp or encrypt");
                p.writeFailure();
                return;
            }

            var toBuinValue = (int)toBuin;
            var toAppValue = (string)toApp;
            var encryptValue = (string)encrypt;

            string timeStamp;
            string nonce;
            string signature;
            if (!queryDict.TryGetValue("timestamp", out timeStamp)
                || timeStamp == null
                || !queryDict.TryGetValue("nonce", out nonce)
                || nonce == null
                || !queryDict.TryGetValue("msg_signature", out signature)
                || signature == null)
            {
                Console.WriteLine("invalid timestamp or nonce or msg_signature");
                p.writeFailure();
                return;
            }

            if (toBuinValue != Buin || !toAppValue.Equals(AppId))
            {
                Console.WriteLine("buin or appId is not matched");
                p.writeFailure();
                return;
            }

            var mySignature = Signature.GenerateSignature(Token, timeStamp, nonce, encryptValue);
            if (!signature.Equals(mySignature))
            {
                Console.WriteLine("signature is not matched");
                p.writeFailure();
                return;
            }
            
            var decryptContent = m_crypto.Decrypt(encryptValue);
            //var msg = new SessionMessage().FromJson(AESCrypto.ToString(decryptContent));
            var msg = new ReceiveMessage().FromJson(AESCrypto.ToString(decryptContent));

            switch (msg.MsgType)
            {
                case Message.MessageTypeImage:
                    {
                        var msgBody = msg.MsgBody.ToImageBody();
                        m_appClient.DownloadFile(msgBody.MediaId, OutDir);
                    }
                    break;

                case Message.MessageTypeFile:
                    {
                        var msgBody = msg.MsgBody.ToFileBody();
                        m_appClient.DownloadFile(msgBody.MediaId, OutDir);
                    }
                    break;

                default:
                    break;
            }

            Console.WriteLine(msg.ToString());
            Console.WriteLine("packageId: {0}", msg.PackageId);

            p.writeSuccess();
            p.outputStream.WriteLine(msg.PackageId);
        }
    }
}
