using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Infrastructure.Helpers.Settings;

namespace Infrastructure.Helpers
{
    public class Notification
    {
        private string smtpSettings = Config.SMTP_SETTINGS;
        private string senderName = Config.SENDER_NAME;
        private MailboxAddress senderEmail = new MailboxAddress(Config.SENDER_EMAIL);

        // TODOs: The Approving Officer (AO) will receive an email notification of the request. DONE
        // (AO) can use the link included in the email to access the BEM System. 
        // Notify the Maker if approved (Update/ Delete/ Regularize). Done
        // Notify the Maker if rejected. with Remarks        
        public void SendApproverNotification(string[] contentDetails, List<string> recipients)
        {
            InternetAddressList recipientADList = new InternetAddressList();

            foreach (var item in recipients)
            {
                if (!recipientADList.Contains(new MailboxAddress(item)))
                {
                    recipientADList.Add(new MailboxAddress(item));
                }
            }

            if (recipientADList.Count != 0)
            {
                BodyBuilder bodyBuilder = ForApprovalMessage(contentDetails);

                foreach (var item in recipientADList)
                {
                    var message = new MimeMessage();
                    message.From.Add(senderEmail);
                    message.Subject = "CCEMS Notification - Request For Approval";
                    message.Body = bodyBuilder.ToMessageBody();
                    message.To.Add(item);
                    SendEmailNotification(smtpSettings, message);
                }
            }
        }
        public void SendMakerNotification(string[] contentDetails, string recipient, bool isApproved)
        {
            BodyBuilder bodyBuilder = new BodyBuilder(); ;

            if (isApproved)
            {
                bodyBuilder = ApprovedMessage(contentDetails);
            }
            else
            {
                bodyBuilder = RejectedMessage(contentDetails);
            }

            var message = new MimeMessage();
            message.From.Add(senderEmail);
            message.To.Add(new MailboxAddress(recipient));
            message.Subject = "CCEMS Notification - Request For Approval";
            message.Body = bodyBuilder.ToMessageBody();
            SendEmailNotification(smtpSettings, message);

        }
        public void SendBranchNotification(string[] contentDetails, List<string> toRecipients, List<string> ccRecipients)
        {
            InternetAddressList toRecipientADList = new InternetAddressList();
            InternetAddressList ccRecipientADList = new InternetAddressList();

            foreach (var item in toRecipients)
            {
                var email = item.Trim();
                if (!toRecipientADList.Contains(new MailboxAddress(email)))
                {
                    toRecipientADList.Add(new MailboxAddress(email));
                }
            }

            foreach (var item in ccRecipients)
            {
                var email = item.Trim();
                if (!ccRecipientADList.Contains(new MailboxAddress(email)))
                {
                    ccRecipientADList.Add(new MailboxAddress(email));
                }
            }

            if (toRecipientADList.Count != 0)
            {
                BodyBuilder bodyBuilder = BranchMessage(contentDetails);

                var message = new MimeMessage();
                message.From.Add(senderEmail);
                message.Subject = "CCEMS Notification";
                message.Body = bodyBuilder.ToMessageBody();
                message.To.AddRange(toRecipientADList);
                message.Cc.AddRange(ccRecipientADList);
                SendEmailNotification(smtpSettings, message);
            }
        }

        #region Private Methods

        // Note: 0 = Branch/Group, 1 = Employee ID/Name, 2 = Action, 3 = ReferenceNo, 4 = Redirect Url
        private static BodyBuilder ForApprovalMessage(string[] contentDetails)
        {
            BodyBuilder bodyBuilder = new BodyBuilder();
            string msg = File.ReadAllText($@"{Settings.Config.EMAILCONTENT_PATH}\ForApprovalMsg.html");
            bodyBuilder.HtmlBody = string.Format(msg, contentDetails[0], contentDetails[1], contentDetails[2], contentDetails[3], contentDetails[4]);
            return bodyBuilder;
        }
        // Note: 0 = Branch/Group, 1 = Employee ID/Name, 2 = Action, 3 = ReferenceNo, 4 = Redirect Url
        private static BodyBuilder ApprovedMessage(string[] contentDetails)
        {
            BodyBuilder bodyBuilder = new BodyBuilder();
            string msg = File.ReadAllText($@"{Settings.Config.EMAILCONTENT_PATH}\ApprovedMsg.html");
            bodyBuilder.HtmlBody = string.Format(msg, contentDetails[0], contentDetails[1], contentDetails[2], contentDetails[3], contentDetails[4]);

            return bodyBuilder;
        }
        // Note: 0 = Branch/Group, 1 = Employee ID/Name, 2 = Action, 3 = ReferenceNo, 4 = Reject Remarks
        private static BodyBuilder RejectedMessage(string[] contentDetails)
        {
            BodyBuilder bodyBuilder = new BodyBuilder();

            string msg = File.ReadAllText($@"{Settings.Config.EMAILCONTENT_PATH}\RejectedMsg.html");

            bodyBuilder.HtmlBody = string.Format(msg, contentDetails[0], contentDetails[1], contentDetails[2], contentDetails[3], contentDetails[4], contentDetails[5]);

            return bodyBuilder;
        }
        // Note: 0 = Report Name, 1 = Branch Name, 2 = Report ID, 3 = URL
        private static BodyBuilder BranchMessage(string[] contentDetails)
        {
            BodyBuilder bodyBuilder = new BodyBuilder();
            string msg = File.ReadAllText($@"{Settings.Config.EMAILCONTENT_PATH}\BranchMsg.html");
            bodyBuilder.HtmlBody = string.Format(msg, contentDetails[0], contentDetails[1], contentDetails[2], contentDetails[3]);

            return bodyBuilder;
        }

        private void SendEmailNotification(string smtpSettings, MimeMessage message)
        {
            string[] settings = smtpSettings.Split(';');
            string ip = settings[0];
            int port = Convert.ToInt16(settings[1]);
            bool isSSL = Convert.ToBoolean(settings[2]);

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(ip, port, isSSL);
                    client.Capabilities &= ~SmtpCapabilities.Chunking;
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    //log..
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
        public void PulloutNotification(string recipient, string fileName)
        {
            BodyBuilder bodyBuilder = new BodyBuilder(); ;

            string msg = File.ReadAllText($@"{Settings.Config.EMAILCONTENT_PATH}\PulloutMsg.html");
            bodyBuilder.HtmlBody = string.Format(msg);
            
            // We may also want to attach a calendar event for Monica's party...
            bodyBuilder.Attachments.Add(fileName);

            var message = new MimeMessage();
            message.From.Add(senderEmail);
            message.To.Add(new MailboxAddress(recipient));
            message.Subject = "CCEMS Notification - Pullout request";
            message.Body = bodyBuilder.ToMessageBody();
            
            SendEmailNotification(smtpSettings, message);

        }
        #endregion

    }
}
