using System;
using System.Collections.Generic;
using DILCore.Class.Model.LauncherAccount;

namespace DILCore.Interface;

public interface ILauncherAccountParser
{
    LauncherAccountModel LauncherAccount { get; }
    bool AddOrReplaceAccount(string uuid, AccountModel account, out Guid? id);
    bool RemoveAccount(Guid id);
    KeyValuePair<string, AccountModel>? Find(Guid id);
    bool ActivateAccount(string uuid);
    void Save();
}