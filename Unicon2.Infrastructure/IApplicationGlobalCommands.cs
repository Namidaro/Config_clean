﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Unicon2.Infrastructure.Common;

namespace Unicon2.Infrastructure
{
    public interface IApplicationGlobalCommands
    {
        void ShutdownApplication();
        void ExecuteByDispacther(Action action);
        void ShowWindowModal(Func<Window> getWindow, object dataContext);
        bool AskUserToDeleteSelectedGlobal(object context);
        void ShowErrorMessage(string errorKey, object context);
        void SetToBuffer(object bufferObject);
        object GetFromBuffer();
        Task CallWaitingProgressWindow(object context,bool isToOpen);
        void OpenOscillogram(string oscillogramPath=null);


        Maybe<FileInfo> SelectFileToOpen(string windowTitle,params string[] filters);
        Maybe<string> SelectFilePathToSave(string windowTitle,string defaultExtension,params string[] filters);

    }
}