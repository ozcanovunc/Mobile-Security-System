﻿#pragma checksum "C:\Users\ozcan\Documents\GitHub\Mobile-Security-System\Mobile Security System\SigninPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "55C3FACA319137EBA49AEB338B971A0F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Mobile_Security_System {
    
    
    public partial class LoginPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.TextBox LoginPage_MailText;
        
        internal System.Windows.Controls.PasswordBox LoginPage_PasswordText;
        
        internal System.Windows.Controls.Button LoginPage_SigninButton;
        
        internal System.Windows.Controls.Button LoginPage_SignupButton;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Mobile%20Security%20System;component/SigninPage.xaml", System.UriKind.Relative));
            this.LoginPage_MailText = ((System.Windows.Controls.TextBox)(this.FindName("LoginPage_MailText")));
            this.LoginPage_PasswordText = ((System.Windows.Controls.PasswordBox)(this.FindName("LoginPage_PasswordText")));
            this.LoginPage_SigninButton = ((System.Windows.Controls.Button)(this.FindName("LoginPage_SigninButton")));
            this.LoginPage_SignupButton = ((System.Windows.Controls.Button)(this.FindName("LoginPage_SignupButton")));
        }
    }
}

