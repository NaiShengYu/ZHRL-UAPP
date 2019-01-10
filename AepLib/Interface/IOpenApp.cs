using System;
using System.Collections.Generic;

namespace AepApp.Interface
{
    public interface IOpenApp
    {
        List<string> JudgeCanOpenAPP();

        string GetVersion();
    }
}
