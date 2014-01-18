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

        private IncomingDelegate sessionIncomingDelegate = null;
        private OutgoingDelegate sessionOutgoingDelegate = null;
        private OTRSessionManager sessionManager = null;

        string me = null;
        string jid = null;
        Tags.jabber.client.message message = null;
        public OTRManager()
        {
        }

        public void incoming(Tags.jabber.client.message incomingMessage, IncomingDelegate callback)
        {
            if (sessionManager == null)
            {
                sessionIncomingDelegate = callback;
                message = incomingMessage;
                me = incomingMessage.to;
                jid = incomingMessage.from;
                sessionManager = new OTRSessionManager(me);
                sessionManager.OnOTREvent += new OTREventHandler(OnOTRMangerEventHandler);
                sessionManager.CreateOTRSession(jid);
            }
            sessionManager.ProcessOTRMessage(jid, incomingMessage.body);
            //var otr = new OTRMessage(incomingMessage);
            //otr.UpdatedBodyElements = new string[] { "WHATSUP!" };
            //callback(otr); 
        }

        public void outgoing(string selfJID, string otherJID, string message, OutgoingDelegate callback)
        {
            if (sessionManager == null)
            {
                sessionOutgoingDelegate = callback;
                me = selfJID;
                jid = otherJID;
                sessionManager = new OTRSessionManager(me);
                sessionManager.OnOTREvent += new OTREventHandler(OnOTRMangerEventHandler);
                sessionManager.CreateOTRSession(jid);
                sessionManager.RequestOTRSession(jid, OTRSessionManager.GetSupportedOTRVersionList()[0]);
            }
            else
            {
                sessionManager.EncryptMessage(jid, message);
            }
        }

        private void OnOTRMangerEventHandler(object source, OTREventArgs e)
        {
            switch (e.GetOTREvent())
            {
                case OTR_EVENT.MESSAGE:
                    if (sessionOutgoingDelegate != null)
                    {
                        sessionOutgoingDelegate(e.GetMessage());
                    }
                    break;
                case OTR_EVENT.SEND:
                    XMPPHelper.SendMessage(me, e.GetSessionID(), e.GetMessage());
                    break;
                case OTR_EVENT.READY:
                    sessionManager.EncryptMessage(e.GetSessionID(), e.GetMessage());
                    break;
                case OTR_EVENT.ERROR:
                    break;
                case OTR_EVENT.DEBUG:
                    break;
                case OTR_EVENT.CLOSED:
                    break;
            }
        }

    }
}
