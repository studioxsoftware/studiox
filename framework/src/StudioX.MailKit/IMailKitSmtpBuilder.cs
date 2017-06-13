using System;
using MailKit.Net.Smtp;

namespace StudioX.MailKit
{
    public interface IMailKitSmtpBuilder
    {
        SmtpClient Build();
    }
}