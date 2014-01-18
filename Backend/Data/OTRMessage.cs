using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using XMPP;
using XMPP.tags.jabber.client;
using Tags = XMPP.tags;

namespace Backend.Data
{
    public class OTRMessage : Tags.jabber.client.message
    {
        public OTRMessage(Tags.jabber.client.message message) : base(message)
        {
            this.UpdatedBodyElements = new List<string>();
        }

        public IEnumerable<string> UpdatedBodyElements
        {
            get;
            set;
        }

        public IEnumerable<string> MessageBodyElements
        {
            get
            {
                if (this.UpdatedBodyElements.Count() > 0)
                {
                    return from n in this.UpdatedBodyElements select n;
                }
                else
                {
                    return from n in this.bodyElements select n.Value;
                }
            }
        }
    }
}
