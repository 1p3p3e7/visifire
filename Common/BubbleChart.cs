﻿/*   
    Copyright (C) 2008 Webyog Softworks Private Limited

    This file is a part of Visifire Charts.
 
    Visifire is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Visifire Charts.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL is not suitable for your products or company, Webyog provides Visifire 
    under a flexible commercial license designed to meet your specific usage and 
    distribution requirements. If you have already obtained a commercial license 
    from Webyog, you can use this file under those license terms.
    
*/

#if WPF
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
#else
using System;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Collections.Generic;
#endif

using Visifire.Commons;

namespace Visifire.Charts
{
    /// <summary>
    /// Visifire.Charts.BubbleChart class
    /// </summary>
    internal class BubbleChart
    {
        #region Public Methods

        #endregion

        #region Public Properties

        #endregion

        #region Public Events And Delegates

        #endregion

        #region Protected Methods

        #endregion

        #region Internal Properties

        #endregion

        #region Private Properties
               
        #endregion

        #region Private Delegates

        #endregion

        #region Private Methods

        /// <summary>
        /// Apply animation for bubble chart
        /// </summary>
        /// <param name="pointGrid">Bubble chart grid</param>
        /// <param name="storyboard">Stroyboard</param>
        /// <param name="width">Width of the chart canvas</param>
        /// <param name="height">Height of the chart canvas</param>
        /// <returns>Storyboard</returns>
        private static Storyboard ApplyBubbleChartAnimation(DataSeries currentDataSeries, Panel bubbleGrid, Storyboard storyboard, Double width, Double height)
        {   
            TranslateTransform translateTransform = new TranslateTransform() { X = 0, Y = -height };
            bubbleGrid.RenderTransform = translateTransform;

            Random rand = new Random((Int32)DateTime.Now.Ticks);
            Double begin = rand.NextDouble();

            Double hPitchSize = width / 5;

            DoubleCollection times1 = Graphics.GenerateDoubleCollection(0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0);
            DoubleCollection times2 = Graphics.GenerateDoubleCollection(0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0);
            DoubleCollection translateXValues;
            DoubleCollection pitchFactors = Graphics.GenerateDoubleCollection(1, 15.0 / 23.0, 11.0 / 23.0, 7.0 / 23.0, 5.0 / 23.0, 3.0 / 23.0, 2.0 / 23.0, 1.0 / 23.0, 0.5 / 23.0, 0);
            translateXValues = new DoubleCollection();
            Double sign = 1;

            if (rand.NextDouble() > 0.5)
            {
                if ((Double)bubbleGrid.GetValue(Canvas.LeftProperty) > width / 2) sign = -1;
                else sign = 1;
            }
            else
            {
                if ((Double)bubbleGrid.GetValue(Canvas.LeftProperty) > width / 2) sign = 1;
                else sign = -1;
            }

            // Generate pitch values
            foreach (Double factor in pitchFactors)
                translateXValues.Add(sign * hPitchSize * factor);

            DoubleCollection translateYValues = Graphics.GenerateDoubleCollection(-height, 0, -height * 0.5, 0, -height * 0.25, 0, -height * 0.125, 0, -height * 0.0625, 0);
            List<KeySpline> splines3 = AnimationHelper.GenerateKeySplineList(
                new Point(0, 0), new Point(1, 1),
                new Point(0, 0), new Point(1, 1),
                new Point(0, 0), new Point(1, 1),
                new Point(0, 0), new Point(1, 1),
                new Point(0, 0), new Point(1, 1),
                new Point(0, 0), new Point(1, 1),
                new Point(0, 0), new Point(1, 1),
                new Point(0, 0), new Point(1, 1),
                new Point(0, 0), new Point(1, 1),
                new Point(0, 0), new Point(1, 1));

            List<KeySpline> splines4 = AnimationHelper.GenerateKeySplineList(
                new Point(0, 0), new Point(1, 1),
                new Point(0.5, 0), new Point(1, 1),
                new Point(0, 0), new Point(0.5, 1),
                new Point(0.5, 0), new Point(1, 1),
                new Point(0, 0), new Point(0.5, 1),
                new Point(0.5, 0), new Point(1, 1),
                new Point(0, 0), new Point(0.5, 1),
                new Point(0.5, 0), new Point(1, 1),
                new Point(0, 0), new Point(0.5, 1),
                new Point(0.5, 0), new Point(1, 1));

            DoubleAnimationUsingKeyFrames xTranslateAnimation = AnimationHelper.CreateDoubleAnimation(currentDataSeries, translateTransform, "(TranslateTransform.X)", begin * 0.5 + 0.5, times1, translateXValues, splines3);
            DoubleAnimationUsingKeyFrames yTranslateAnimation = AnimationHelper.CreateDoubleAnimation(currentDataSeries, translateTransform, "(TranslateTransform.Y)", begin * 0.5 + 0.5, times2, translateYValues, splines4);
            storyboard.Children.Add(xTranslateAnimation);
            storyboard.Children.Add(yTranslateAnimation);

            return storyboard;
        }
        
        #endregion

        #region Internal Methods

        private static void CalculateMaxAndMinZValueFromAllSeries(ref List<DataSeries> seriesList, out Double minimumZVal, out Double maximumZVal)
        {
            List<Double> minValues = new List<double>();
            List<Double> maxValues = new List<double>();
            
            minimumZVal = 0;
            maximumZVal = 1;

            foreach (DataSeries series in seriesList)
            {
                if (series.Enabled == false)
                    continue;
                
                CalculateMaxAndMinZValue(series, out minimumZVal, out maximumZVal);
                minValues.Add(minimumZVal);
                maxValues.Add(maximumZVal);
            }

            if (minValues.Count > 0)
                minimumZVal = minValues.Min();

            if (maxValues.Count > 0)
                maximumZVal = maxValues.Max();
        }

        /// <summary>
        /// Get visual object for bubble chart
        /// </summary>
        /// <param name="width">Width of the chart</param>
        /// <param name="height">Height of the chart</param>
        /// <param name="plotDetails">plotDetails</param>
        /// <param name="seriesList">List of DataSeries</param>
        /// <param name="chart">Chart</param>
        /// <param name="plankDepth">Plank depth</param>
        /// <param name="animationEnabled">Whether animation is enabled</param>
        /// <returns>Bubble chart canvas</returns>
        internal static Canvas GetVisualObjectForBubbleChart(Panel preExistingPanel, Double width, Double height, PlotDetails plotDetails, List<DataSeries> seriesList, Chart chart, Double plankDepth, bool animationEnabled)
        {   
            if (Double.IsNaN(width) || Double.IsNaN(height) || width <= 0 || height <= 0) return null;

            // Visual for all bubble charts, Nothing but presisting visual for buble chart
            Canvas visual;

            // Holds all bubbles of all series 
            Canvas bubbleChartCanvas;

            RenderHelper.RepareCanvas4Drawing(preExistingPanel as Canvas, out visual, out bubbleChartCanvas, width, height);
      
            Double depth3d = plankDepth / (plotDetails.Layer3DCount == 0 ? 1 : plotDetails.Layer3DCount) * (chart.View3D ? 1 : 0);
            Double visualOffset = depth3d * (plotDetails.SeriesDrawingIndex[seriesList[0]] + 1 - (plotDetails.Layer3DCount == 0 ? 0 : 1));
            
            visual.SetValue(Canvas.TopProperty, visualOffset);
            visual.SetValue(Canvas.LeftProperty, -visualOffset);

            Double minimumZVal, maximumZVal;
            CalculateMaxAndMinZValueFromAllSeries(ref seriesList, out minimumZVal, out maximumZVal);

            foreach (DataSeries series in seriesList)
            {
                Faces dsFaces = new Faces() { Visual = bubbleChartCanvas };
                series.Faces = dsFaces;

                if (series.Enabled == false)
                    continue;
                                
                //out Double minimumZVal, out Double maximumZVal
                PlotGroup plotGroup = series.PlotGroup;

                List<DataPoint> dataPointsInViewPort = RenderHelper.GetDataPointsUnderViewPort(series, false);

                foreach (DataPoint dataPoint in dataPointsInViewPort)
                {   
                    CreateOrUpdateAPointDataPoint(bubbleChartCanvas, dataPoint, minimumZVal, maximumZVal, width, height);

                    // Apply animation
                    if (animationEnabled && dataPoint.Marker != null)
                    {   
                        if (dataPoint.Parent.Storyboard == null)
                            dataPoint.Parent.Storyboard = new Storyboard();

                        // Apply animation to the bubbles
                        dataPoint.Parent.Storyboard = ApplyBubbleChartAnimation(dataPoint.Parent, dataPoint.Marker.Visual, dataPoint.Parent.Storyboard, width, height);
                    }
                }
            }

            RectangleGeometry clipRectangle = new RectangleGeometry();
            clipRectangle.Rect = new Rect(0, -chart.ChartArea.PLANK_DEPTH, width + chart.ChartArea.PLANK_OFFSET, height + chart.ChartArea.PLANK_DEPTH);
            visual.Clip = clipRectangle;

            return visual;
        }
        
        private static void CreateOrUpdateAPointDataPoint(Canvas bubleChartCanvas, DataPoint dataPoint, Double minimumZVal, Double maximumZVal, Double plotWidth, Double plotHeight)
        {
            Faces dpFaces = dataPoint.Faces;

            // Remove preexisting dataPoint visual and label visual
            if (dpFaces != null && dpFaces.Visual != null && bubleChartCanvas == dpFaces.Visual.Parent)
            {   
                bubleChartCanvas.Children.Remove(dataPoint.Faces.Visual);
                // dpFaces = null;
            }

            dataPoint.Faces = null;

            if (Double.IsNaN(dataPoint.InternalYValue) || (dataPoint.Enabled == false))
                return;

            Chart chart = dataPoint.Chart as Chart;

            PlotGroup plotGroup = dataPoint.Parent.PlotGroup;

            Canvas dataPointVisual = new Canvas();

            Faces bubbleFaces = new Faces();

            Double xPosition = Graphics.ValueToPixelPosition(0, plotWidth, (Double)plotGroup.AxisX.InternalAxisMinimum, (Double)plotGroup.AxisX.InternalAxisMaximum, dataPoint.InternalXValue);
            Double yPosition = Graphics.ValueToPixelPosition(plotHeight, 0, (Double)plotGroup.AxisY.InternalAxisMinimum, (Double)plotGroup.AxisY.InternalAxisMaximum, dataPoint.InternalYValue);

            Brush markerColor = dataPoint.Color;
            markerColor = (chart.View3D ? Graphics.Get3DBrushLighting(markerColor, (Boolean)dataPoint.LightingEnabled) : ((Boolean)dataPoint.LightingEnabled ? Graphics.GetLightingEnabledBrush(markerColor, "Linear", null) : markerColor));
            
            String labelText = (Boolean)dataPoint.LabelEnabled ? dataPoint.TextParser(dataPoint.LabelText) : "";

            Marker marker = new Marker((MarkerTypes)dataPoint.MarkerType, 1, new Size(6,6), false, markerColor, labelText);
            dataPoint.Marker = marker;

            ApplyZValue(dataPoint, minimumZVal, maximumZVal, plotWidth, plotHeight);
                       
            marker.ShadowEnabled = (Boolean)dataPoint.ShadowEnabled;

            if (dataPoint.BorderColor != null)
                marker.BorderColor = dataPoint.BorderColor;

            marker.TextBackground = dataPoint.LabelBackground;
            marker.BorderThickness = ((Thickness)dataPoint.MarkerBorderThickness).Left;
            marker.TextAlignmentX = AlignmentX.Center;
            marker.TextAlignmentY = AlignmentY.Center;
            marker.Tag = new ElementData() { Element = dataPoint };

            Double gap = ((Double)dataPoint.MarkerScale * (Double)dataPoint.MarkerSize) / 2;

            if (!String.IsNullOrEmpty(labelText))
            {
                marker.FontColor = Chart.CalculateDataPointLabelFontColor(chart, dataPoint, dataPoint.LabelFontColor, LabelStyles.OutSide);
                marker.FontSize = (Double)dataPoint.LabelFontSize;
                marker.FontWeight = (FontWeight)dataPoint.LabelFontWeight;
                marker.FontFamily = dataPoint.LabelFontFamily;
                marker.FontStyle = (FontStyle)dataPoint.LabelFontStyle;
                marker.TextBackground = dataPoint.LabelBackground;

                marker.TextAlignmentX = AlignmentX.Center;
                marker.TextAlignmentY = AlignmentY.Center;

                if (!Double.IsNaN(dataPoint.LabelAngle) && dataPoint.LabelAngle != 0)
                {
                    marker.LabelAngle = dataPoint.LabelAngle;
                    marker.TextOrientation = Orientation.Vertical;

                    marker.TextAlignmentX = AlignmentX.Center;
                    marker.TextAlignmentY = AlignmentY.Center;

                    marker.LabelStyle = (LabelStyles)dataPoint.LabelStyle;
                }

                marker.CreateVisual();

                if (Double.IsNaN(dataPoint.LabelAngle) || dataPoint.LabelAngle == 0)
                {   
                    if (yPosition - gap < 0 && (yPosition - marker.TextBlockSize.Height / 2) < 0)
                        marker.TextAlignmentY = AlignmentY.Bottom;
                    else if (yPosition + gap > plotHeight && (yPosition + marker.TextBlockSize.Height / 2) > plotHeight)
                        marker.TextAlignmentY = AlignmentY.Top;

                    if (xPosition - gap < 0 && (xPosition - marker.TextBlockSize.Width / 2) < 0)
                        marker.TextAlignmentX = AlignmentX.Right;
                    else if (xPosition + gap > plotWidth && (xPosition + marker.TextBlockSize.Width / 2) > plotWidth)
                        marker.TextAlignmentX = AlignmentX.Left;
                }
            }

            if (!VisifireControl.IsMediaEffectsEnabled)
                marker.PixelLavelShadow = false; // pixelLavelShadow;
            else
                marker.PixelLavelShadow = true;

            marker.CreateVisual();

            UpdateBubblePositionAccording2XandYValue(dataPoint, plotWidth, plotHeight, false, 0, 0);
            bubleChartCanvas.Children.Add(marker.Visual);
            
            bubbleFaces.Parts.Add(marker.MarkerShape);
            bubbleFaces.VisualComponents.Add(marker.Visual);
            bubbleFaces.BorderElements.Add(marker.MarkerShape);

            bubbleFaces.Visual = marker.Visual;
            dataPoint.Faces = bubbleFaces;

            dataPoint.Faces.Visual.Opacity = (Double)dataPoint.Opacity * (Double)dataPoint.Parent.Opacity;
            dataPoint.AttachEvent2DataPointVisualFaces(dataPoint);
            dataPoint.AttachEvent2DataPointVisualFaces(dataPoint.Parent);
            dataPoint._parsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
            if(!chart.IndicatorEnabled)
                dataPoint.AttachToolTip(chart, dataPoint, dataPoint.Faces.VisualComponents);
            dataPoint.AttachHref(chart, dataPoint.Faces.VisualComponents, dataPoint.Href, (HrefTargets)dataPoint.HrefTarget);
            dataPoint.SetCursor2DataPointVisualFaces();

            if (dataPoint.Parent.SelectionEnabled && dataPoint.Selected)
                dataPoint.Select(true);
        }

        internal static void UpdateBubblePositionAccording2XandYValue(DataPoint dataPoint, Double drawingAreaWidth, Double drawingAreaHeight, Boolean animatedUpdate, Double oldSize, Double newSize)
        {
            dataPoint._parsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);

            //animatedUpdate = false;
            Marker marker = dataPoint.Marker;
            PlotGroup plotGroup = dataPoint.Parent.PlotGroup;

            Double xPosition = Graphics.ValueToPixelPosition(0, drawingAreaWidth, (Double)plotGroup.AxisX.InternalAxisMinimum, (Double)plotGroup.AxisX.InternalAxisMaximum, dataPoint.InternalXValue);
            Double yPosition = Graphics.ValueToPixelPosition(drawingAreaHeight, 0, (Double)plotGroup.AxisY.InternalAxisMinimum, (Double)plotGroup.AxisY.InternalAxisMaximum, dataPoint.InternalYValue);

            if (animatedUpdate)
            {
                Point newPosition = marker.CalculateActualPosition(xPosition, yPosition, new Point(0.5, 0.5));
                ApplyAnimation4XYZUpdate(dataPoint, newPosition, oldSize, newSize);
                
                //dataPoint.Storyboard.SpeedRatio = 2;
                //dataPoint.Storyboard.Begin();
            }
            else
                marker.SetPosition(xPosition, yPosition, new Point(0.5, 0.5));

            if (dataPoint.Parent.ToolTipElement != null)
                dataPoint.Parent.ToolTipElement.Hide();

            (dataPoint.Chart as Chart).ChartArea.DisableIndicators();

            dataPoint._visualPosition = new Point(xPosition, yPosition);
        }

        private static void ApplyAnimation4XYZUpdate(DataPoint dataPoint, Point newPosition, Double oldSize, Double newSize)
        {
            Marker marker = dataPoint.Marker;
            FrameworkElement markerVisual = marker.Visual;
            Double oldMarkerLeft, oldMarkerTop;

            if (dataPoint.Storyboard != null)
            {
                // ClockState cs = dataPoint.Storyboard.GetCurrentState();
                // if (cs == ClockState.Active)
                //      dataPoint.Storyboard.SkipToFill();
                // dataPoint.Storyboard.Stop();
                dataPoint.Storyboard.Pause();
                // ClockState cs = dataPoint.Storyboard.GetCurrentState();
                
                oldMarkerLeft = (Double)marker.Visual.GetValue(Canvas.LeftProperty);
                oldMarkerTop = (Double)marker.Visual.GetValue(Canvas.TopProperty);
                dataPoint.Storyboard.Resume();


                dataPoint.Storyboard = null;
                // dataPoint.Storyboard.Children.Clear();
            }
            else
            {   
                oldMarkerLeft = (Double)marker.Visual.GetValue(Canvas.LeftProperty);
                oldMarkerTop = (Double)marker.Visual.GetValue(Canvas.TopProperty);
            }
            
            Point oldPosition = new Point(oldMarkerLeft, oldMarkerTop);

            Storyboard storyboard = new Storyboard();

            //if (dataPoint.Storyboard != null)
            //    storyboard = dataPoint.Storyboard;
            //else
                //storyboard = new Storyboard();

            if (oldPosition.X != newPosition.X)
            {
                storyboard = AnimationHelper.ApplyPropertyAnimation(markerVisual, "(Canvas.Left)", dataPoint, storyboard, 0,
                                new Double[] { 0, 1 }, new Double[] { oldPosition.X, newPosition.X },
                                null);
            }

            if (oldPosition.Y != newPosition.Y)
            {
                storyboard = AnimationHelper.ApplyPropertyAnimation(markerVisual, "(Canvas.Top)", dataPoint, storyboard, 0,
                               new Double[] { 0, 1 }, new Double[] { oldPosition.Y, newPosition.Y },
                               null);
            }

            if (dataPoint.Parent.RenderAs == RenderAs.Bubble && oldSize != newSize)
            {   
                storyboard = AnimationHelper.ApplyPropertyAnimation(marker.MarkerShape, "Height", dataPoint, storyboard, 0,
                new Double[] { 0, 1 }, new Double[] { oldSize, newSize },
                null);
                storyboard = AnimationHelper.ApplyPropertyAnimation(marker.MarkerShape, "Width", dataPoint, storyboard, 0,
                new Double[] { 0, 1 }, new Double[] { oldSize, newSize },
                null);

                if (marker.MarkerShadow != null)
                {   
                    storyboard = AnimationHelper.ApplyPropertyAnimation(marker.MarkerShadow, "Height", dataPoint, storyboard, 0,
                    new Double[] { 0, 1 }, new Double[] { oldSize, newSize },
                    null);
                    storyboard = AnimationHelper.ApplyPropertyAnimation(marker.MarkerShadow, "Width", dataPoint, storyboard, 0,
                    new Double[] { 0, 1 }, new Double[] { oldSize, newSize },
                    null);
                }
            }

            Random rand = new Random(DateTime.Now.Millisecond);
            storyboard.SpeedRatio = 2 - rand.NextDouble();
            dataPoint.Storyboard = storyboard;
            
#if WPF
            storyboard.Begin(dataPoint.Chart._rootElement, true);
#else
            storyboard.Begin();
#endif
        }

        private static void CalculateMaxAndMinZValue(DataSeries series, out Double minimumZVal, out Double maximumZVal)
        {   
            var dataPointsList = (from dp in series.InternalDataPoints where !Double.IsNaN(dp.ZValue) && dp.Enabled == true select dp.ZValue);

            minimumZVal = 0;
            maximumZVal = 1;

            if (dataPointsList.Count() > 0)
            {
                minimumZVal = dataPointsList.Min();
                maximumZVal = dataPointsList.Max();
            }
        }

        /// <summary>
        /// Calculate ControlPoints of a DataSeries
        /// </summary>
        /// <param name="chart">Chart</param>
        /// <param name="plotGroup">Corresponding plotGroup</param>
        /// <param name="dataPoints">List of DataPoints</param>
        /// <returns>ControlPoints</returns>
        internal static Point[] CalculateControlPointsOfADataSeries(Chart chart, PlotGroup plotGroup, List<DataPoint> dataPoints)
        {   
            List<Point> tempCPoints = new List<Point>();

            if (chart != null && chart.InternalSeries != null && plotGroup != null && dataPoints.Count > 0)
            {   
                Double minimumZVal, maximumZVal;

                List<DataSeries> seriesList = chart.InternalSeries;
                CalculateMaxAndMinZValueFromAllSeries(ref seriesList, out minimumZVal, out maximumZVal);

                Double tempAxisMinimum = plotGroup._initialAxisXMin;
                Double tempAxisMaximum = plotGroup._initialAxisXMax;

                dataPoints = dataPoints.OrderBy(a => a.ZValue).ToList();
                DataPoint dataPointHavingMaxZVal = dataPoints.Last();
                Double maxBubbleRadius = CalculateBubbleSize(dataPointHavingMaxZVal, minimumZVal, maximumZVal);

                foreach (DataPoint dataPoint in dataPoints)
                {   
                    // Double maxBubbleRadius = CalculateBubbleSize(dataPoint, minimumZVal, maximumZVal);
                    List<Point> controlPointsOfDp = CalculateControlPointsOfDataPoint(dataPoint, plotGroup, maxBubbleRadius, minimumZVal, maximumZVal, plotGroup.AxisX, plotGroup.AxisY);
                    tempCPoints.InsertRange(0, controlPointsOfDp);
                }

                if (tempCPoints.Count > 0)
                {   
                    tempCPoints = tempCPoints.OrderBy(x => x.X).ToList();
                }

                // tempCPoints.Clear();
            }

            return tempCPoints.ToArray();
        }

        /// <summary>
        /// Calculate ControlPoints of a DataPoint
        /// </summary>
        /// <param name="dataPoint">DataPoint</param>
        /// <param name="minimumZVal">Minimum ZValue</param>
        /// <param name="maximumZVal">Maximum ZValue</param>
        /// <param name="axisX">AxisX</param>
        /// <param name="axisY">AxisY</param>
        /// <returns></returns>
        private static List<Point> CalculateControlPointsOfDataPoint(DataPoint dataPoint, PlotGroup plotGroup, Double maxBubbleRadius,  Double minimumZVal, Double maximumZVal, Axis axisX, Axis axisY)
        {   
            Chart chart = dataPoint.Chart as Chart;
            List<Point> ctPoints = new List<Point>();

            if (chart != null && axisX !=null && axisY != null)
            {
                Double leftRightOffset = 4; // One Pixel
                Double topBottomOffset = 4; // One Pixel

                Size initialPlotAreaSize = new Size(plotGroup._intialAxisXWidth, plotGroup._intialAxisYHeight);
                
                Double xPosition = Graphics.ValueToPixelPosition(0, initialPlotAreaSize.Width, (Double)plotGroup._initialAxisXMin, (Double)plotGroup._initialAxisXMax, dataPoint.InternalXValue);
                Double yPosition = Graphics.ValueToPixelPosition(initialPlotAreaSize.Height, 0, (Double)plotGroup._initialAxisYMin, (Double)plotGroup._initialAxisYMax, dataPoint.InternalYValue);

                if (axisY.Logarithmic)
                {   
                    Double yValueInLog = Graphics.PixelPositionToValue(initialPlotAreaSize.Height, 0, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, yPosition);
                    Double actualYValue = DataPoint.ConvertLogarithmicValue2ActualValue(chart, yValueInLog, axisY.AxisType);

                    Point pointLeft = new Point(
                    Graphics.PixelPositionToValue(0, initialPlotAreaSize.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, xPosition - (maxBubbleRadius + leftRightOffset))
                    , actualYValue);

                    // Calculate right point of bubble in terms of XValue and YValue 
                    Point pointRight = new Point(
                        Graphics.PixelPositionToValue(0, initialPlotAreaSize.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, xPosition + (maxBubbleRadius + leftRightOffset))
                        , actualYValue);

                    yValueInLog = Graphics.PixelPositionToValue(initialPlotAreaSize.Height, 0, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, yPosition - (maxBubbleRadius + topBottomOffset));
                    actualYValue = DataPoint.ConvertLogarithmicValue2ActualValue(chart, yValueInLog, axisY.AxisType);

                    // Calculate top point of bubble in terms of XValue and YValue 
                    Point pointTop = new Point(
                        Graphics.PixelPositionToValue(0, initialPlotAreaSize.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, xPosition)
                        , actualYValue);

                    yValueInLog = Graphics.PixelPositionToValue(initialPlotAreaSize.Height, 0, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, yPosition + (maxBubbleRadius + topBottomOffset));
                    actualYValue = DataPoint.ConvertLogarithmicValue2ActualValue(chart, yValueInLog, axisY.AxisType);

                       // Calculate bottom point of bubble in terms of XValue and YValue 
                    Point pointBottom = new Point(
                        Graphics.PixelPositionToValue(0, initialPlotAreaSize.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, xPosition)
                        , actualYValue);

                    ctPoints.Add(pointLeft);
                    ctPoints.Add(pointRight);
                    ctPoints.Add(pointTop);
                    ctPoints.Add(pointBottom);
                }
                else
                {
                    // Calculate left point of bubble in terms of XValue and YValue 
                    Point pointLeft = new Point(
                        Graphics.PixelPositionToValue(0, initialPlotAreaSize.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, xPosition - (maxBubbleRadius + leftRightOffset))
                        , Graphics.PixelPositionToValue(0, initialPlotAreaSize.Height, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, yPosition));

                    // Calculate right point of bubble in terms of XValue and YValue 
                    Point pointRight = new Point(
                        Graphics.PixelPositionToValue(0, initialPlotAreaSize.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, xPosition + (maxBubbleRadius + leftRightOffset))
                        , Graphics.PixelPositionToValue(0, initialPlotAreaSize.Height, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, yPosition));

                    // Calculate top point of bubble in terms of XValue and YValue 
                    Point pointTop = new Point(
                        Graphics.PixelPositionToValue(0, initialPlotAreaSize.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, xPosition)
                        , Graphics.PixelPositionToValue(initialPlotAreaSize.Height, 0, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, yPosition - (maxBubbleRadius + topBottomOffset)));

                    // Calculate bottom point of bubble in terms of XValue and YValue 
                    Point pointBottom = new Point(
                        Graphics.PixelPositionToValue(0, initialPlotAreaSize.Width, plotGroup._initialAxisXMin, plotGroup._initialAxisXMax, xPosition)
                        , Graphics.PixelPositionToValue(initialPlotAreaSize.Height, 0, plotGroup._initialAxisYMin, plotGroup._initialAxisYMax, yPosition + (maxBubbleRadius + topBottomOffset)));

                    ctPoints.Add(pointLeft);
                    ctPoints.Add(pointRight);
                    ctPoints.Add(pointTop);
                    ctPoints.Add(pointBottom);
                
                }


            }

            return ctPoints;
        }
        
        /// <summary>
        /// Calculate bubble size in pixel
        /// </summary>
        /// <param name="dataPoint">DataPoint</param>
        /// <param name="minimumZVal">Minimum ZValue</param>
        /// <param name="maximumZVal">Maximum ZValue</param>
        /// <returns>Bubble Height and Width</returns>
        private static Double CalculateBubbleSize(DataPoint dataPoint, Double minimumZVal, Double maximumZVal)
        {
            Double value = !Double.IsNaN(dataPoint.ZValue) ? dataPoint.ZValue : (minimumZVal + maximumZVal) / 2;
            Double markerScale = Graphics.ConvertScale(minimumZVal, maximumZVal, value, 1, (Double)dataPoint.MarkerScale);
            Size markerSize = new Size((Double)dataPoint.MarkerSize, (Double)dataPoint.MarkerSize);

            Double markerScaleFactor = markerScale * (Double)dataPoint.MarkerScale;
            Double newSize = markerSize.Height * markerScaleFactor;
            
            return newSize;
        }

        private static void ApplyZValue(DataPoint dataPoint, Double minimumZVal, Double maximumZVal, Double drawingAreaWidth, Double drawingAreaHeight)
        {   
            Boolean animatedUpdate = true;

            Double value = !Double.IsNaN(dataPoint.ZValue) ? dataPoint.ZValue : (minimumZVal + maximumZVal) / 2;
            Double markerScale = Graphics.ConvertScale(minimumZVal, maximumZVal, value, 1, (Double)dataPoint.MarkerScale);
            Marker marker = dataPoint.Marker;
            
            marker.ScaleFactor = markerScale * (Double)dataPoint.MarkerScale;
            marker.MarkerSize = new Size((Double)dataPoint.MarkerSize, (Double)dataPoint.MarkerSize);
            
            if (marker.MarkerShape != null)
            {
                Double newSize = marker.MarkerSize.Height * marker.ScaleFactor;
                dataPoint._targetBubleSize = newSize;

                if(animatedUpdate)
                {
                    Double oldSize;

                    if (dataPoint.Storyboard != null)
                    {
                        dataPoint.Storyboard.Pause();
                        oldSize = marker.MarkerShape.Height;
                        dataPoint.Storyboard.Resume();
                    }
                    else
                        oldSize = marker.MarkerShape.Height;

                    marker.MarkerShape.Width = marker.MarkerShape.Height = newSize;
                    if (marker.MarkerShadow != null)
                        marker.MarkerShadow.Width = marker.MarkerShadow.Height = newSize;

                    UpdateBubblePositionAccording2XandYValue(dataPoint, drawingAreaWidth, drawingAreaHeight, animatedUpdate, oldSize, newSize);
                    
                    marker.MarkerShape.Width = marker.MarkerShape.Height = oldSize;
                    if (marker.MarkerShadow != null)
                        marker.MarkerShadow.Width = marker.MarkerShadow.Height = oldSize;

                }
                else
                {   
                    marker.MarkerShadow.Width = marker.MarkerShadow.Height = newSize;

                    if (marker.MarkerShadow != null)
                        marker.MarkerShadow.Width = marker.MarkerShadow.Height = newSize;
                }
            }
        }
        
        public static void Update(ObservableObject sender, VcProperties property, object newValue, Boolean isAxisChanged)
        {
            Boolean isDataPoint = sender.GetType().Equals(typeof(DataPoint));

            if (isDataPoint)
                UpdateDataPoint(sender as DataPoint, property, newValue, isAxisChanged);
            else
                UpdateDataSeries(sender as DataSeries, property, newValue, isAxisChanged);
        }

        private static void UpdateDataSeries(DataSeries dataSeries, VcProperties property, object newValue, Boolean isAxisChanged)
        {
            Chart chart = dataSeries.Chart as Chart;

            if (chart == null)
                return;

            switch (property)
            {
                case VcProperties.DataPoints:
                //case VcProperties.YValues:
                    chart.ChartArea.RenderSeries();
                    //Canvas ChartVisualCanvas = chart.ChartArea.ChartVisualCanvas;

                    //Double width = chart.ChartArea.ChartVisualCanvas.Width;
                    //Double height = chart.ChartArea.ChartVisualCanvas.Height;

                    //PlotDetails plotDetails = chart.PlotDetails;
                    //PlotGroup plotGroup = dataSeries.PlotGroup;

                    //List<DataSeries> dataSeriesListInDrawingOrder = plotDetails.SeriesDrawingIndex.Keys.ToList();

                    //List<DataSeries> selectedDataSeries4Rendering = new List<DataSeries>();

                    //RenderAs currentRenderAs = dataSeries.RenderAs;

                    //Int32 currentDrawingIndex = plotDetails.SeriesDrawingIndex[dataSeries];

                    //for (Int32 i = 0; i < chart.InternalSeries.Count; i++)
                    //{
                    //    if (currentRenderAs == dataSeriesListInDrawingOrder[i].RenderAs && currentDrawingIndex == plotDetails.SeriesDrawingIndex[dataSeriesListInDrawingOrder[i]])
                    //        selectedDataSeries4Rendering.Add(dataSeriesListInDrawingOrder[i]);
                    //}

                    //if (selectedDataSeries4Rendering.Count == 0)
                    //    return;

                    //Panel oldPanel = null;
                    //Dictionary<RenderAs, Panel> RenderedCanvasList = chart.ChartArea.RenderedCanvasList;

                    //if (chart.ChartArea.RenderedCanvasList.ContainsKey(currentRenderAs))
                    //{
                    //    oldPanel = RenderedCanvasList[currentRenderAs];
                    //}

                    //Panel renderedChart = chart.ChartArea.RenderSeriesFromList(oldPanel, selectedDataSeries4Rendering);

                    //if (oldPanel == null)
                    //{
                    //    chart.ChartArea.RenderedCanvasList.Add(currentRenderAs, renderedChart);
                    //    renderedChart.SetValue(Canvas.ZIndexProperty, currentDrawingIndex);
                    //    ChartVisualCanvas.Children.Add(renderedChart);
                    //}
                    //else
                    //    chart.ChartArea.RenderedCanvasList[currentRenderAs] = renderedChart;
                    break;
                default:

                    List<DataPoint> dataPointsInViewPort = RenderHelper.GetDataPointsUnderViewPort(dataSeries, false);

                    // case VcProperties.Enabled:
                    foreach (DataPoint dataPoint in dataPointsInViewPort)
                        UpdateDataPoint(dataPoint, property, newValue, isAxisChanged);
                    break;
            }
        }

        internal static void Update(Chart chart, RenderAs currentRenderAs, List<DataSeries> selectedDataSeries4Rendering, VcProperties property, object newValue)
        {   
            Boolean is3D = chart.View3D;
            ChartArea chartArea = chart.ChartArea;
            Canvas ChartVisualCanvas = chart.ChartArea.ChartVisualCanvas;

            // Double width = chart.ChartArea.ChartVisualCanvas.Width;
            // Double height = chart.ChartArea.ChartVisualCanvas.Height;

            Panel preExistingPanel = null;
            Dictionary<RenderAs, Panel> RenderedCanvasList = chart.ChartArea.RenderedCanvasList;

            if (chartArea.RenderedCanvasList.ContainsKey(currentRenderAs))
            {
                preExistingPanel = RenderedCanvasList[currentRenderAs];
            }

            Panel renderedChart = chartArea.RenderSeriesFromList(preExistingPanel, selectedDataSeries4Rendering);

            if (preExistingPanel == null)
            {
                chartArea.RenderedCanvasList.Add(currentRenderAs, renderedChart);
                ChartVisualCanvas.Children.Add(renderedChart);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataPoint"></param>
        /// <param name="property"></param>
        /// <param name="newValue"></param>
        /// <param name="isAxisChanged"></param>
        private static void UpdateDataPoint(DataPoint dataPoint, VcProperties property, object newValue, Boolean isAxisChanged)
        {   
            Chart chart = dataPoint.Chart as Chart;

            if (chart == null)
                return;

            PlotDetails plotDetails = chart.PlotDetails;
            Marker marker = dataPoint.Marker;
            DataSeries dataSeries = dataPoint.Parent;

            if (dataSeries.Faces == null)
                return;

            Canvas bubleChartCanvas = dataSeries.Faces.Visual as Canvas;
            
            Double plotHeight = chart.ChartArea.ChartVisualCanvas.Height;
            Double plotWidth = chart.ChartArea.ChartVisualCanvas.Width;
            Double minimumZVal, maximumZVal;

            ColumnChart.UpdateParentVisualCanvasSize(chart, bubleChartCanvas);

            if (property == VcProperties.Enabled || ((dataPoint.Faces == null || Double.IsNaN(dataPoint.InternalYValue)) && (property == VcProperties.XValue || property == VcProperties.YValue)))
            {   
                dataPoint._parsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);

                // Calculate max and min ZValue from all DataSeries
                List<DataSeries> seriesList = (from ds in chart.InternalSeries where ds.RenderAs == RenderAs.Bubble && ((Boolean)ds.Enabled == true) select ds).ToList(); 
                CalculateMaxAndMinZValueFromAllSeries(ref seriesList, out minimumZVal, out maximumZVal);

                CreateOrUpdateAPointDataPoint(bubleChartCanvas, dataPoint, minimumZVal, maximumZVal, plotWidth, plotHeight);
                return;
            }

            if (dataPoint.Faces == null)
                return;

            Grid bubbleVisual = dataPoint.Faces.Visual as Grid;
            
            switch (property)
            {   
                case VcProperties.Bevel:
                    break;
                
                case VcProperties.Cursor:
                    break;

                case VcProperties.Href:
                    dataPoint.SetHref2DataPointVisualFaces();
                    break;

                case VcProperties.HrefTarget:
                    dataPoint.SetHref2DataPointVisualFaces();
                    break;

                case VcProperties.LabelBackground:
                    if (marker != null)
                        marker.TextBackground = dataPoint.LabelBackground;
                    break;

                case VcProperties.LabelEnabled:
                    CalculateMaxAndMinZValue(dataPoint.Parent, out minimumZVal, out maximumZVal);
                    CreateOrUpdateAPointDataPoint(bubleChartCanvas, dataPoint, minimumZVal, maximumZVal, plotWidth, plotHeight);

                    // if (marker != null)
                    //   marker.LabelEnabled = (Boolean)dataPoint.LabelEnabled;

                    break;

                case VcProperties.LabelFontColor:
                    if (marker != null)
                        marker.FontColor = dataPoint.LabelFontColor;

                    break;

                case VcProperties.LabelFontFamily:
                    if (marker != null)
                        marker.FontFamily = dataPoint.LabelFontFamily;
                    break;

                case VcProperties.LabelFontStyle:
                    if (marker != null)
                        marker.FontStyle = (FontStyle)dataPoint.LabelFontStyle;
                    break;

                //case VcProperties.LabelFontSize:
                //    if (marker != null)
                //        marker.FontSize = (Double)dataPoint.LabelFontSize;
                //    break;

                case VcProperties.LabelFontWeight:
                    if (marker != null)
                        marker.FontWeight = (FontWeight)dataPoint.LabelFontWeight;
                    break;

                case VcProperties.LabelAngle:
                    if (marker != null)
                        marker.FontWeight = (FontWeight)dataPoint.LabelFontWeight;
                    break;

                case VcProperties.LegendText:
                    chart.InvokeRender();
                    break;

                case VcProperties.Color:
                case VcProperties.LightingEnabled:
                    if (marker != null)
                        marker.MarkerShape.Fill = (chart.View3D ? Graphics.Get3DBrushLighting(dataPoint.Color, (Boolean)dataPoint.LightingEnabled) : ((Boolean)dataPoint.LightingEnabled ? Graphics.GetLightingEnabledBrush(dataPoint.Color, "Linear", null) : dataPoint.Color));
                    break;

                case VcProperties.MarkerBorderColor:
                    if (marker != null)
                        marker.BorderColor = dataPoint.MarkerBorderColor;
                    break;

                case VcProperties.MarkerBorderThickness:
                    if (marker != null)
                        marker.BorderThickness = dataPoint.MarkerBorderThickness.Value.Left;
                    break;

                case VcProperties.XValueFormatString:
                case VcProperties.YValueFormatString:
                case VcProperties.LabelFontSize:
                case VcProperties.LabelStyle:
                case VcProperties.LabelText:
                case VcProperties.MarkerScale:
                case VcProperties.MarkerSize:
                case VcProperties.MarkerType:
                    dataPoint._parsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
                    CalculateMaxAndMinZValue(dataPoint.Parent, out minimumZVal, out maximumZVal);
                    CreateOrUpdateAPointDataPoint(bubleChartCanvas, dataPoint, minimumZVal, maximumZVal, plotWidth, plotHeight);
                    break;

                case VcProperties.ShadowEnabled:
                    if (marker != null)
                    {
                        marker.ShadowEnabled = (Boolean)dataPoint.ShadowEnabled;
                        marker.ApplyRemoveShadow();
                    }
                    break;

                case VcProperties.Opacity:
                    if (marker != null)
                        marker.Visual.Opacity = (Double)dataPoint.Opacity * (Double)dataSeries.Opacity;
                    break;

                case VcProperties.ShowInLegend:
                    chart.InvokeRender();
                    break;

                case VcProperties.ToolTipText:
                    dataPoint._parsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);
                    break;

                case VcProperties.XValueType:
                    chart.InvokeRender();
                    break;
                    
                case VcProperties.XValue:
                case VcProperties.YValue:
                case VcProperties.YValues:
                case VcProperties.DataPoints:

                    if (isAxisChanged)
                        UpdateDataSeries(dataSeries, property, newValue, false);
                    else
                        if(marker != null)
                        {   
                            dataPoint._parsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);

                            if ((Boolean)dataPoint.LabelEnabled)
                                marker.Text = dataPoint.TextParser(dataPoint.LabelText);

                            UpdateBubblePositionAccording2XandYValue(dataPoint, plotWidth, plotHeight, (Boolean)chart.AnimatedUpdate, marker.MarkerShape.Width, marker.MarkerShape.Width);
                        }

                    break;

                case VcProperties.ZValue:

                    dataPoint._parsedToolTipText = dataPoint.TextParser(dataPoint.ToolTipText);

                    //if ((Boolean)dataPoint.LabelEnabled)
                    //    marker.Text = dataPoint.TextParser(dataPoint.LabelText);

                    List<DataSeries> seriesList = (from ds in chart.InternalSeries where ds.RenderAs == RenderAs.Bubble && ((Boolean)ds.Enabled == true) select ds).ToList();
                    CalculateMaxAndMinZValueFromAllSeries(ref seriesList, out minimumZVal, out maximumZVal);
                        
                    // CalculateMaxAndMinZValue(dataPoint.Parent, out minimumZVal, out maximumZVal);
                    foreach (DataSeries ds in seriesList)
                    {
                        foreach (DataPoint dp in ds.InternalDataPoints)
                        {   
                            if (Double.IsNaN(dp.InternalYValue) || (dp.Enabled == false))
                                continue;

                            ApplyZValue(dp, minimumZVal, maximumZVal, plotWidth, plotHeight);
                        }
                    }

                    break;
            }
            
            if (bubleChartCanvas.Parent != null)
            {
                RectangleGeometry clipRectangle = new RectangleGeometry();
                clipRectangle.Rect = new Rect(0, -(dataPoint.Chart as Chart).ChartArea.PLANK_DEPTH, plotWidth + (dataPoint.Chart as Chart).ChartArea.PLANK_OFFSET, plotHeight + (dataPoint.Chart as Chart).ChartArea.PLANK_DEPTH);
                (bubleChartCanvas.Parent as Canvas).Clip = clipRectangle;
            }
        }

        #endregion

        #region Internal Events And Delegates

        #endregion

        #region Data

        #endregion

    }
}
