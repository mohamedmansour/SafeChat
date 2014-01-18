using Backend.Data;
using OTR.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMPP;
using Tags = XMPP.tags;

namespace Chat.Frontend
{
    public class OTRManager
    {
        public delegate void IncomingDelegate(OTRMessage decryptedMessage);
        public delegate void OutgoingDelegate(string encryptedMessage);
        
        OTRSessionManager sessionManager = null;
        
        public OTRManager()
        {
        }

        public void incoming(Tags.jabber.client.message incomingMessage, IncomingDelegate callback)
        {
            var message = new OTRMessage(incomingMessage);
            callback(message);
        }

        public void outgoing(string selfJID, string otherJID, string message, OutgoingDelegate callback)
        {
            callback("ENCRYPTED" + message);
        }
    }
}
