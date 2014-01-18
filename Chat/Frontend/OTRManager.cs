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

        string jid = null;
        OTRMessage message = null;
        public OTRManager()
        {
        }

        public void incoming(Tags.jabber.client.message incomingMessage, IncomingDelegate callback)
        {
            sessionIncomingDelegate = callback; 
            jid = incomingMessage.from;

            if (sessionManager == null)
            {
                sessionManager = new OTRSessionManager(incomingMessage.to);
                sessionManager.OnOTREvent += new OTREventHandler(OnOTRMangerEventHandler);
                sessionManager.CreateOTRSession(jid);
                //sessionManager.RequestOTRSession(jid, OTRSessionManager.GetSupportedOTRVersionList()[0]);
                message = new OTRMessage(incomingMessage);
            }
            
            
            sessionManager.ProcessOTRMessage(jid, incomingMessage.body);
            
        }

        public void outgoing(string selfJID, string otherJID, string message, OutgoingDelegate callback)
        {
            sessionOutgoingDelegate = callback;
            jid = otherJID;

            if (sessionManager == null || !sessionManager.IsSessionValid(jid))
            {
                sessionManager = new OTRSessionManager(selfJID);
                sessionManager.OnOTREvent += new OTREventHandler(OnOTRMangerEventHandler);
                sessionManager.CreateOTRSession(otherJID);
                sessionManager.RequestOTRSession(otherJID, OTRSessionManager.GetSupportedOTRVersionList()[0]);
            }
            
            //    sessionManager.EncryptMessage(otherJID, message);
            
            //callback("ENCRYPTED" + message);
        }

        private void OnOTRMangerEventHandler(object source, OTREventArgs e)
        {
            switch (e.GetOTREvent())
            {
                case OTR_EVENT.MESSAGE:
                    //log.Text += String.Format("{0}: {1} \n", e.GetSessionID(), e.GetMessage());
                    //_bob_otr_session_manager.EncryptMessage(_bob_my_buddy_unique_id, "Hello");
                    if (sessionIncomingDelegate != null)
                    {
                        message.UpdatedBodyElements = new string[] {e.GetMessage()};
                        sessionIncomingDelegate(message);
                        sessionIncomingDelegate = null;
                    }
                    break;
                case OTR_EVENT.SEND:
                    message.UpdatedBodyElements = new string[] {e.GetMessage()};
                    XMPPHelper.SendMessage(message.to, message.from, e.GetMessage());      
                    break;
                case OTR_EVENT.READY:
                    if (sessionOutgoingDelegate != null)
                    {
                        sessionManager.EncryptMessage(jid, e.GetMessage());
                    }
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
