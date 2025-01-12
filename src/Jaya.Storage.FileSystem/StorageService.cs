﻿using Jaya.Shared.Storage;

namespace Jaya.Storage.FileSystem
{
    public class StorageService : Service
    {
        public StorageService() : base("File System", new(1, 0))
        {
            Description = "View your local drives, inspect their properties and play with directories & files stored within them.";
            ImageData = "M21,14H3V4H21M21,2H3C1.89,2 1,2.89 1,4V16A2,2 0 0,0 3,18H10L8,21V22H16V21L14,18H21A2,2 0 0,0 23,16V4C23,2.89 22.1,2 21,2Z";
            IsNotificationEnabled = true;
        }
    }
}
