using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Atkins.Intranet.Utilities.HelperUtils
{
    public class SecurityUtility
    {
        public static void CreateListGroup(SPWeb web, SPList currentList, string groupName, string groupDescription, SPRoleType roleType)
        {
            SPUserCollection users = web.SiteAdministrators;
            if (users.Count > 0)
            {
                SPUser siteAdmin = users[0];
                SPUser owner = users[0];
                SPMember member = users[0];
                SPGroupCollection groups = web.SiteGroups;
                bool groupExist = false;
                foreach (SPGroup grp in groups)
                {
                    if (grp.Name == groupName)
                    {
                        groupExist = true;
                    }
                }
                if(!groupExist)
                    groups.Add(groupName, owner, owner, groupName);
                SPRoleAssignment assignment = new SPRoleAssignment(web.SiteGroups[groupName]);
                SPRoleDefinition _role = web.RoleDefinitions.GetByType(roleType);
                assignment.RoleDefinitionBindings.Add(_role);
                currentList.RoleAssignments.Add(assignment);
                currentList.Update();
            }
        }
        
        public static SPRoleDefinition CreateDeviationCustomRoleDefinition(SPWeb web,string Name)
        {
            
            bool exist = false;
            foreach (SPRoleDefinition role in web.Site.RootWeb.RoleDefinitions)
            {
                if (role.Name == Name)
                {
                    exist = true;
                    break;
                }
            }
            if (!exist)
            {
                SPRoleDefinition roleDef = new SPRoleDefinition();
                roleDef.BasePermissions =
                SPBasePermissions.Open |
                SPBasePermissions.AddListItems |
                SPBasePermissions.ViewListItems |
                SPBasePermissions.ViewFormPages | 
                SPBasePermissions.ViewPages |
                SPBasePermissions.EditListItems;
                roleDef.Name = Name;
                web.Site.RootWeb.AllowUnsafeUpdates = true;
                web.Site.RootWeb.RoleDefinitions.Add(roleDef);
                web.Site.RootWeb.Update();
                web.Site.RootWeb.AllowUnsafeUpdates = true;
                return roleDef;
            }
            return null;


        }
        public static void AddExistingGroup(SPWeb web, SPList currentList, SPGroup group, SPRoleType roleType)
        {
            SPRoleAssignment assignment = new SPRoleAssignment(group);
            
            SPRoleDefinition _role = web.RoleDefinitions.GetByType(roleType);
            assignment.RoleDefinitionBindings.Add(_role);
            currentList.RoleAssignments.Add(assignment);
            currentList.Update();
        }
        public static void AddExistingGroupCustomDefinition(SPWeb web, SPList currentList, SPGroup group, SPRoleDefinition definition)
        {
            SPRoleAssignment assignment = new SPRoleAssignment(group);
            SPRoleDefinition _role = definition;
            assignment.RoleDefinitionBindings.Add(_role);
            currentList.RoleAssignments.Add(assignment);
            currentList.Update();
        }
    }
   
    
}
