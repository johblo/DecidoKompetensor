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
        /*
        public static void CreateTermSet(SPWeb currentWeb,string TermGroup, string TermSetName)
        {
            //Guid employeHandBookTermSetId = new Guid(EmployeeHandbookTaxonomy.TermSetId);

            using (SPSite site = new SPSite(currentWeb.Site.Url))
            {
                TaxonomySession session = new TaxonomySession(site);

                if (session.TermStores.Count > 0)
                {
                    //TermStore store = session.TermStores[TermStoreName.TermStore];
                    TermStore store = session.DefaultKeywordsTermStore;
                    Group group = null;
                    try
                    {
                        group = store.Groups[TermStoreName.TermGroup];
                        TermSet termSet = group.CreateTermSet(TermSetName);
                        store.CommitAll();
                    }
                    catch (Exception ex)
                    { }
                    if (group == null)
                    {
                        group = store.CreateGroup(TermStoreName.TermGroup);
                        TermSet termSet = group.CreateTermSet(TermSetName);
                        store.CommitAll();
                    }
                }
            }
        }
        */
        public static void CreateTermSet(SPSite currentSite, string TermSetName)
        {
            //Guid employeHandBookTermSetId = new Guid(EmployeeHandbookTaxonomy.TermSetId);

            using (SPSite site = new SPSite(currentSite.Url))
            {
                TaxonomySession session = new TaxonomySession(site);

                if (session.TermStores.Count > 0)
                {
                    TermStore store = session.DefaultKeywordsTermStore;
                    Group group = null;
                    try
                    {
                        group = store.Groups[TermStoreName.TermGroup];
                        if(group!=null)
                            CheckAndCreate(TermSetName, store, group);
                    }
                    catch (Exception ex)
                    { }
                    if (group == null)
                    {
                        group = store.CreateGroup(TermStoreName.TermGroup);
                        if (group != null)
                            CheckAndCreate(TermSetName, store, group);
                    }
                }
            }
        }

        private static void CheckAndCreate(string TermSetName, TermStore store, Group group)
        {
            bool exist = false;
            foreach (TermSet termSetEnumerator in group.TermSets)
            {
                if (termSetEnumerator.Name.ToLower().Equals(TermSetName.ToLower()))
                {
                    exist = true;
                }
            }
            if (!exist)
            {
                TermSet termSet = group.CreateTermSet(TermSetName);
                store.CommitAll();
            }
        }

        public static SPField ConnectTaxonomyField(SPSite site, Guid fieldId, string termGroup, string termSetName)
        {
            if (site.RootWeb.Fields.Contains(fieldId))
            {
                TaxonomySession session = new TaxonomySession(site);
                if (session.DefaultKeywordsTermStore != null)
                {
                    // get the default metadata service application
                    var termStore = session.DefaultKeywordsTermStore;
                    var group = termStore.Groups.GetByName(termGroup);
                    var termSet = group.TermSets.GetByName(termSetName);
                    TaxonomyField field = site.RootWeb.Fields[fieldId] as TaxonomyField;
                    // connect the field to the specified term
                    field.SspId = termSet.TermStore.Id;
                    field.TermSetId = termSet.Id;
                    field.TargetTemplate = string.Empty;
                    field.AnchorId = Guid.Empty;
                    field.Update();
                    return field;
                }
            }
            return null;
        }
        
    }
    public static class Extension
    {
        public static Group GetByName(this GroupCollection col, string termGroup)
        {
            foreach (Group grp in col)
            {
                if (termGroup.ToLower().Equals(grp.Name.ToLower()))
                    return grp;
            }
            return null;
        }

        public static TermSet GetByName(this TermSetCollection col, string termSet)
        {
            foreach (TermSet term in col)
            {
                if (termSet.ToLower().Equals(term.Name.ToLower()))
                    return term;
            }
            return null;
        }
    }
}
