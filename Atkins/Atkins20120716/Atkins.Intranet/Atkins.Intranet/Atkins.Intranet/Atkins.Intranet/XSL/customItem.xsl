<xsl:stylesheet
  version="1.0"
  exclude-result-prefixes="x d xsl msxsl cmswrt"
  xmlns:x="http://www.w3.org/2001/XMLSchema"
  xmlns:d="http://schemas.microsoft.com/sharepoint/dsp"
  xmlns:cmswrt="http://schemas.microsoft.com/WebParts/v3/Publishing/runtime"
  xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt">
  <xsl:param name="ItemsHaveStreams">
    <xsl:value-of select="'False'" />
  </xsl:param>
  <xsl:variable name="OnClickTargetAttribute" select="string('javascript:this.target=&quot;_blank&quot;')" />
  <xsl:variable name="ImageWidth" />
  <xsl:variable name="ImageHeight" />

  <xsl:template name="ShowXML" match="Row[@Style='ShowXML']" mode="itemstyle">
    <xsl:for-each select="@*">
      <br />
      NameNN: <xsl:value-of select="name()" />
      <br />Value:<xsl:value-of select="." />
    </xsl:for-each>
  </xsl:template>
  <xsl:template name="removeMarkup">
    <xsl:param name="string" />
    <xsl:choose>
      <xsl:when test="contains($string, '&lt;')">
        <xsl:variable name="nextString">
          <xsl:call-template name="removeMarkup">
            <xsl:with-param name="string" select="substring-after($string, '&gt;')" />
          </xsl:call-template>
        </xsl:variable>
        <xsl:value-of select="concat(substring-before($string, '&lt;'), $nextString)" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$string" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="CQWP_Blog" match="Row[@Style='CQWP_Blog']" mode="itemstyle">
    
    <xsl:variable name="SafeLinkUrl">
      <xsl:call-template name="OuterTemplate.GetSafeLink">
        <xsl:with-param name="UrlColumnName" select="'LinkUrl'"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="SafeImageUrl">
      <xsl:call-template name="OuterTemplate.GetSafeStaticUrl">
        <xsl:with-param name="UrlColumnName" select="'ImageUrl'"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="DisplayTitle">
      <xsl:call-template name="OuterTemplate.GetTitle">
        <xsl:with-param name="Title" select="@Title"/>
        <xsl:with-param name="UrlColumnName" select="'LinkUrl'"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="LinkTarget">
      <xsl:if test="@OpenInNewWindow = 'True'" >_blank</xsl:if>
    </xsl:variable>
    <xsl:variable name="bodyContent">
      <xsl:call-template name="removeMarkup">
        <!--<xsl:with-param name="string" select="$bodyContent" />-->
        <xsl:with-param name="string" select="substring(@Body,1,300)" />
      </xsl:call-template>
    </xsl:variable>
    <div id="linkitem" class="announcementItem">
      <xsl:if test="string-length($SafeImageUrl) != 0">
        <div class="image-area-left">
          <a href="{$SafeLinkUrl}" target="{$LinkTarget}">
            <img class="image" src="{$SafeImageUrl}" alt="{@ImageUrlAltText}" />
          </a>
        </div>
      </xsl:if>
      <div class="link-item">
        <xsl:call-template name="OuterTemplate.CallPresenceStatusIconTemplate"/>
        <a href="{$SafeLinkUrl}" target="{$LinkTarget}" title="{@LinkToolTip}">
          <span class="ms-announcementtitle">
            <xsl:value-of select="$DisplayTitle"/>
          </span> (skriven <xsl:value-of select="@Created" /> av <xsl:value-of select="@Author" />)
        </a>
        <div class="description">
          <!--<xsl:value-of select="substring($bodyContent,1,1000)" disable-output-escaping="yes" />-->
          <xsl:value-of select="$bodyContent" disable-output-escaping="yes" />
          ...(<a href="{$SafeLinkUrl}" mce_href="{$SafeLinkUrl}" target="{$LinkTarget}" title="{@LinkToolTip}">läs mer</a>)
          <br />
          <xsl:value-of select="@PublishedDate" />
        </div>
      </div>
    </div>
  </xsl:template>
  <xsl:template name="Announcements" match="Row[@Style='Announcements']" mode="itemstyle">
    <xsl:variable name="CurPosition" select="count(./preceding-sibling::*)" />
    <xsl:variable name="bgcolor">
      <xsl:choose>
        <xsl:when test="$CurPosition mod 2 = 0">
          <xsl:text>oddRow</xsl:text>
        </xsl:when>
        <xsl:otherwise>
          <xsl:text>evenRow</xsl:text>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="SiteLink" select="@SiteLink"/>

    <xsl:variable name="SafeLinkUrl">
      <xsl:call-template name="OuterTemplate.GetSafeLink">
        <xsl:with-param name="UrlColumnName" select="'LinkUrl'"/>
      </xsl:call-template>
    </xsl:variable>
    <xsl:variable name="DisplayTitle">
      <xsl:call-template name="OuterTemplate.GetTitle">
        <xsl:with-param name="Title" select="@Title"/>
        <xsl:with-param name="UrlColumnName" select="'LinkUrl'"/>
      </xsl:call-template>
    </xsl:variable>
    <div class="{$bgcolor}">
      
      <a href="{$SafeLinkUrl}" title="{@LinkToolTip}">
        <span class="ms-announcementtitle">
          <xsl:value-of select="$SiteLink"/>
        </span>
        <xsl:value-of select="$DisplayTitle"/>
      </a>
    </div>
  </xsl:template>

  <xsl:template name="SPWebUrl">
    <xsl:param name="siteUrl" />
    <xsl:param name="contentclass" />
    <xsl:choose>
      <!-- Check the content class to see if it is a document -->
      <xsl:when test="$contentclass='STS_ListItem_DocumentLibrary'">
        <!-- Get Document Library Url Name -->
        <xsl:variable name="DocLib">
          <xsl:call-template name="StripSlash">
            <xsl:with-param name="text" select="$siteUrl"/>
          </xsl:call-template>
        </xsl:variable>
        <!-- Remove the document library from the url -->
        <xsl:variable name="SPWebURLString" select="substring-before(concat($siteUrl, '/'), concat('/', concat($DocLib, '/')))" />
        <xsl:value-of select="$SPWebURLString"/>
      </xsl:when>
      <xsl:otherwise>
        <!-- Get List Url Name -->
        <xsl:variable name="ListUrl">
          <xsl:call-template name="StripSlash">
            <xsl:with-param name="text" select="$siteUrl"/>
          </xsl:call-template>
        </xsl:variable>
        <!-- Remove the list name from the url -->
        <xsl:variable name="urlLists" select="substring-before(concat($siteUrl, '/'), concat('/', concat($ListUrl, '/')))" />
        <!-- Remove Lists from the url -->
        <xsl:variable name="Lists">
          <xsl:call-template name="StripSlash">
            <xsl:with-param name="text" select="$urlLists"/>
          </xsl:call-template>
        </xsl:variable>
        <xsl:variable name="SPWebURLString" select="substring-before(concat($urlLists, '/'), concat('/', concat($Lists, '/')))" />
        <xsl:value-of select="$SPWebURLString"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="StripSlash">
    <xsl:param name="text"/>
    <xsl:choose>
      <xsl:when test="contains($text, '/')">
        <xsl:call-template name="StripSlash">
          <xsl:with-param name="text" select="substring-after($text, '/')"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$text"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>

<!--<a>
  <xsl:attribute name="href">
    <xsl:call-template name="SPWebUrl">
      <xsl:with-param name="siteUrl" select="sitename" />
      <xsl:with-param name="contentclass" select="contentclass" />
    </xsl:call-template>
  </xsl:attribute>
  <xsl:value-of select="sitetitle"/>
</a>-->
