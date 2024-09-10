// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.MessageCenter.Dingtalk
{
    public class DingtalkMsg
    {
        public string msgtype { get; set; } = "text";

        public DingtalkContent text { get; set; }
    }

    public class DingtalkContent
    {
        public string content { get; set; }
    }
}