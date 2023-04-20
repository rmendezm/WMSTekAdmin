<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" indent="yes" encoding="utf-8"/>
	<!-- Find the root node called Menus 
       and call MenuListing for its children -->
	<xsl:template match="/Menus">
		<MenuItems>
			<xsl:call-template name="MenuListing" />
		</MenuItems>
	</xsl:template>

	<!-- Allow for recusive child node processing -->
	<xsl:template name="MenuListing">
		<xsl:apply-templates select="Menu" />
	</xsl:template>

	<xsl:template match="Menu">
		<MenuItem>
			<!-- Convert Menu child elements to MenuItem attributes -->

			<xsl:attribute name="Text">
				<xsl:value-of select="TextValue"/>
			</xsl:attribute>

			<xsl:attribute name="Target">
				<xsl:text>_self</xsl:text>
				<!--  <xsl:value-of select="aspxPage"/> -->
			</xsl:attribute>

			<xsl:attribute name="NavigateUrl">
				<xsl:text>javascript:openWindow('</xsl:text>
				<xsl:value-of select="Name"/>
				<xsl:text>', '</xsl:text>
				<xsl:value-of select="TextValue"/>
				<xsl:text>', '</xsl:text>
				<xsl:value-of select="WinPath"/>
				<xsl:text>', '</xsl:text>
				<xsl:value-of select="ShortTitle"/>
				<xsl:text>', '</xsl:text>
				<xsl:value-of select="AspxPage"/>
				<xsl:text>', </xsl:text>
				<xsl:value-of select="MaxOpenedPages"/>
				<xsl:text>, '</xsl:text>
				<xsl:value-of select="MaxOpenedPagesMessage"/>
				<xsl:text>');</xsl:text>
			</xsl:attribute>

			<xsl:attribute name="Selectable">
				<xsl:choose>
					<xsl:when test="AspxPage != '' ">
						<xsl:text>True</xsl:text>
					</xsl:when>
					<xsl:otherwise>
						<xsl:text>False</xsl:text>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>


			<!-- Call MenuListing if there are child Menu nodes -->
			<xsl:if test="count(Menu) > 0">
				<xsl:call-template name="MenuListing" />
			</xsl:if>
		</MenuItem>
	</xsl:template>
</xsl:stylesheet>