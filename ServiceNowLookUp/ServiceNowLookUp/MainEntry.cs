using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceNowLookUp
{
    class MainEntry
    {
        static void Main(string[] args)
        {
            try
            {
                CheckIncidents cin = new CheckIncidents();
                cin.CheckAndDeleteIncidents();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
