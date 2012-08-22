using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint.Publishing.Navigation;

namespace CustomSPNavigationProvider
{
    public class MyCustomSiteMapProvider : PortalSiteMapProvider
    {
        SiteMapNodeCollection siteMapNodeColl = null;


        public override SiteMapNodeCollection GetChildNodes(System.Web.SiteMapNode node)
        {
            PortalSiteMapNode pNode = node as PortalSiteMapNode;
            if (pNode != null)
            {
                if (pNode.Type == NodeTypes.Area)
                {
                    SiteMapNodeCollection nodeColl = base.GetChildNodes(pNode);


                    //We can use SharePoint list or XML file to make our navigation configurable.


                    SiteMapNode childNode = new SiteMapNode(this, "<http://www.mainsite.com>",
                    "<http://www.mainsite.com>", "Root site");


                    SiteMapNode childNode1 = new SiteMapNode(this, "<http://www.level1site.com>",
                    "<http://www.level1site.com>", "Level 1 Site");


                    SiteMapNode childNode2 = new SiteMapNode(this, "<http://www.level2site.com>",
                    "<http://www.level2site.com>", "Level 2 Site");


                    SiteMapNode childNode11 = new SiteMapNode(this,
        "<http://www.level11site.com>", "<http://www.level11site.com>", "Subsite level 11");


                    SiteMapNode childNode12 = new SiteMapNode(this, "<http://www.level12site.com>",
                                "<http://www.level12site.com>", "Subsite level 12");


                    SiteMapNode childNode111 = new SiteMapNode(this, "<http://www.level111site.com>",
                                "<http://www.level111site.com>", "Site Pages 1");


                    SiteMapNode childNode112 = new SiteMapNode(this, "<http://www.level112site.com>",
                                "<http://www.level112site.com>", "Site Pages 2");


                    nodeColl.Add(childNode);


                    siteMapNodeColl = new SiteMapNodeCollection();
                    siteMapNodeColl.Add(childNode111);
                    siteMapNodeColl.Add(childNode112);


                    childNode12.ChildNodes = siteMapNodeColl;


                    siteMapNodeColl = new SiteMapNodeCollection();
                    siteMapNodeColl.Add(childNode11);
                    siteMapNodeColl.Add(childNode12);


                    childNode1.ChildNodes = siteMapNodeColl;


                    siteMapNodeColl = new SiteMapNodeCollection();
                    siteMapNodeColl.Add(childNode1);
                    siteMapNodeColl.Add(childNode2);
                    childNode.ChildNodes = siteMapNodeColl;


                    return nodeColl;
                }
                else
                    return base.GetChildNodes(pNode);
            }
            else
                return new SiteMapNodeCollection();
        }
    }
}
