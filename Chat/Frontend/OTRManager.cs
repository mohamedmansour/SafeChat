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
        OTRMessage message = null;
        public OTRManager()
        {
        }

        public void incoming(Tags.jabber.client.message incomingMessage, IncomingDelegate callback)
        {
            /*sessionIncomingDelegate = callback;
            if (sessionManager == null)
            {
                message = new OTRMessage(incomingMessage);
                me = incomingMessage.to;
                jid = incomingMessage.from;
                sessionManager = new OTRSessionManager(me);
                sessionManager.OnOTREvent += new OTREventHandler(OnOTRMangerEventHandler);
                sessionManager.CreateOTRSession(jid);
            }
            sessionManager.ProcessOTRMessage(jid, incomingMessage.body);
            */var otr = new OTRMessage(incomingMessage);
            otr.UpdatedBodyElements = new string[] { "WHATSUP!" };
            callback(otr); 
        }

        public void outgoing(string selfJID, string otherJID, string outgoingMsg, OutgoingDelegate callback)
        {
            callback(outgoingMsg);
            /*
            sessionOutgoingDelegate = callback;
            if (sessionManager == null)
            {
                me = selfJID;
                jid = otherJID;
                sessionManager = new OTRSessionManager(me);
                sessionManager.OnOTREvent += new OTREventHandler(OnOTRMangerEventHandler);
                sessionManager.CreateOTRSession(jid);
                sessionManager.RequestOTRSession(jid, OTRSessionManager.GetSupportedOTRVersionList()[0]);
            }
            else
            {
                sessionManager.EncryptMessage(jid, outgoingMsg);
            }*/
        }

        private void OnOTRMangerEventHandler(object source, OTREventArgs e)
        {
            switch (e.GetOTREvent())
            {
                case OTR_EVENT.MESSAGE:
                    if (sessionIncomingDelegate != null)
                    {
                        message.UpdatedBodyElements = new string[] { e.GetMessage() };
                        sessionIncomingDelegate(message);
                        sessionIncomingDelegate = null;
                    }
                    break;
                case OTR_EVENT.SEND:
                    XMPPHelper.SendMessage(me, e.GetSessionID(), e.GetMessage());
                    break;
                case OTR_EVENT.READY:
                    sessionManager.EncryptMessage(e.GetSessionID(), e.GetMessage());
                    break;
                case OTR_EVENT.ERROR:
                    sessionManager.CloseAllSessions();
                    sessionManager = null;
                    break;
                case OTR_EVENT.DEBUG:
                    break;
                case OTR_EVENT.CLOSED:
                    sessionManager.CloseAllSessions();
                    sessionManager = null;
                    break;
            }
        }

    }
}
