﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using YouduSDK.EntApp.MessageEntity;
using System;
using System.Collections.Generic;


namespace YouduSDK.EntApp.Tests
{
    [TestClass()]
    public class AppClientTests
    {
        private const string FileName = ""; // 请填写文件名称
        private const string FilePath = @""; // 请填写文件路径
        private const string ImageName = ""; // 请填写图片名称
        private const string ImagePath = @""; // 请填写图片路径
        private const string OutDir = @""; // 请填写文件下载目录

        private const string Address = "127.0.0.1:7080"; // 请填写有度服务器地址

        private const int Buin = 16100865; // 请填写企业总机号码

        private const string EncodingaesKey = "nCgq3Depv/gWbP6Ki81XwMbdLZBvtOAIbOL/ygEW++Q="; // 请填写企业应用的EncodingaesKey

        private const string AppId = "yd09FB937746554796B8327F9FA942B80A"; // 请填写企业应用AppId

        private const string ToUsers = "cs1|cs2"; // 测试收收消息的账号

        private AppClient m_appClient = new AppClient(Address, Buin, AppId, EncodingaesKey);

        [TestMethod()]
        public void SendTextMsgTest()
        {
            var body = new TextBody("你好有度");
            var msg = new Message(ToUsers, Message.MessageTypeText, body);
            m_appClient.SendMsg(msg);
        }

        [TestMethod()]
        public void SendSysMsgTest()
        {
            var sysMsg = new SysmsgBody();
            sysMsg.Title = "有度即时通";
            sysMsg.addTextBody("有度即时通：");
            sysMsg.addLinkBody("https://youdu.im", "有度即时通", 0);
            var msg = new Message(ToUsers, Message.MessageTypeSysmsg, sysMsg);
            m_appClient.SendMsg(msg);
        }

        [TestMethod()]
        public void SendImageMsgTest()
        {
            var mediaId = m_appClient.UploadFile(AppClient.FileTypeImage, ImageName, ImagePath);
            var body = new ImageBody(mediaId);
            var msg = new Message(ToUsers, Message.MessageTypeImage, body);
            m_appClient.SendMsg(msg);
        }

        [TestMethod()]
        public void SendFileMsgTest()
        {
            var mediaId = m_appClient.UploadFile(AppClient.FileTypeFile, FileName, FilePath);
            var body = new FileBody(mediaId);
            var msg = new Message(ToUsers, Message.MessageTypeFile, body);
            m_appClient.SendMsg(msg);
        }

        [TestMethod()]
        public void SendMpnewsMsgTest()
        {
            var mediaId = m_appClient.UploadFile(AppClient.FileTypeImage, ImageName, ImagePath);
            var cell = new MpnewsBodyCell("你好有度", mediaId, "有度", "工作需要张弛有度");
            var body = new MpnewsBody(new List<MpnewsBodyCell> { cell });
            var msg = new Message(ToUsers, Message.MessageTypeMpnews, body);
            m_appClient.SendMsg(msg);
        }

        [TestMethod()]
        public void SendExlinkMsgTest()
        {
            var mediaId = m_appClient.UploadFile(AppClient.FileTypeImage, ImageName, ImagePath);
            var cell = new ExlinkBodyCell("你好有度", "https://youdu.im", "有度", mediaId);
            var body = new ExlinkBody(new List<ExlinkBodyCell> { cell });
            var msg = new Message(ToUsers, Message.MessageTypeExlink, body);
            m_appClient.SendMsg(msg);
        }

        [TestMethod()]
        public void UploadFileTest()
        {
            var mediaId = m_appClient.UploadFile(AppClient.FileTypeFile, FileName, FilePath);
            Console.WriteLine(mediaId);
        }

        [TestMethod()]
        public void DownloadFileTest()
        {
            var mediaId = m_appClient.UploadFile(AppClient.FileTypeFile, FileName, FilePath);
            Console.WriteLine(mediaId);
            var fileInfo = m_appClient.DownloadFile(mediaId, OutDir);
            Console.WriteLine(fileInfo.Item1);
            Console.WriteLine(fileInfo.Item2);
        }

        [TestMethod()]
        public void SearchFileTest()
        {
            var mediaId = m_appClient.UploadFile(AppClient.FileTypeFile, FileName, FilePath);
            Console.WriteLine(mediaId);
            var fileInfo = m_appClient.SearchFile(mediaId);
            Console.WriteLine(fileInfo.ToString());
        }

        [TestMethod()]
        public void TimestampTest()
        {
            long seconds = Helper.GetSecondTimeStamp();
            Console.WriteLine(seconds.ToString());
        }

        [TestMethod()]
        public void GetSessionTest()
        {
            var session = m_appClient.GetSession("{726C0689-AB55-422A-B4F7-8BA8A580AF9E}");
            Console.WriteLine(session.ToString());
        }

        [TestMethod()]
        public void SetAppNoticeTest() {
            m_appClient.SetAppNotice("max.chen",10,"");
        }

         [TestMethod()]
        public void PopWindowTest()
        {
            PopWindow win = new PopWindow();
            win.Url = "http://youdu.im";
            win.ToUser="max.chen";
            win.ToDept="1|2|3";
            win.Title="弹窗测试";
            win.Height = 500;
            win.Width = 500;
            win.Duration = 5;
            win.PopMode = Const.PopWindow_Mode_Window;
            //win.PopMode = Const.PopWindow_Mode_Browser;
            win.Position = Const.PopWindow_Position_BottomRight;
            m_appClient.PopWindow(win);
        }
    }
}