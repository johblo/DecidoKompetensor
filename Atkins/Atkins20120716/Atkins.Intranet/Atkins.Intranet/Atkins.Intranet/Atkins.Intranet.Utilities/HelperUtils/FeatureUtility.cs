using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Atkins.Intranet.Utilities.HelperUtils
{
    public class FeatureUtility
    {
        public static bool IsFeatureActivated(SPSite currentSite,Guid featureGuid)
        {
            foreach (SPFeature feature in currentSite.Features)
            {
                if (feature.DefinitionId == featureGuid)
                    return true;
            }
            return false;
        }
        public static bool IsFeatureActivated(SPWeb currentWeb, Guid featureGuid)
        {
            foreach (SPFeature feature in currentWeb.Features)
            {
                if (feature.DefinitionId == featureGuid)
                    return true;
            }
            return false;
        }
    }
}
