using Moiro_Orders.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;

namespace Moiro_Orders
{
    class ActiveDyrectoryController :IDisposable
    {
        public ActiveDyrectoryController() { }

        List<User> allUsers = new List<User>();
        DirectoryEntry searchRoot;

        public List<User> GetNewADUsers()
        {
            searchRoot = new DirectoryEntry("LDAP://" + "moiro.bel", null, null, AuthenticationTypes.Secure);
            DirectorySearcher search = new DirectorySearcher(searchRoot)
            {
                Filter = "(&(objectClass=user)(objectCategory=person))"
            };
            search.PropertiesToLoad.Add("distinguishedname");
            SearchResult result, result2;
            SearchResultCollection resultCol = search.FindAll();
            search.PropertiesToLoad.Add("samaccountname");
            SearchResultCollection resultCol2 = search.FindAll();

            if (resultCol == null)
            {
                return allUsers;
            }
            for (int counter = 0; counter < resultCol.Count; counter++)
            {
                result = resultCol[counter];
                result2 = resultCol2[counter];
                if (result.Properties.Contains("distinguishedname"))
                {
                    string ulongData = result.Properties["distinguishedname"][0].ToString();
                    string login = result2.Properties["samaccountname"][0].ToString();
                    // allUsers.Add(ulongData);
                    int startIndex = ulongData.IndexOf("OU=", 1) + 3; //+3 for  length of "OU="
                    int endIndex = ulongData.IndexOf(",", startIndex);
                    int startIndexCN = ulongData.IndexOf("CN=", 1) + 3;
                    int endIndexCN = ulongData.IndexOf(",", startIndexCN);
                    var group = ulongData.Substring((startIndex), (endIndex - startIndex));
                    var name = ulongData.Substring(startIndexCN, endIndexCN - startIndexCN).Trim('=');
                    if (name[0] >= 0x0400 && name[0] <= 0x04FF)
                    {
                        allUsers.Add(new User
                        {
                            FullName = name,
                            Login = login,
                            OrganizationalUnit = group
                        });
                    }
                }
            }
            return allUsers;
        }

        public void Dispose()
        {
            allUsers = null;
            searchRoot.Dispose();
        }
    }
    
}
