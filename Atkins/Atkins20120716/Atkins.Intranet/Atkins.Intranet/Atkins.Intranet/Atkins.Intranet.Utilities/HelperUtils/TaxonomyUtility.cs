using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace Atkins.Intranet.Utilities.HelperUtils
{
    public class TaxonomyUtility
    {

        public static void CreateTermSet(SPWeb currentWeb,string TermGroupName,string TermSetName)
        {
            //Guid employeHandBookTermSetId = new Guid(EmployeeHandbookTaxonomy.TermSetId);
            using (SPSite site = new SPSite(currentWeb.Site.Url))
            {
                TaxonomySession session = new TaxonomySession(site);
                if (session.TermStores.Count > 0)
                {
                    TermStore store = session.TermStores[TermStoreName.TermStore];
                    //store.Languages.Add(1053);

                    Group group = null;
                    try
                    {
                        group = store.Groups[TermGroupName];
                        TermSet termSet = group.CreateTermSet(TermSetName);
                        store.CommitAll();
                    }
                    catch (Exception ex)
                    { }
                    if (group == null)
                    {
                        group = store.CreateGroup(TermGroupName);
                        TermSet termSet = group.CreateTermSet(TermSetName);
                        store.CommitAll();
                    }
                }
            }
        }
    }
}
