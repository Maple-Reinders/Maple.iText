/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Geom;
using iText.StyledXmlParser.Css;
using iText.StyledXmlParser.Css.Util;
using iText.Svg;
using iText.Svg.Logs;
using iText.Svg.Renderers;
using iText.Svg.Utils;

namespace iText.Svg.Renderers.Impl {
    /// <summary>
    /// <see cref="iText.Svg.Renderers.ISvgNodeRenderer"/>
    /// implementation for the &lt;marker&gt; tag.
    /// </summary>
    public class MarkerSvgNodeRenderer : AbstractBranchSvgNodeRenderer {
        /// <summary>Attribute defining the marker index on polygon, line or polyline.</summary>
        /// <remarks>
        /// Attribute defining the marker index on polygon, line or polyline.
        /// It is not a property from css standard, used for internal marker processing.
        /// </remarks>
        public const String MARKER_INDEX = "marker-index";

        // Default marker width in point units (3 px)
        private const float DEFAULT_MARKER_WIDTH = 2.25f;

        // Default marker height in point units (3 px)
        private const float DEFAULT_MARKER_HEIGHT = 2.25f;

        // Default refX value
        private const float DEFAULT_REF_X = 0f;

        // Default refY value
        private const float DEFAULT_REF_Y = 0f;

        public override ISvgNodeRenderer CreateDeepCopy() {
            MarkerSvgNodeRenderer copy = new MarkerSvgNodeRenderer();
            DeepCopyAttributesAndStyles(copy);
            DeepCopyChildren(copy);
            return copy;
        }

        public override Rectangle GetObjectBoundingBox(SvgDrawContext context) {
            return null;
        }

        protected internal override bool IsHidden() {
            return CommonCssConstants.HIDDEN.Equals(this.attributesAndStyles.Get(CommonCssConstants.VISIBILITY));
        }

//\cond DO_NOT_DOCUMENT
        internal override void PreDraw(SvgDrawContext context) {
            base.PreDraw(context);
            float[] markerWidthHeight = GetMarkerWidthHeightValues();
            float markerWidth = markerWidthHeight[0];
            float markerHeight = markerWidthHeight[1];
            String xAttribute = this.GetAttribute(SvgConstants.Attributes.X);
            String yAttribute = this.GetAttribute(SvgConstants.Attributes.Y);
            float x = xAttribute == null ? 0f : CssDimensionParsingUtils.ParseAbsoluteLength(xAttribute);
            float y = yAttribute == null ? 0f : CssDimensionParsingUtils.ParseAbsoluteLength(yAttribute);
            Rectangle markerViewport = new Rectangle(x, y, markerWidth, markerHeight);
            context.AddViewPort(markerViewport);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal virtual void ApplyMarkerAttributes(SvgDrawContext context) {
            ApplyRotation(context);
            ApplyUserSpaceScaling(context);
            ApplyCoordinatesTranslation(context);
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void DrawMarker(SvgDrawContext context, String moveX, String moveY, MarkerVertexType markerToUse
            , AbstractSvgNodeRenderer parent) {
            String elementToReUse = parent.attributesAndStyles.Get(markerToUse.ToString());
            String normalizedName = SvgTextUtil.FilterReferenceValue(elementToReUse);
            ISvgNodeRenderer template = context.GetNamedObject(normalizedName);
            //Clone template
            ISvgNodeRenderer namedObject = template == null ? null : template.CreateDeepCopy();
            if (namedObject is MarkerSvgNodeRenderer && 
                        // Having markerWidth or markerHeight with negative or zero value disables rendering of the element .
                        MarkerWidthHeightAreCorrect((MarkerSvgNodeRenderer)namedObject)) {
                // setting the parent of the referenced element to this instance
                namedObject.SetParent(parent);
                namedObject.SetAttribute(SvgConstants.Tags.MARKER, markerToUse.ToString());
                namedObject.SetAttribute(SvgConstants.Attributes.X, moveX);
                namedObject.SetAttribute(SvgConstants.Attributes.Y, moveY);
                context.GetCurrentCanvas().SaveState();
                namedObject.Draw(context);
                context.GetCurrentCanvas().RestoreState();
                // unsetting the parent of the referenced element
                namedObject.SetParent(null);
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal static void DrawMarkers(SvgDrawContext context, int startIndex, IList<Point> markerPoints, MarkerVertexType
             markerToUse, AbstractSvgNodeRenderer parent) {
            String elementToReUse = SvgTextUtil.FilterReferenceValue(parent.attributesAndStyles.Get(markerToUse.ToString
                ()));
            ISvgNodeRenderer template = context.GetNamedObject(elementToReUse);
            if (!(template is MarkerSvgNodeRenderer && 
                        // Having markerWidth or markerHeight with negative or zero value disables rendering of the element .
                        MarkerWidthHeightAreCorrect((MarkerSvgNodeRenderer)template))) {
                return;
            }
            for (int i = 0; i < markerPoints.Count; ++i) {
                ISvgNodeRenderer marker = template.CreateDeepCopy();
                // setting the parent of the referenced element to this instance
                marker.SetParent(parent);
                marker.SetAttribute(SvgConstants.Tags.MARKER, markerToUse.ToString());
                marker.SetAttribute(SvgConstants.Attributes.X, Convert.ToString(markerPoints[i].GetX(), System.Globalization.CultureInfo.InvariantCulture
                    ));
                marker.SetAttribute(SvgConstants.Attributes.Y, Convert.ToString(markerPoints[i].GetY(), System.Globalization.CultureInfo.InvariantCulture
                    ));
                marker.SetAttribute(MarkerSvgNodeRenderer.MARKER_INDEX, JavaUtil.IntegerToString(startIndex + i));
                context.GetCurrentCanvas().SaveState();
                marker.Draw(context);
                context.GetCurrentCanvas().RestoreState();
            }
        }
//\endcond

//\cond DO_NOT_DOCUMENT
        internal override void ApplyViewBox(SvgDrawContext context) {
            if (this.attributesAndStyles != null) {
                float[] markerWidthHeight = GetMarkerWidthHeightValues();
                float markerWidth = markerWidthHeight[0];
                float markerHeight = markerWidthHeight[1];
                float[] values = GetViewBoxValues(markerWidth, markerHeight);
                Rectangle currentViewPort = context.GetCurrentViewPort();
                base.CalculateAndApplyViewBox(context, values, currentViewPort);
            }
        }
//\endcond

        private float[] GetMarkerWidthHeightValues() {
            float markerWidth = DEFAULT_MARKER_WIDTH;
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.MARKER_WIDTH)) {
                String markerWidthRawValue = attributesAndStyles.Get(SvgConstants.Attributes.MARKER_WIDTH);
                markerWidth = CssDimensionParsingUtils.ParseAbsoluteLength(markerWidthRawValue);
            }
            else {
                if (this.attributesAndStyles.ContainsKey(StringNormalizer.ToLowerCase(SvgConstants.Attributes.MARKER_WIDTH
                    ))) {
                    // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
                    String markerWidthRawValue = attributesAndStyles.Get(StringNormalizer.ToLowerCase(SvgConstants.Attributes.
                        MARKER_WIDTH));
                    markerWidth = CssDimensionParsingUtils.ParseAbsoluteLength(markerWidthRawValue);
                }
            }
            float markerHeight = DEFAULT_MARKER_HEIGHT;
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.MARKER_HEIGHT)) {
                String markerHeightRawValue = attributesAndStyles.Get(SvgConstants.Attributes.MARKER_HEIGHT);
                markerHeight = CssDimensionParsingUtils.ParseAbsoluteLength(markerHeightRawValue);
            }
            else {
                if (this.attributesAndStyles.ContainsKey(StringNormalizer.ToLowerCase(SvgConstants.Attributes.MARKER_HEIGHT
                    ))) {
                    // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
                    String markerHeightRawValue = attributesAndStyles.Get(StringNormalizer.ToLowerCase(SvgConstants.Attributes
                        .MARKER_HEIGHT));
                    markerHeight = CssDimensionParsingUtils.ParseAbsoluteLength(markerHeightRawValue);
                }
            }
            return new float[] { markerWidth, markerHeight };
        }

        private static bool MarkerWidthHeightAreCorrect(MarkerSvgNodeRenderer namedObject) {
            ILogger log = ITextLogManager.GetLogger(typeof(MarkerSvgNodeRenderer));
            String markerWidth = namedObject.GetAttribute(SvgConstants.Attributes.MARKER_WIDTH);
            // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
            if (markerWidth == null) {
                markerWidth = namedObject.GetAttribute(StringNormalizer.ToLowerCase(SvgConstants.Attributes.MARKER_WIDTH));
            }
            String markerHeight = namedObject.GetAttribute(SvgConstants.Attributes.MARKER_HEIGHT);
            // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
            if (markerHeight == null) {
                markerHeight = namedObject.GetAttribute(StringNormalizer.ToLowerCase(SvgConstants.Attributes.MARKER_HEIGHT
                    ));
            }
            bool isCorrect = true;
            if (markerWidth != null) {
                float absoluteMarkerWidthValue = CssDimensionParsingUtils.ParseAbsoluteLength(markerWidth);
                if (absoluteMarkerWidthValue == 0) {
                    log.LogWarning(SvgLogMessageConstant.MARKER_WIDTH_IS_ZERO_VALUE);
                    isCorrect = false;
                }
                else {
                    if (absoluteMarkerWidthValue < 0) {
                        log.LogWarning(SvgLogMessageConstant.MARKER_WIDTH_IS_NEGATIVE_VALUE);
                        isCorrect = false;
                    }
                }
            }
            if (markerHeight != null) {
                float absoluteMarkerHeightValue = CssDimensionParsingUtils.ParseAbsoluteLength(markerHeight);
                if (absoluteMarkerHeightValue == 0) {
                    log.LogWarning(SvgLogMessageConstant.MARKER_HEIGHT_IS_ZERO_VALUE);
                    isCorrect = false;
                }
                else {
                    if (absoluteMarkerHeightValue < 0) {
                        log.LogWarning(SvgLogMessageConstant.MARKER_HEIGHT_IS_NEGATIVE_VALUE);
                        isCorrect = false;
                    }
                }
            }
            return isCorrect;
        }

        private void ApplyRotation(SvgDrawContext context) {
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.ORIENT)) {
                String orient = this.attributesAndStyles.Get(SvgConstants.Attributes.ORIENT);
                double rotAngle = double.NaN;
                // If placed by marker-start, the marker is oriented 180° different from
                // the orientation that would be used if auto was specified.
                // For all other markers, auto-start-reverse means the same as auto.
                if (SvgConstants.Values.AUTO.Equals(orient) || (SvgConstants.Values.AUTO_START_REVERSE.Equals(orient) && !
                    SvgConstants.Attributes.MARKER_START.Equals(this.attributesAndStyles.Get(SvgConstants.Tags.MARKER)))) {
                    rotAngle = ((IMarkerCapable)GetParent()).GetAutoOrientAngle(this, false);
                }
                else {
                    if (SvgConstants.Values.AUTO_START_REVERSE.Equals(orient) && SvgConstants.Attributes.MARKER_START.Equals(this
                        .attributesAndStyles.Get(SvgConstants.Tags.MARKER))) {
                        rotAngle = ((IMarkerCapable)GetParent()).GetAutoOrientAngle(this, true);
                    }
                    else {
                        if (CssTypesValidationUtils.IsAngleValue(orient) || CssTypesValidationUtils.IsNumber(orient)) {
                            rotAngle = CssDimensionParsingUtils.ParseAngle(this.attributesAndStyles.Get(SvgConstants.Attributes.ORIENT
                                ));
                        }
                    }
                }
                if (!double.IsNaN(rotAngle)) {
                    context.GetCurrentCanvas().ConcatMatrix(AffineTransform.GetRotateInstance(rotAngle));
                }
            }
        }

        private void ApplyUserSpaceScaling(SvgDrawContext context) {
            bool markerUnitsEqualsStrokeWidth = !this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.MARKER_UNITS
                ) || SvgConstants.Values.STROKEWIDTH.Equals(this.attributesAndStyles.Get(SvgConstants.Attributes.MARKER_UNITS
                ));
            // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
            bool markerUnitsLowerEqualsStrokeWidth = !this.attributesAndStyles.ContainsKey(StringNormalizer.ToLowerCase
                (SvgConstants.Attributes.MARKER_UNITS)) || SvgConstants.Values.STROKEWIDTH.Equals(this.attributesAndStyles
                .Get(StringNormalizer.ToLowerCase(SvgConstants.Attributes.MARKER_UNITS)));
            if (markerUnitsEqualsStrokeWidth && markerUnitsLowerEqualsStrokeWidth) {
                String parentValue = this.GetParent().GetAttribute(SvgConstants.Attributes.STROKE_WIDTH);
                if (parentValue != null) {
                    // If stroke width is a percentage value is always computed as a percentage of the normalized viewBox diagonal length.
                    double rootViewPortHeight = context.GetRootViewPort().GetHeight();
                    double rootViewPortWidth = context.GetRootViewPort().GetWidth();
                    double viewBoxDiagonalLength = CssUtils.ConvertPxToPts(Math.Sqrt(rootViewPortHeight * rootViewPortHeight +
                         rootViewPortWidth * rootViewPortWidth));
                    float strokeWidthScale = CssUtils.ConvertPtsToPx(SvgCssUtils.ParseAbsoluteLength(this, parentValue, (float
                        )viewBoxDiagonalLength, 1f, context));
                    context.GetCurrentCanvas().ConcatMatrix(AffineTransform.GetScaleInstance(strokeWidthScale, strokeWidthScale
                        ));
                }
            }
        }

        private void ApplyCoordinatesTranslation(SvgDrawContext context) {
            float xScale = 1;
            float yScale = 1;
            float[] viewBox = SvgCssUtils.ParseViewBox(this);
            if (viewBox != null && viewBox.Length == SvgConstants.Values.VIEWBOX_VALUES_NUMBER) {
                xScale = context.GetCurrentViewPort().GetWidth() / viewBox[2];
                yScale = context.GetCurrentViewPort().GetHeight() / viewBox[3];
            }
            float moveX = DEFAULT_REF_X;
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.REFX)) {
                String refX = this.attributesAndStyles.Get(SvgConstants.Attributes.REFX);
                moveX = SvgCssUtils.ParseAbsoluteLength(this, refX, context.GetRootViewPort().GetWidth(), moveX, context);
                //Apply scale
                moveX *= -1 * xScale;
            }
            else {
                if (this.attributesAndStyles.ContainsKey(StringNormalizer.ToLowerCase(SvgConstants.Attributes.REFX))) {
                    // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
                    String refX = this.attributesAndStyles.Get(StringNormalizer.ToLowerCase(SvgConstants.Attributes.REFX));
                    moveX = SvgCssUtils.ParseAbsoluteLength(this, refX, context.GetRootViewPort().GetWidth(), moveX, context);
                    //Apply scale
                    moveX *= -1 * xScale;
                }
            }
            float moveY = DEFAULT_REF_Y;
            if (this.attributesAndStyles.ContainsKey(SvgConstants.Attributes.REFY)) {
                String refY = this.attributesAndStyles.Get(SvgConstants.Attributes.REFY);
                moveY = SvgCssUtils.ParseAbsoluteLength(this, refY, context.GetRootViewPort().GetHeight(), moveY, context);
                moveY *= -1 * yScale;
            }
            else {
                if (this.attributesAndStyles.ContainsKey(StringNormalizer.ToLowerCase(SvgConstants.Attributes.REFY))) {
                    // TODO: DEVSIX-3923 remove normalization (.toLowerCase)
                    String refY = this.attributesAndStyles.Get(StringNormalizer.ToLowerCase(SvgConstants.Attributes.REFY));
                    moveY = SvgCssUtils.ParseAbsoluteLength(this, refY, context.GetRootViewPort().GetHeight(), moveY, context);
                    moveY *= -1 * yScale;
                }
            }
            AffineTransform translation = AffineTransform.GetTranslateInstance(moveX, moveY);
            if (!translation.IsIdentity()) {
                context.GetCurrentCanvas().ConcatMatrix(translation);
            }
        }

        private float[] GetViewBoxValues(float defaultWidth, float defaultHeight) {
            float[] values = SvgCssUtils.ParseViewBox(this);
            if (values == null || values.Length < SvgConstants.Values.VIEWBOX_VALUES_NUMBER) {
                //If viewBox is not specified or incorrect, it's width and height are the same as passed defaults
                return new float[] { 0, 0, defaultWidth, defaultHeight };
            }
            else {
                return values;
            }
        }
    }
}
