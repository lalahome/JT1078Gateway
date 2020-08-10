﻿using JT1078.Gateway.Configurations;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JT1078.Gateway
{
    /// <summary>
    /// 协调器客户端
    /// </summary>
    public class JT1078CoordinatorHttpClient
    {
        private HttpClient httpClient;

        private JT1078Configuration Configuration;

        private const string endpoint = "/JT1078WebApi";

        public JT1078CoordinatorHttpClient(IOptions<JT1078Configuration> configurationAccessor)
        {
            Configuration = configurationAccessor.Value;
            this.httpClient = new HttpClient();
            this.httpClient.BaseAddress = new Uri(Configuration.CoordinatorUri);
            this.httpClient.Timeout = TimeSpan.FromSeconds(3);
        }

        /// <summary>
        /// 发送重制至协调器中
        /// </summary>
        public async ValueTask Reset()
        {
            await httpClient.GetAsync($"{endpoint}/reset");
        }

        /// <summary>
        /// 发送心跳至协调器中
        /// </summary>
        /// <param name="content"></param>
        public async ValueTask Heartbeat(string content)
        {
            await httpClient.PostAsync($"{endpoint}/heartbeat", new StringContent(content));
        }

        /// <summary>
        /// 发送设备号和通道给协调器中
        /// </summary>
        /// <param name="terminalPhoneNo"></param>
        /// <param name="channelNo"></param>
        public async ValueTask ChannelClose(string terminalPhoneNo,int channelNo)
        {
            //todo:通过自维护，当协调重启导致集群内网关未关闭的情况下，通过轮询的方式再去调用
            string json = $"{{\"TerminalPhoneNo\":\"{terminalPhoneNo}\",\"ChannelNo\":\"{channelNo}\"}}";
            await httpClient.PostAsync($"{endpoint}/ChannelClose", new StringContent(json));
        }
    }
}
