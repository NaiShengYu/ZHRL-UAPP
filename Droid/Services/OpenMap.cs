using System;
using AepApp.Interface;
[assembly: Dependency(typeof(OpenMap))]

namespace AepApp.Droid.Services
{
    public interface OpenMap : IOpenApp
    {


        public List<string> JudgeCanOpenAPP()
        {
        
            List<string> aaa = new List<string>();



            return aaa;
        }
    }
}
