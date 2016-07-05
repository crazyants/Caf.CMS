using CAF.WebSite.Application.Services.Media;
using CAF.Infrastructure.Core.Domain.Cms.Channels;
using CAF.Infrastructure.Core.Domain.Cms.Media;
using System;
using System.Collections.Generic;
using System.Linq;


namespace CAF.WebSite.Application.Services.Channels
{
    public static class ChannelExtensions
    {


        public static bool ChannelExtendedAttributeExists(this Channel channel, int ExtendedAttributeId)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");

            bool result = channel.ExtendedAttributes.ToList().Find(pt => pt.Id == ExtendedAttributeId) != null;
            return result;
        }
    }
}
